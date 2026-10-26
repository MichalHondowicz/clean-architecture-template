# Implementer contract

The implementer is the role that builds one ticket while `/ship` runs a feature unattended. Today it is the
`mattpocock-skills:implementer` subagent; its name is `implementerAgent` in `dod.json`, and the hooks in
`.claude/hooks/` apply to whatever agent carries that name. A different model taking the role gets this same
contract and the same hooks.

## Input

From the orchestrator (`/ship`), as pointers, not copies:

- the ticket path, `.scratch/<feature>/issues/<NN>-<slug>.md`, with `**Seams:**` and criteria `AC1…n`, each
  naming its planned test (format: `issue-tracker.md`);
- the worktree it works in (its working directory) and the feature branch `feat/<feature>`;
- the round number and, from round 2, the file with the review findings to address.

## Output

- Commits on the feature branch with the ticket's work and its `## Comments` entry. The implementer never
  changes the ticket's `**Status:**` line: `/ship` owns it.
- The `## Comments` entry: for each `ACn`, the test that proves it and its red evidence (the failing assertion
  seen in the cycle, or a red proof: one mechanism broken, the test failing, the file restored with a matching
  hash), plus build and test counts.
- A final message whose first line is `STATUS: done` or `STATUS: stuck`. After `stuck`: what was tried, where
  the work stands, and what is needed to go on.

## Rules

- Test at the ticket's seams with `tdd`. They were approved with the ticket: never ask about them, never ask
  the user anything.
- The DoD in `dod.json` is enforced on stop: the implementer cannot finish while a DoD command fails.
- Never write a path listed in `protectedPaths` of `dod.json`, nor anything outside the worktree. Edit and
  Write are refused there; a change made any other way shows up in the verifier's diff check. If the ticket
  cannot be done without such a change, finish with `STATUS: stuck` and say why.
- Never weaken a test or add an analyzer suppression (`AGENTS.md`). Changed existing tests are listed for the
  verifier to judge.
- Never push, switch branches, or change another ticket.
