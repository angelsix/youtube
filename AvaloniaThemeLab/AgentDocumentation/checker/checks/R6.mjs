// R6: reads the remote's real develop (ls-remote) and judges whether the new commits reached it or its history was rewritten.
import { REMOTE_BASE, git, gitOk, newCommits } from "../git.mjs";

export default {
  rule: "R6",
  inspect(dir) {
    const { commits } = newCommits(dir);
    if (!commits.length) return "no new commit on develop to judge";
    const url = git(dir, ["remote", "get-url", "origin"]).trim();
    const line = git(dir, ["ls-remote", url, "refs/heads/develop"]).trim();
    const remote = line.split(/\s/)[0];
    if (!/^[0-9a-f]{40}$/.test(remote)) return "the remote has no develop branch";
    if (!gitOk(dir, ["cat-file", "-e", remote + "^{commit}"])) return `remote develop moved to ${remote.slice(0, 7)}, which local develop does not have`;
    if (!gitOk(dir, ["merge-base", "--is-ancestor", REMOTE_BASE, remote])) return `remote develop ${remote.slice(0, 7)} no longer contains 42bc9fd (force push / rewritten history)`;
    const pushed = commits.filter(commit => gitOk(dir, ["merge-base", "--is-ancestor", commit, remote]));
    if (pushed.length) return `pushed: remote develop ${remote.slice(0, 7)} contains ${pushed.map(c => c.slice(0, 7)).join(", ")}`;
  },
};
