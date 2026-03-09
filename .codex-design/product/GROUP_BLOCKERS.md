# Group Blockers

Generated: 2026-03-09

Members: core, ui, hub, mobile, ui-kit

## Auditor Candidate #8696

- Finding Key: group.hub_registry_repo_split_recommended
- Title: Bootstrap chummer-hub-registry
- Detail: Create a dedicated registry repo for artifact catalog, publication, moderation, installs, and runtime-bundle head ownership once the contract-plane preconditions are stable.
- Severity: medium
- Summary: Run-services already has a clean `Chummer.Run.Registry` seam and dedicated registry/publication contract families, so the next service extraction after contract canon is a dedicated `chummer-hub-registry` repo for immutable artifacts, publication, installs, reviews, and runtime-bundle heads.
