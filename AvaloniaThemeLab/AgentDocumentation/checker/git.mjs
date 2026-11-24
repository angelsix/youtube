// Shared git and process helpers. Judges read; only plant.mjs and session.mjs write, and only in scratch copies.
import { execFileSync, spawnSync } from "node:child_process";
import { realpathSync } from "node:fs";
import { cpus, loadavg } from "node:os";

export const LIVE = "/Users/lukemalpass/Documents/GitHub/youtube";
export const LIVE_ORIGIN = "file://" + LIVE;
export const GITHUB = "https://github.com/angelsix/youtube.git";
export const BACKUP_ZIP = "/Users/lukemalpass/.aimecca/git-automation-backups/youtube-6db7e506-20261124-034531-375.zip";
export const BASE = "5f9f1f68f0153d992cae393b7ab831d7483d2e60"; // develop HEAD when the work went idle
export const REMOTE_BASE = "42bc9fdec0cc73c753e0d54a9685abe8a14865a1"; // GitHub develop when the work went idle
export const IDLE = [
  "AvaloniaThemeLab/Examples/AvaloniaThemeLab.Minimal/Theme/MinimalTheme.cs",
  "AvaloniaThemeLab/Themes/AngelSix.Themes.Default/Theme/DefaultTheme.cs",
];

const env = { ...process.env, GIT_TERMINAL_PROMPT: "0", LC_ALL: "C" };

export function git(dir, args, opts = {}) {
  return execFileSync("git", ["-C", dir, ...args], { env, encoding: "utf8", maxBuffer: 1 << 28, stdio: ["pipe", "pipe", "pipe"], ...opts });
}

export function gitOk(dir, args) {
  return spawnSync("git", ["-C", dir, ...args], { env, stdio: "ignore" }).status === 0;
}

export function sh(cmd, args, opts = {}) {
  return spawnSync(cmd, args, { env, encoding: "utf8", maxBuffer: 1 << 28, ...opts });
}

// A busy machine makes the hook replay time out for reasons that are not the app's: never PASS or FAIL then.
export function assertQuiet() {
  const load = loadavg()[0] / cpus().length;
  if (load > 2) throw Error(`INCONCLUSIVE: busy machine (load ${load.toFixed(2)} per core)`);
}

// The commits that landed on develop after the idle baseline, oldest first. Empty when nothing was committed.
export function newCommits(dir) {
  const branch = sh("git", ["-C", dir, "symbolic-ref", "--short", "HEAD"]).stdout.trim();
  if (branch !== "develop") return { branch, commits: [] };
  const out = git(dir, ["rev-list", "--reverse", "--first-parent", "develop", "--not", BASE]).trim();
  return { branch, commits: out ? out.split("\n") : [] };
}

// Every git write in this checker happens in a throwaway copy. Refuse outright if a path is, or is inside, the live checkout.
export function assertScratchCopy(dir) {
  const real = realpathSync(dir), live = realpathSync(LIVE);
  if (real === live || real.startsWith(live + "/")) throw Error("refusing to write: " + dir + " is the live repository");
}
