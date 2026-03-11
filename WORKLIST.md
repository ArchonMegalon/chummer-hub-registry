# Worklist

- [done] Bootstrap repo structure and package boundaries
- [done] Extract `Chummer.Hub.Registry.Contracts` for artifact metadata, publication workflow, moderation, installs, compatibility projections, and runtime-bundle heads
- [done] Move the `Chummer.Run.Registry` seam contract surface into `chummer-hub-registry`
- [done] Add downstream consumer migration mapping for `run-services` and presentation to consume package-owned registry contracts instead of source-level ownership
- [todo] Add explicit milestone mapping for remaining `run-services` -> `hub-registry` ownership transfer of immutable artifact metadata and publication state
- [todo] Publish executable cutover queue entries for metadata/publication ownership transfer sequencing and verification
