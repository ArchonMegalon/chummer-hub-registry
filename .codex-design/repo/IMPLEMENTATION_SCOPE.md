
# Hub Registry Implementation Scope

`chummer-hub-registry` owns immutable artifact catalog, publication workflow, moderation state, installs, reviews, and runtime-bundle head metadata.

Must not own:
- AI gateway routing
- Spider/session relay
- media rendering
- play/client implementation

Current focus:
- extract registry contracts and catalog lifecycle out of `chummer.run-services`
