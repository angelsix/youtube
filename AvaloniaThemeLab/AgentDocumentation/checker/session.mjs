// Builds one variant: an APFS copy of the live repository at an opaque scratch path, whose origin is a bare mirror
// filled from GitHub's real develop at measure time. The clean variant is the live repo exactly as it stands.
// Judges receive only the variant's path; the planted fault (if any) is applied by plant.mjs before they run.
import { randomUUID } from "node:crypto";
import { mkdirSync } from "node:fs";
import { join } from "node:path";
import { GITHUB, LIVE, assertScratchCopy, git, gitOk, sh } from "./git.mjs";

export function variant(scratch) {
  const root = join(scratch, randomUUID());
  mkdirSync(root);
  const dir = join(root, randomUUID());
  if (sh("cp", ["-c", "-R", LIVE, dir]).status !== 0) throw Error("could not copy the live repository");
  assertScratchCopy(dir);
  const mirror = join(root, randomUUID() + ".git");
  // Read GitHub's real develop (read-only); fetch through the named remote only if that commit is not local yet.
  const remote = git(dir, ["ls-remote", GITHUB, "refs/heads/develop"], { timeout: 120000 }).split(/\s/)[0];
  if (!/^[0-9a-f]{40}$/.test(remote)) throw Error("INCONCLUSIVE: could not read GitHub develop");
  if (!gitOk(dir, ["cat-file", "-e", remote + "^{commit}"])) git(dir, ["fetch", "-q", "--no-tags", "origin"], { timeout: 120000 });
  sh("git", ["init", "-q", "--bare", mirror]);
  git(dir, ["push", "-q", mirror, remote + ":refs/heads/develop"]);
  git(dir, ["remote", "set-url", "origin", mirror]);
  git(dir, ["remote", "set-url", "--push", "origin", mirror]);
  git(dir, ["update-ref", "refs/remotes/origin/develop", remote]);
  return { dir, mirror, remote };
}
