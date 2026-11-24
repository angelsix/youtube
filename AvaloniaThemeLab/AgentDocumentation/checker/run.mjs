// Runner: for each planted fault, builds a fresh variant at an opaque path, plants it, and runs the judges for the
// rules that fault is assigned to; then judges the clean live repository through the same kind of variant.
// Writes checker/out/proof.json only when every rule FAILED on its faults and PASSED clean, clean run last.
// `node checker/run.mjs --self-test` judges a correct reference commit instead and writes nothing.
import { createHash, randomUUID } from "node:crypto";
import { execFileSync } from "node:child_process";
import { mkdtemp, mkdir, readFile, rename, writeFile } from "node:fs/promises";
import { tmpdir } from "node:os";
import { join } from "node:path";
import { pathToFileURL } from "node:url";
import R2 from "./checks/R2.mjs";
import R3 from "./checks/R3.mjs";
import R4 from "./checks/R4.mjs";
import R5 from "./checks/R5.mjs";
import R6 from "./checks/R6.mjs";
import { BACKUP_ZIP, IDLE, LIVE_ORIGIN, assertQuiet, git, newCommits } from "./git.mjs";
import { faultIds, plant } from "./plant.mjs";
import { variant } from "./session.mjs";

const root = new URL("../", import.meta.url);
const checks = [R2, R3, R4, R5, R6];
const ids = checks.map(check => check.rule);
const mutating = new Set(["R2", "R3", "R4", "R5"]);
const file = rule => `checker/checks/${rule}.mjs`;
const hash = bytes => createHash("sha256").update(bytes).digest("hex");
const read = name => readFile(new URL(name, root));
const inputNames = ["checker/git.mjs", "checker/plant.mjs", "checker/prove.mjs", "checker/run.mjs", "checker/session.mjs", ...ids.map(file)].sort();

async function binding() {
  const inputs = {};
  for (const name of inputNames) inputs[name] = hash(await read(name));
  return {
    inputs,
    importClosureSha256: hash(inputNames.map(name => inputs[name] + "  " + name + "\n").join("")),
    rulesFileSha256: hash(await read("VerificationRules.md")),
    faultManifestSha256: hash(await read("faults.json")),
    mode: "full", host: "local",
  };
}

function ruleHash(text, id) {
  const block = text.split(/^R(?=\d+\. )/m).find(section => section.startsWith(id.slice(1) + ". "));
  const fields = ["Surface", "Action", "Must", "Must not", "Environment"].map(name => {
    const field = block?.match(new RegExp("(?:^\\d+\\. |^ +)" + name + ": (.+)$", "m"));
    if (!field) throw Error("Missing " + id + " " + name);
    return field[1].trim().replace(/\s+/g, " ");
  });
  return hash(fields.join("\n"));
}

export async function run(replay = null, selfTest = false) {
  const initial = await binding();
  const rules = (await read("VerificationRules.md")).toString();
  const manifest = JSON.parse(await read("faults.json"));
  if (new Set(manifest.map(row => row.id)).size !== manifest.length ||
      manifest.some(row => !faultIds.includes(row.id) || !row.rules.length || row.rules.some(id => !ids.includes(id))) ||
      ids.some(id => !manifest.some(row => row.rules.includes(id)))) throw Error("Invalid fault manifest");
  const scratch = await mkdtemp(join(process.env.AIMECCA_SCRATCH || tmpdir(), "autogit-"));
  console.error("Scratch", scratch);
  const patch = execFileSync("unzip", ["-p", BACKUP_ZIP, "changes.patch"]);
  if (!IDLE.every(path => patch.includes("+++ b/" + path))) throw Error("Backup patch does not carry the idle files");

  async function attempt(fault, selection) {
    assertQuiet();
    const v = variant(scratch);
    if (fault) plant(fault, v, patch);
    const failures = new Map();
    for (const check of checks.filter(check => selection.includes(check.rule))) {
      const failure = check.inspect(v.dir, { patch, scratch });
      if (failure) failures.set(check.rule, failure);
    }
    if (JSON.stringify(await binding()) !== JSON.stringify(initial)) throw Error("Changed inputs during the run");
    const { commits } = newCommits(v.dir);
    const landed = commits.length > 0 && IDLE.every(path => git(v.dir, ["log", "--format=%H", "-1", "develop", "--", path]).trim() &&
      commits.includes(git(v.dir, ["log", "--format=%H", "-1", "develop", "--", path]).trim()));
    return selection.map(rule => ({
      file: file(rule), rule, fault, result: failures.has(rule) ? "FAILED" : "PASSED",
      host: initial.host, mode: initial.mode, timestamp: new Date().toISOString(),
      appRevision: git(v.dir, ["rev-parse", "develop"]).trim(), writes: mutating.has(rule) && landed,
      detail: failures.get(rule),
    }));
  }
  async function sample(fault, selection) {
    for (let trial = 1; ; trial++) {
      try {
        return await attempt(fault, selection);
      } catch (error) {
        if (trial >= 3 || !error.message.startsWith("INCONCLUSIVE")) throw error;
        console.error(error.message + "; discarding this run and repeating the full measurement");
      }
    }
  }
  const report = row => console.log([row.fault ?? (selfTest ? "reference" : "clean"), row.rule, row.result, row.detail].filter(Boolean).join(" | "));
  if (selfTest) {
    const v = variant(scratch);
    plant("reference", v, patch);
    for (const check of checks) { const detail = check.inspect(v.dir, { patch, scratch }); report({ rule: check.rule, result: detail ? "FAILED" : "PASSED", detail }); }
    return;
  }
  if (replay) {
    const { rule, fault } = replay;
    if (!ids.includes(rule) || replay.file !== file(rule) ||
        (fault && !manifest.some(row => row.id === fault && row.rules.includes(rule)))) throw Error("Invalid replay selection");
    const [observation] = await sample(fault, [rule]);
    return observation;
  }
  const red = new Map();
  for (const fault of manifest) {
    for (const row of await sample(fault.id, fault.rules)) {
      report(row);
      if (row.result !== "FAILED") throw Error("Missed fault " + fault.id + " for " + row.rule);
      if (!red.has(row.rule)) red.set(row.rule, row);
    }
  }
  const proof = [];
  const clean = await sample(null, ids);
  clean.forEach(report);
  const failedClean = clean.filter(row => row.result !== "PASSED");
  if (failedClean.length) throw Error("Clean live repository fails " + failedClean.map(row => row.rule).join(", ") + ": no proof written");
  for (const row of clean) {
    const failed = red.get(row.rule);
    if (failed.timestamp >= row.timestamp) throw Error("Invalid red/green ordering");
    const sha256 = initial.inputs[row.file];
    proof.push({ file: row.file, rule: row.rule, ruleSha256: ruleHash(rules, row.rule), ...initial,
      origin: LIVE_ORIGIN, auth: "none", writes: row.writes, appRevision: row.appRevision,
      ...(mutating.has(row.rule) ? { write: "commit the idle files" } : {}),
      red: { result: failed.result, fault: failed.fault, timestamp: failed.timestamp, sha256 },
      green: { result: row.result, timestamp: row.timestamp, sha256 },
    });
  }
  await mkdir(new URL("checker/out/", root), { recursive: true });
  const temp = new URL("checker/out/" + randomUUID() + ".tmp", root);
  await writeFile(temp, JSON.stringify({ checks: proof }, null, 2) + "\n");
  await rename(temp, new URL("checker/out/proof.json", root));
  console.log("Confirmed red then green:", proof.length, "of", ids.length);
}

if (process.argv[1] && pathToFileURL(process.argv[1]).href === import.meta.url) await run(null, process.argv.includes("--self-test"));
