# chummer-hub-registry

Dedicated contract boundary for the Hub registry split.

This repo currently seeds `Chummer.Hub.Registry.Contracts`, a dependency-light .NET package for:

- immutable artifact metadata and lifecycle state
- publication draft and moderation workflow contracts
- install state, install-history records, and compatibility projections
- runtime-bundle issuance and head projections

This boundary explicitly excludes:

- AI gateway routing logic
- Spider routing orchestration
- session relay logic
- media rendering or generation services

## Projects

- `Chummer.Hub.Registry.Contracts`: shared immutable records and stable vocabulary.
- `Chummer.Hub.Registry.Contracts.Verify`: no-network verification harness that asserts the extracted surface compiles and preserves key shape guarantees.

## Verification

Run `scripts/ai/verify.sh`.
