# youtube

Project instructions for agents working in this repo. Fleshed out as the project is worked on.

## Overview

_TODO: what this project is and does._

## Agent files

- `AgentDocumentation/Project.md` (this file): project overview + project-specific rules.
- `AgentDocumentation/Memory.md`: project long-term memory.
- `AgentDocumentation/Sessions/`: per-session `SessionMemory-*.md` reflection files.

## Rules

Inherits all global rules from `AgentDocumentationGlobal/Global.md` (safety, no-secrets, naming, push-back, no workarounds). Project-specific rules go here as they emerge.

## Public repository — confidentiality rules

This is a PUBLIC GitHub repository. Its audience includes people who have no relationship to AngelSix. Therefore:

- **Never record information about other repositories** in any file here — not their names, locations, folder structures, or contents. Refer to them only as "a separate private repo".
- **Never record absolute local filesystem paths** (e.g. `/Users/<name>/...`). Paths relative to this repo are fine.
- **Never disclose business details of AngelSix's other ventures** (company sites, products, customers, incidents) beyond what is already public on angelsix.com.
- **Never name internal infrastructure or agent-toolchain specifics** — server/hostnames, model identities, private tool names, workflow internals — even in code comments, MSBuild props, or doc prose. Build files ship to every fork; a comment explaining *why* a target exists must describe the engineering reason generically, never the measurement environment it came from. Proprietary tools referenced by build steps must degrade silently when absent (detect natively, skip), because a public clone will never have them.
- Session notes (`AgentDocumentation/Sessions/`, gitignored) are exempt from shipping but NOT from these rules — assume anything written there could be recovered from history.
- When unsure whether a fact is public, treat it as non-public and leave it out.
