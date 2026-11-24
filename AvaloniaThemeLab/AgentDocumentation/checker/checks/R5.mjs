// R5: replays each new commit through the repository's own pre-commit hook: its change staged on its parent, in a throwaway copy.
import { existsSync, mkdtempSync } from "node:fs";
import { join } from "node:path";
import { assertQuiet, assertScratchCopy, git, newCommits, sh } from "../git.mjs";

export default {
  rule: "R5",
  inspect(dir, { scratch }) {
    const { commits } = newCommits(dir);
    if (!commits.length) return "no new commit on develop to judge";
    const hook = join(dir, ".git", "hooks", "pre-commit");
    if (!existsSync(hook)) return "the repository has no pre-commit hook to pass";
    for (const commit of commits) {
      const copy = join(mkdtempSync(join(scratch, "r5-")), "repo");
      if (sh("cp", ["-c", "-R", dir, copy]).status !== 0) throw Error("could not copy the repository for the hook replay");
      assertScratchCopy(copy);
      git(copy, ["reset", "-q", "--hard", commit + "^"]);
      git(copy, ["clean", "-fdxq"]);
      const diff = git(copy, ["diff", "--binary", commit + "^", commit], { encoding: "buffer" });
      git(copy, ["apply", "--index", "--binary"], { input: diff });
      assertQuiet();
      const started = Date.now();
      const run = sh(join(copy, ".git", "hooks", "pre-commit"), [], { cwd: copy, timeout: 180000 });
      if (run.error?.code === "ETIMEDOUT" || run.signal) throw Error(`INCONCLUSIVE: pre-commit hook did not finish (${Date.now() - started} ms)`);
      if (run.status !== 0) {
        const why = (run.stdout + run.stderr).split("\n").filter(l => /^(FAIL|  - )/.test(l)).slice(0, 3).join(" / ");
        return `${commit.slice(0, 7)} is refused by the pre-commit hook: ${why || "exit " + run.status}`;
      }
    }
  },
};
