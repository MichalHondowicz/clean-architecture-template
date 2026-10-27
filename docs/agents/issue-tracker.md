# Issue tracker: Local Markdown

Issues and specs for this repo live as markdown files in `.scratch/`.

## Conventions

- One feature per directory: `.scratch/<feature-slug>/`
- The spec is `.scratch/<feature-slug>/spec.md`
- Implementation issues are one file per ticket at `.scratch/<feature-slug>/issues/<NN>-<slug>.md`, numbered from `01`, never a single combined tickets file
- Triage state is recorded as a `Status:` line near the top of each issue file (see `triage-labels.md` for the role strings)
- Comments and conversation history append to the bottom of the file under a `## Comments` heading
- When `/implement` finishes a ticket: tick its acceptance criteria, append the result (build, test counts, red proof) under `## Comments`, and set `Status: done`

## Ticket format for unattended work

A ticket that `/ship` can run without asking anything carries, besides the `to-tickets` fields:

- `**Seams:**` under `**Blocked by:**`: where its tests observe behaviour, following ADR 0004 (HTTP first; handler tests for failure paths and time; one validator test per rule). Approving the ticket approves its seams, so `tdd` does not ask about them again.
- Acceptance criteria numbered `AC1`, `AC2`, ..., each naming the test planned to prove it: `- [ ] AC1: <criterion> (test: \`<TestClass>.<Method>\`)`. The implementer may rename a test; its `## Comments` entry then gives the real name for that `ACn`.

`/ship` owns the status line while it runs: `in-progress` when it starts a ticket, `done` once the verifier and the spec reviewer pass it, `needs-info` when it is stuck after three rounds (or the implementer reports itself stuck). The implementer's own contract is in `implementer-contract.md`.

What `/ship` leaves in `.scratch/<feature>/` besides the tickets:

- `findings/<NN>-round-<r>.md`: the in-scope findings that failed round `r` of ticket `NN`, the next round's input. Their count is how many rounds the ticket has spent.
- Under a `needs-info` ticket's `## Comments`: a brief in three parts, **Tried**, **Where it stands** (last findings, stash name, reverted commits) and **Needs**.
- New tickets with `Status: needs-triage` for findings outside the reviewed ticket's change, numbered after the existing ones, saying which ticket and round they came from. They wait for triage and block nothing.
- `metrics.md`: a table with a row per finished ticket (plan usage of the 5-hour and weekly windows at its start and end, rounds, model of each role, time, result), a `branch` row for the review of the whole branch, a `pause` row each time the usage reached a threshold in `dod.json`'s `quota`, and a `**Total**` row when a run ends with nothing left that it can do. A 5-hour pause waits in the same session; a weekly pause ends it, and the user runs `/ship` again after the reset.

Once every ticket is `done`, `/ship` reviews the whole branch on Opus (the project's standards skill named in `dod.json`'s `standardsSkill`, and the Spec axis against `spec.md` and all tickets) and leaves:

- `findings/branch-review.md`: a `Started` line with the plan usage, then both reviewers' reports as they came, with findings, notes and convention proposals.
- `findings/branch-round-<r>.md`: the in-scope findings that fix round `r` of the branch addresses (round 1 from the review, rounds 2 and 3 from the re-check of the previous fix). The implementer appends a `## Resolution`, and `/ship` ends the file with a `## Result`: `resolved`, the next round, or `open` with the commit the code went back to when the last round was not verified. Their count is how many fix rounds the branch has spent, three at most.
- `brief.md`: the readiness brief for gate 2. It lists what was done, the test counts, the review findings (resolved and open), what is uncertain, the changed screens when `dod.json` names `screenPaths`, the convention proposals (each with its target file, the rule quoted ready to paste, its rationale and the ticket it came from), whether Remote Control was on, and the gate 2 question. It is committed as `ship: brief`, and a push points at it.
- Under `## Decision` at the end of `brief.md`: one entry per gate 2 answer (time, `merge: OK` or `merge: no`, each proposal accepted, rejected or dropped, the answer verbatim). Accepted rules go into their target files (`AGENTS.md` or the `ca-review` checklist) in the same commit, `ship: gate 2, conventions <n>` or `ship: gate 2 decision`. After `OK`, `/ship` fast-forwards `main`, runs `git push origin main`, removes the feature's worktree and branch, and reports the CI run of that push.

## When a skill says "publish to the issue tracker"

Create a new file under `.scratch/<feature-slug>/` (creating the directory if needed).

## When a skill says "fetch the relevant ticket"

Read the file at the referenced path. The user will normally pass the path or the issue number directly.

## Wayfinding operations

Used by `/wayfinder`. The **map** is a file with one **child** file per ticket.

- **Map**: `.scratch/<effort>/map.md` (the Notes / Decisions-so-far / Fog body).
- **Child ticket**: `.scratch/<effort>/issues/NN-<slug>.md`, numbered from `01`, with the question in the body. A `Type:` line records the ticket type (`research`/`prototype`/`grilling`/`task`); a `Status:` line records `claimed`/`resolved`.
- **Blocking**: a `Blocked by: NN, NN` line near the top. A ticket is unblocked when every file it lists is `resolved`.
- **Frontier**: scan `.scratch/<effort>/issues/` for files that are open, unblocked, and unclaimed; first by number wins.
- **Claim**: set `Status: claimed` and save before any work.
- **Resolve**: append the answer under an `## Answer` heading, set `Status: resolved`, then append a context pointer (gist + link) to the map's Decisions-so-far in `map.md`.
