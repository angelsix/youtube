// R4: every new commit authored by Luke, with a plain specific subject and no AI attribution anywhere in the message.
import { git, newCommits } from "../git.mjs";

const placeholder = /^(wip|update[sd]?|changes?|misc|fix(es)?|auto[ -]?(git|commit)|commit|idle (work|files)|tidy|stuff)\b[.!: ]*(the )?(files|work|changes)?\.?$/i;
const attribution = /co-authored-by|generated (with|by)|🤖|\bclaude\b|\bcodex\b|anthropic|openai|chatgpt|\bgpt-?\d|\bcopilot\b|\bA\.?I\.?\b|\bLLM\b|\bagent\b|noreply@anthropic/i;

export default {
  rule: "R4",
  inspect(dir) {
    const { commits } = newCommits(dir);
    if (!commits.length) return "no new commit on develop to judge";
    for (const commit of commits) {
      const [author, email, committer, cemail] = git(dir, ["show", "-s", "--format=%an%n%ae%n%cn%n%ce", commit]).trim().split("\n");
      if (author !== "Luke Malpass" || committer !== "Luke Malpass") return `${commit.slice(0, 7)} authored/committed as ${author} <${email}> / ${committer} <${cemail}>`;
      const message = git(dir, ["show", "-s", "--format=%B", commit]);
      const subject = message.split("\n")[0].trim();
      if (subject.length < 12 || placeholder.test(subject)) return `${commit.slice(0, 7)} has a placeholder subject: "${subject}"`;
      if (!/theme|minimal|default|stage|palette|colou?r/i.test(subject)) return `${commit.slice(0, 7)} subject does not say what changed in the theme files: "${subject}"`;
      const hit = message.match(attribution);
      if (hit) return `${commit.slice(0, 7)} message carries AI attribution: "${hit[0]}"`;
    }
  },
};
