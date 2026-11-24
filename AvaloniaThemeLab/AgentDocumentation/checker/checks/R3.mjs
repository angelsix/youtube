// R3: the new commits carry only the idle files (and at most a narrow .gitignore entry); no secrets, build output, logs, scratch or temp files.
import { IDLE, git, newCommits } from "../git.mjs";

const forbidden = [
  [/(^|\/)\.env(\.|$)|\.(pem|key|p12|pfx)$|(^|\/)(credentials|secrets?)(\.|\/|$)|id_rsa/i, "a secrets file"],
  [/(^|\/)(bin|obj|out|publish|node_modules)\//i, "build output"],
  [/\.(dll|exe|pdb|nupkg|log|tmp|bak)$|~$|(^|\/)\.DS_Store$/i, "a log, binary or temporary file"],
  [/(^|\/)Scratch\//, "scratch"],
];
const secretText = /AKIA[0-9A-Z]{16}|-----BEGIN [A-Z ]*PRIVATE KEY|sk-(proj-)?[A-Za-z0-9]{20,}|gh[pousr]_[A-Za-z0-9]{30,}|(api[_-]?key|password|secret|token)\s*[:=]\s*["']?[A-Za-z0-9\/+_\-]{12,}/i;

export default {
  rule: "R3",
  inspect(dir) {
    const { commits } = newCommits(dir);
    if (!commits.length) return "no new commit on develop to judge";
    for (const commit of commits) {
      const files = git(dir, ["diff-tree", "--no-commit-id", "-r", "--name-only", "--root", commit]).trim().split("\n").filter(Boolean);
      for (const path of files) {
        const hit = forbidden.find(([pattern]) => pattern.test(path));
        if (hit) return `${commit.slice(0, 7)} commits ${hit[1]}: ${path}`;
        if (path === ".gitignore" || path.endsWith("/.gitignore")) {
          const lines = git(dir, ["diff", commit + "^", commit, "--", path]).split("\n").filter(l => /^[+-][^+-]/.test(l));
          if (lines.some(l => l.startsWith("-")) || lines.length > 5) return `${commit.slice(0, 7)} makes a broad .gitignore change (${lines.length} lines)`;
          continue;
        }
        if (!IDLE.includes(path)) return `${commit.slice(0, 7)} commits a file outside the idle work: ${path}`;
      }
      const added = git(dir, ["show", "--format=", "-U0", commit]).split("\n").filter(l => l.startsWith("+") && !l.startsWith("+++"));
      const secret = added.find(l => secretText.test(l));
      if (secret) return `${commit.slice(0, 7)} adds what looks like a secret`;
    }
  },
};
