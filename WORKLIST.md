# Worklist

- [done] Bootstrap repo structure and package boundaries
- [done] Extract `Chummer.Hub.Registry.Contracts` for artifact metadata, publication workflow, moderation, installs, compatibility projections, and runtime-bundle heads
- [done] Inventory the extracted registry DTO surface; legacy `Chummer.Run.Registry` consumer removal still requires downstream evidence
- [done] Add downstream consumer migration mapping for `run-services` and presentation to consume package-owned registry contracts instead of source-level ownership
- [done] Add explicit milestone mapping for remaining `run-services` -> `hub-registry` ownership transfer of immutable artifact metadata and publication state
- [done] Publish executable cutover queue entries for metadata/publication ownership transfer sequencing and verification
- [done] Document the H0-H8 milestone spine as a March planning snapshot; this is not current ETA or runtime completion evidence
- [done] Add milestone mapping for install/review/compatibility/runtime-bundle-head seams that are not yet package-only registry boundaries
- [done] Publish runnable backlog for install/review/compatibility/runtime-bundle-head package-boundary cutover and verify-gate expansion
- [done] Add milestone mapping for moderation/publication projections that still need explicit registry-owned read models
- [done] Publish runnable backlog for moderation/publication projection read-model ownership and verify-gate expansion
- [planned] Complete the metadata/publication ownership inventory with a pinned downstream revision and exact handler/store/projection paths; the current draft only lists assumed owner buckets
- [done] Publish a cutover checklist that moves metadata/publication write ownership and persistence authority to `chummer-hub-registry` while keeping `run-services` as contract consumer only
- [implemented; verification pending] Add fail-closed source-ownership checks and regression fixtures for canonical registry types, plus direct package-reference checks; execute the C# harness and pinned downstream build before declaring this gate verified
- [queued] Verify package ID/version/feed alignment and the full ownership gate against `chummer.run-services` PR1, then verify presentation separately
- [queued] Implement and evidence registry-owned handlers, persistence, and read models; DTO packaging alone does not complete runtime ownership transfer
