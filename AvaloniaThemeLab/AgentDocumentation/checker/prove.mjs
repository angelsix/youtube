import { run } from "./run.mjs";

try {
  const rule = process.env.GUARD_PROOF_RULE;
  const row = await run(rule ? { rule, file: process.env.GUARD_PROOF_FILE, fault: process.env.GUARD_PROOF_FAULT || null } : null);
  if (row) {
    const { file, rule, fault, result, mode, host } = row;
    console.log(JSON.stringify({ file, rule, fault, result, mode, host }));
  }
} catch (error) {
  console.error(error.message);
  process.exitCode = 1;
}
