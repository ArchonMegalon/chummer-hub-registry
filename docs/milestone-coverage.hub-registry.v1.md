# Hub Registry Milestone Coverage Model

Date: 2026-03-11
Scope: complete milestone coverage modeling for `chummer-hub-registry` so ETA and completion truth are explicit instead of partial.

## Coverage contract

- Milestone spine source: `.codex-design/repo/IMPLEMENTATION_SCOPE.md` (`H0` through `H8`).
- Program alignment: `.codex-design/product/PROGRAM_MILESTONES.yaml` (`C0`, `E2`).
- Boundary guardrails: no provider adapters, approval bridges, docs/help vendor execution, or render execution in this repo.

## Milestone registry (complete coverage)

| Milestone | Theme | Program tie-in | Status | ETA band | Completion truth | Evidence in repo |
| --- | --- | --- | --- | --- | --- | --- |
| `H0` | Contract canon | `C0` | done | achieved 2026-03 | `Chummer.Hub.Registry.Contracts` exists with verify harness and boundary-safe DTO families. | `Chummer.Hub.Registry.Contracts/*`, `Chummer.Hub.Registry.Contracts.Verify/*` |
| `H1` | Artifact domain | `C0` | in_progress | 2026-Q2 | Immutable artifact metadata ownership is modeled, but runtime write/persistence cutover from `run-services` is still queued. | `docs/milestone-mapping.metadata-publication-cutover.v1.md`, `docs/runnable-backlog.metadata-publication-cutover.v1.md` |
| `H2` | Publication drafts | `C0`, `E2` | in_progress | 2026-Q2 | Publication contracts exist, but publication-state service ownership and projections are not yet fully registry-operated. | `Chummer.Hub.Registry.Contracts/PublicationContracts.cs`, cutover docs above |
| `H3` | Install/compatibility engine | `C0`, `E2` | planned | 2026-Q2 to Q3 | DTO surface exists; package-only seam enforcement and implementation cutover are not complete. | `Chummer.Hub.Registry.Contracts/CompatibilityContracts.cs` |
| `H4` | Search/discovery/reviews | `E2` | planned | 2026-Q3 | Review/discovery scope is represented in contracts but registry-owned read-model execution remains to be materialized. | `Chummer.Hub.Registry.Contracts/ArtifactContracts.cs` |
| `H5` | Style/template publication | `E2` | planned | 2026-Q3 | Repo may reference promoted help/template/style/preview artifacts as registry truth; no execution ownership is modeled here. | boundary rules in `.codex-design/repo/IMPLEMENTATION_SCOPE.md` |
| `H6` | Federation/org channels | `E2` | planned | 2026-Q4 | No active implementation in this repo yet; reserved for governed org/channel publication and install policies. | this registry model (current file) |
| `H7` | Hardening | `E2` | planned | 2026-Q4 | Verify harness exists; broader boundary/regression gates still need expansion as downstream cutovers land. | `scripts/ai/verify.sh`, `Chummer.Hub.Registry.Contracts.Verify/Program.cs` |
| `H8` | Finished registry | `E2` | planned | 2027+ | End-to-end publication/install/review/discovery/compatibility truth is not yet complete across repos. | program/design mirror refs in `.codex-design/product/*` |

## Audit finding to milestone mapping

Mapped from 2026-03-11 auditor publications (`487877`-`487883`):

1. Metadata/publication still effectively owned in `run-services` -> `H1`, `H2`, `C0`
2. Install/review/compatibility/runtime-bundle seams not yet package-only registry boundary -> `H3`, `H4`, `C0`, `E2`
3. Moderation/publication projections need explicit registry-owned read models -> `H2`, `H4`, `E2`
4. Milestone coverage incomplete -> resolved by this complete `H0`-`H8` registry model

## Executable next queue slices by milestone

1. `H1/H2`: finish metadata/publication write + persistence authority cutover from `run-services` to registry service ownership and publish evidence.
2. `H3`: add install/compatibility/runtime-bundle-head package-only consumption gates and migration checklist for downstream repos.
3. `H2/H4`: define explicit registry-owned moderation/publication read-model projections and downstream consumption contract.
4. `H7`: extend verification checks so regression gates fail on source-owned registry DTO reintroduction outside `Chummer.Hub.Registry.Contracts`.

## Definition of done for milestone truth

Milestone truth is considered complete only when each `H*` row has:

1. concrete completion criteria
2. evidence path(s) in this repo and/or linked downstream queue artifact
3. explicit status and ETA band
4. boundary-safe ownership consistent with `.codex-design/repo/IMPLEMENTATION_SCOPE.md`
