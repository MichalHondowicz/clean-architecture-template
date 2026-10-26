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

`/ship` owns the status line while it runs: `in-progress` when it starts a ticket, `done` after the implementer finishes, `needs-info` (with the implementer's note under `## Comments`) when the implementer is stuck. The implementer's own contract is in `implementer-contract.md`.

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
