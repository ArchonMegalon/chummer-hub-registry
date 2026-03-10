# Ownership Matrix

| Repo | Owns | Must Not Own |
| --- | --- | --- |
| core | engine runtime, reducer truth, explain canon, engine contracts | UI heads, hosted-service workflows, media rendering |
| presentation | workbench/browser/desktop UX, inspectors, builders | play shell, rules math, media job execution |
| play | mobile/play shell, offline ledger, local sync client | builder UX, rule evaluation, provider secrets |
| run-services | identity, relay, approvals, memory, AI orchestration, play APIs | duplicate engine semantics, registry persistence after split, media rendering after split |
| ui-kit | tokens, themes, shell chrome, accessibility primitives | DTOs, HTTP clients, storage, rules math |
| hub-registry | artifacts, publication, moderation, runtime bundle heads | AI routing, Spider, relay, media rendering |
| media-factory | `Chummer.Media.Contracts`, render jobs, asset manifests, previews, provider adapters, retention lifecycle | campaign truth, rules truth, approvals policy, public UI |
