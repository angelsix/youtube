# GenerateContextMap
Status: READY
Author AgentDocumentation/ContextMap.md for this .NET project so agents have an orientation map and 'guard repo context-map' passes. Groundwork — tackle as and when.
## The plan
Author `AgentDocumentation/ContextMap.md` for this .NET project using the `context-map` skill: survey the solutions and projects, name the subsystems and where things live, capture the key flows and invariants, and add a short "run `contextmap impact` before editing" note. The `contextmap` tool is installed for live blast-radius queries. It makes `guard repo context-map` pass and gives agents an orientation map before they change code.
## Tasks
- [ ] 1. Author AgentDocumentation/ContextMap.md via the context-map skill (survey the code, then write the orientation map) [risk: low]
    - Done when: guard repo context-map against this repo exits 0 (ContextMap.md present and valid)
## Decisions and trade-offs
## Pinned terms
