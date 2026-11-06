# Chummer.Hub.Registry.Contracts

Immutable contract package for Hub registry artifacts, publication and moderation workflow, install projections, compatibility projections, and runtime-bundle head projections.

Consumers such as `chummer.run-services` and presentation depend on this package boundary for shared registry DTOs. Orchestration and UX-specific shaping stay local to those consumers. Registry write/persistence authority is planned for the registry service; it is not implemented by this DTO package or established by importing it.

The package targets `net10.0` and has no provider, storage, or compiler dependency. Records and vocabulary describe data shapes; collections are not made deeply immutable, and no runtime validation, persistence, provenance receipts, or service cutover is supplied.
