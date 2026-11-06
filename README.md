# chummer-hub-registry

Dedicated contract boundary for the Hub registry split.

This March split repository is a contract seed, with no registry service, persistence implementation, or deployment. It does not replace the current `chummer6-core`/Hub/UI runtime. The mirrored product design describes target ownership, not a completed runtime cutover.

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

## Downstream Consumption

`chummer.run-services` and presentation are expected to consume registry DTOs through the `Chummer.Hub.Registry.Contracts` package boundary rather than through source-level registry ownership.

The consumer migration map for that split is tracked in [docs/downstream-consumers.v1.md](docs/downstream-consumers.v1.md).

## Verification

With the .NET 10 SDK installed, run against the exact downstream checkout being reviewed:

```bash
CHUMMER_RUN_SERVICES_ROOT=/absolute/path/to/chummer.run-services scripts/ai/verify.sh
```

The default check requires an existing consumer checkout with C# source and projects. It scans declarations for every public type exported by the contract assembly, including generic wrappers, vocabulary, and inactive `#if` branches. Comments and string literals are not declarations. It also requires a direct canonical `PackageReference` in a consumer project and rejects registry `ProjectReference`/named source links. Missing/invalid roots, empty scans, and unreadable or linked paths fail rather than silently skipping. `.git`, `bin`, and `obj` are excluded.

Conditional declarations are parsed across symbol combinations, including declarations split by directives. Analysis is bounded to eight conditional symbols per file; more symbols require explicit configuration review and fail the gate instead of yielding an incomplete pass.

For an isolated contract check, run `scripts/ai/verify.sh --contracts-only`. Its output explicitly says consumer ownership was **not** checked. Both modes run the ownership detector's regression fixtures. The verifier uses Roslyn assemblies shipped with the SDK; the contract package gains no compiler dependency.

The script builds this checkout's verifier and contracts, runs verification without rebuilding, then packs the same Debug outputs. Packages go to a fresh temporary directory printed on success, or to `CHUMMER_REGISTRY_PACK_OUTPUT` when set. The package ID/assembly/namespace is `Chummer.Hub.Registry.Contracts`, targeting `net10.0`; the SDK default package version is `1.0.0`. Align the version/feed with downstream references before accepting a consumer migration. Packing is local and does not publish a package.

This is a source boundary check, not an evaluated MSBuild dependency graph or a restored consumer build. It does not establish package/feed availability, linked sources hidden behind arbitrary imports, handler/persistence ownership, data migration, runtime provenance receipts, or service behavior. Those require downstream source review and focused builds/cutover evidence. It checks the supplied consumer only; presentation must be checked separately.

Without an SDK, `python3 scripts/ai/test_verify_script.py` checks shell routing and failure propagation using a stub `dotnet`. It does not execute C# or produce a package.
