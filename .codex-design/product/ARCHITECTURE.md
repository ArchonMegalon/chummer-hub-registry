# Architecture

## Canonical rule
- Central design lives here.
- Repo-local mirrors are synced into code repos for workers and GitHub review.
- Cross-repo DTO ownership must be explicit and package-based.

## Split order
1. `Chummer.Engine.Contracts`
2. `Chummer.Play.Contracts`
3. finish `chummer-play`
4. `chummer-ui-kit`
5. `chummer-hub-registry`
6. `chummer-media-factory`
7. shrink `chummer.run-services`
8. purify `chummer-core-engine`
