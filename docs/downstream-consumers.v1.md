# Downstream Consumer Mapping

This repo does not edit `chummer.run-services` or presentation. This document specifies the migration they must adopt; it is not evidence that either consumer has already migrated.

## Package Boundary

- Package: `Chummer.Hub.Registry.Contracts`
- Namespace: `Chummer.Hub.Registry.Contracts`
- Ownership: immutable registry DTOs and stable vocabulary only
- Non-ownership: registry persistence, HTTP handlers, hosted orchestration, Spider, relay, media rendering, and presentation-specific view models

## `chummer.run-services`

`chummer.run-services` is intended to consume the package as the hosted-service boundary for registry flows. Verify its exact PR head and restored references before claiming that boundary is active.

Required changes downstream:

- replace source-level registry DTO definitions or shared-project links with a package reference to `Chummer.Hub.Registry.Contracts`
- keep orchestration, approvals, relay, AI, Spider, and play API aggregation local to `run-services`
- treat registry catalog lifecycle, publication records, moderation records, install projections, and runtime-bundle head DTOs as imported contracts instead of `run-services`-owned types
- restrict `run-services` registry code to adapters, transport mapping, and composition around these contracts

Must not remain downstream:

- duplicated copies of registry records from the former `Chummer.Run.Registry` seam
- direct source ownership claims over publication/moderation DTO shape
- registry persistence semantics leaking into the contract package

## Presentation

Presentation is intended to consume the package only for registry-facing request/response contracts and read models that cross the repo boundary.

Required changes downstream:

- reference `Chummer.Hub.Registry.Contracts` instead of importing registry DTOs from `run-services` source assemblies
- use contract records for artifact metadata, install/compatibility projections, publication drafts, moderation queue items, and runtime-bundle head projections
- keep presentation-only composition, display formatting, and UX state in presentation-local types

Must not remain downstream:

- presentation-owned copies of hub registry DTOs
- source-level references that make presentation depend on `run-services` registry implementation ownership
- presentation-specific UI concerns added to the shared contract package

## Migration Rule

When a type belongs to the artifact catalog, publication workflow, moderation workflow, install history/projections, compatibility projections, or runtime-bundle heads, downstream repos consume `Chummer.Hub.Registry.Contracts`.

Orchestration, transport, and UX-specific shaping stay local to their owning consumers. Registry storage and canonical publication state belong in the target registry service, not in the contract package. Existing storage in `run-services` is transitional until an evidenced service cutover.

## Dependency order

1. Verify this registry contract source and locally pack the exact ID/version required by consumers (default package version here is `1.0.0`, target `net10.0`). Make the verified package available through the agreed local dependency flow; a source merge alone does not publish it.
2. Verify `chummer.run-services` PR1 against that package: remove the former `Chummer.Run.Registry` source seam, align namespaces and references, and build the affected consumers. Run this repo's full ownership gate with `CHUMMER_RUN_SERVICES_ROOT` pointing at that exact PR checkout. This packet does not contain that PR's source or package-reference version, so it cannot certify those changes.
3. Verify presentation and the other March splits against their canonical packages (`Chummer.Engine.Contracts`, `Chummer.Play.Contracts`, `Chummer.Run.Contracts`, `Chummer.Ui.Kit`, `Chummer.Media.Contracts`) in their respective packets. They are not supplied or built here, and this registry package does not replace them.
4. Treat metadata/publication handler, store, and read-model cutover as separate work after a registry service exists. Package-only DTO consumption is necessary but does not complete `C0`/`E2`.

These are dependency/verification constraints; existing enforced merge and authority checks still apply. Build and package locally under the current operator policy. No hosted build, provider call, public publication, or deployment is required by this document.
