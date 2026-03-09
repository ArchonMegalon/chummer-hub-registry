# Project Chummer Design

This directory is the canonical human-readable source for Chummer's cross-repo design.

## Front door
- Start design work here before updating any code repo-local mirror.
- Treat documents in this directory as the approval gate for cross-repo product, ownership, blocker, milestone, and contract changes.
- After approval, Fleet mirrors only the affected subset into each code repo so workers and GitHub review stay design-aware without duplicating the full canon.

## Active repos
- `chummer-core-engine`
- `chummer-presentation`
- `chummer.run-services`
- `chummer-play`
- `chummer-ui-kit`
- `chummer-hub-registry`

## Current direction
- stabilize the contract plane first
- complete the play split cleanly
- extract the shared UI kit
- extract hub registry
- extract media factory after render-versus-narrative cleanup

## Sync workflow
1. Update the canonical product docs in this repo.
2. Approve the design change here before any code-repo mirror publish.
3. Publish the affected files listed in `sync/sync-manifest.yaml`.
4. Land the mirror in the destination repo under `.codex-design/product`, `.codex-design/repo`, or `.codex-design/review`.
5. Use the repo-local mirror during implementation and review, and treat drift from canon as an audit failure.
