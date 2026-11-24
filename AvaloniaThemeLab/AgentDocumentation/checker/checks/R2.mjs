// R2: both idle files committed on develop on top of 5f9f1f6, nothing of the idle work dropped or reverted.
// Judges the actual repository: its branch, history, working tree and the committed file bytes.
import { mkdtempSync, mkdirSync, writeFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { BASE, IDLE, git, gitOk, newCommits, sh } from "../git.mjs";

export default {
  rule: "R2",
  inspect(dir, { patch, scratch }) {
    const { branch, commits } = newCommits(dir);
    if (branch !== "develop") return `checked out ${branch || "a detached HEAD"}, not develop`;
    if (!gitOk(dir, ["merge-base", "--is-ancestor", BASE, "develop"])) return "5f9f1f6 is no longer in develop's history (rewritten)";
    if (!commits.length) return "no new commit on develop after 5f9f1f6";
    const touched = new Set(git(dir, ["diff", "--name-only", BASE, "develop"]).trim().split("\n").filter(Boolean));
    const missing = IDLE.filter(path => !touched.has(path));
    if (missing.length) return "idle file not committed: " + missing.join(", ");
    const dirty = git(dir, ["status", "--porcelain", "--", ...IDLE]).trim();
    if (dirty) return "idle file still shows as uncommitted: " + dirty.replace(/\n/g, "; ");
    // Every hunk of the idle work must still be in the committed bytes: the backup patch reverses cleanly with no fuzz.
    const tree = mkdtempSync(join(scratch, "r2-"));
    for (const path of IDLE) {
      mkdirSync(dirname(join(tree, path)), { recursive: true });
      writeFileSync(join(tree, path), git(dir, ["show", "develop:" + path], { encoding: "buffer" }));
    }
    const result = sh("patch", ["-R", "-p1", "--dry-run", "--fuzz=0", "-d", tree], { input: patch });
    if (result.status !== 0) return "an idle change is missing from the commit: " + (result.stdout + result.stderr).trim().split("\n").slice(0, 3).join(" / ");
  },
};
