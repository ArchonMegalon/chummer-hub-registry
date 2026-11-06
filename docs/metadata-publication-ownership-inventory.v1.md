# Metadata/Publication Ownership Inventory (Planning Draft)

Date: 2026-03-11
Program milestone: `C0` (Hub registry extraction)
Scope: immutable artifact metadata and publication state flows that are still implemented inside `chummer.run-services`.

## Notes

- This planning draft is based on queue scope and mirrored design context, not an audit of a pinned `chummer.run-services` revision.
- The downstream source is not supplied in this packet. Rows below are assumed ownership buckets; they do not identify actual handler, persistence, or projection implementation paths.
- Before accepting this as the cutover source list, record the exact downstream commit and concrete owner paths for every row. Inventory completion and runtime ownership remain unverified.

## Proposed `run-services` implementation-owner buckets (unverified)

| Flow | Contract operations / DTOs | Remaining `run-services` implementation owner | Current write owner | Current persistence owner | Current read-model owner | Target post-cutover owner |
| --- | --- | --- | --- | --- | --- | --- |
| Artifact metadata create/upsert | `HubArtifactCreateRequest`, `HubArtifactRecord`, `HubArtifactMetadata` | Metadata command handling and validation lane in `run-services` | `chummer.run-services` | `chummer.run-services` | `chummer.run-services` | `chummer-hub-registry` |
| Artifact metadata lifecycle transitions | `HubArtifactStateChangeRequest`, `HubArtifactStateResponse`, `HubArtifactDeleteAttemptResponse` | Metadata lifecycle/state-transition lane in `run-services` | `chummer.run-services` | `chummer.run-services` | `chummer.run-services` | `chummer-hub-registry` |
| Artifact publication pointer updates from metadata flows | `ArtifactPublicationPointer`, `HubArtifactMetadata` publication fields (`PublisherId`, `PublishedAtUtc`) | Metadata-to-publication linkage lane in `run-services` | `chummer.run-services` | `chummer.run-services` | `chummer.run-services` | `chummer-hub-registry` |
| Publication draft create/update/archive/delete | `HubPublicationOperations` (`create-draft`, `update-draft`, `archive-draft`, `delete-draft`), `HubPublishDraftRequest`, `HubUpdateDraftRequest`, `HubDraftRecord` | Draft write lane in `run-services` | `chummer.run-services` | `chummer.run-services` | `chummer.run-services` | `chummer-hub-registry` |
| Publication submission state transition | `HubSubmitProjectRequest`, `HubProjectSubmissionReceipt`, `HubDraftRecord` (`State`, `SubmittedAtUtc`) | Submission transition lane in `run-services` | `chummer.run-services` | `chummer.run-services` | `chummer.run-services` | `chummer-hub-registry` |
| Publication approval/rejection state transition | `HubModerationDecisionRequest`, `HubModerationDecisionReceipt`, `HubPublicationReceipt` | Moderation-to-publication decision lane in `run-services` | `chummer.run-services` | `chummer.run-services` | `chummer.run-services` | `chummer-hub-registry` |
| Publication state read models | `HubPublishDraftReceipt`, `HubPublishDraftList`, `HubDraftDetailProjection`, `HubPublicationReceipt` | Publication projection builder lane in `run-services` | `chummer.run-services` | `chummer.run-services` | `chummer.run-services` | `chummer-hub-registry` |

## Cutover implications captured by this inventory

- The model assumes `chummer.run-services` currently implements these flows; source review must confirm each row and identify any additional flows.
- The required cutover direction is the same for each flow: move write ownership, persistence authority, and read-model ownership into `chummer-hub-registry`.
- After cutover, `run-services` remains a contract consumer (`Chummer.Hub.Registry.Contracts`) and orchestration adapter only.
