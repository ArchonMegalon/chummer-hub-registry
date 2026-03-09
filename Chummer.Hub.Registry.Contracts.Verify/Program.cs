using System.Reflection;
using System.Text;
using Chummer.Hub.Registry.Contracts;

VerifySealedRecord(typeof(ArtifactInstallState));
VerifySealedRecord(typeof(ArtifactCompatibilityProjection));
VerifySealedRecord(typeof(HubArtifactMetadata));
VerifySealedRecord(typeof(HubPublishDraftReceipt));
VerifySealedRecord(typeof(HubModerationCaseRecord));
VerifySealedRecord(typeof(RuntimeBundleHeadProjection));

Assert(HubPublicationOperations.SubmitProject == "submit-project", "Publication operation constants must match the existing workflow vocabulary.");
Assert(HubModerationStates.PendingReview == "pending-review", "Moderation states must preserve pending-review.");
Assert(ArtifactInstallStates.Pinned == "pinned", "Install state constants must preserve pinned.");
Assert(ArtifactCompatibilityStates.CompatibleWithWarnings == "compatible-with-warnings", "Compatibility states must preserve warning vocabulary.");
Assert(Enum.GetNames<HubArtifactKind>().Contains(nameof(HubArtifactKind.RuntimeBundle)), "Artifact kinds must include RuntimeBundle.");
Assert(Enum.GetNames<RuntimeBundleHeadKind>().SequenceEqual(["Session", "Mobile", "Offline"]), "Runtime bundle heads must stay Session/Mobile/Offline.");

ArtifactInstallState install = new(
    State: ArtifactInstallStates.Pinned,
    InstalledAtUtc: DateTimeOffset.UnixEpoch,
    InstalledTargetKind: "workspace",
    InstalledTargetId: "workspace-1",
    RuntimeFingerprint: "sha256:runtime");

HubArtifactMetadata artifact = new(
    Id: "runtime-bundle-1",
    Name: "Seattle Session Bundle",
    Kind: HubArtifactKind.RuntimeBundle,
    Version: "2026.03.09-session",
    RulesetId: "sr5",
    State: HubArtifactState.Active,
    Visibility: ArtifactVisibilityModes.Shared,
    TrustTier: ArtifactTrustTiers.Curated,
    OwnerId: "alice",
    PublisherId: "pub.shadowops",
    Summary: "Immutable session bundle projection.",
    Description: "Retained for installs and replay.",
    RuntimeFingerprint: "sha256:session",
    StateReason: null,
    SupersededByArtifactId: null,
    ImmutableRetentionRequired: true,
    InstallCount: 2,
    ActiveRuntimeRefCount: 1,
    ReviewCount: 0,
    AverageReviewScore: 0,
    CreatedAtUtc: DateTimeOffset.UnixEpoch,
    UpdatedAtUtc: DateTimeOffset.UnixEpoch);

HubArtifactInstallProjection installProjection = new(
    ArtifactId: artifact.Id,
    Kind: artifact.Kind,
    Version: artifact.Version,
    RulesetId: artifact.RulesetId,
    State: artifact.State,
    SupersededByArtifactId: artifact.SupersededByArtifactId,
    ImmutableRetentionRequired: artifact.ImmutableRetentionRequired,
    AcceptingNewInstalls: true,
    InstallCount: artifact.InstallCount,
    ActiveRuntimeRefCount: artifact.ActiveRuntimeRefCount,
    HasInstallReferences: true,
    HasRuntimeReferences: true,
    LastInstalledAtUtc: DateTimeOffset.UnixEpoch,
    Install: install);

ArtifactCompatibilityProjection compatibilityProjection = new(
    ArtifactId: artifact.Id,
    Kind: artifact.Kind,
    Version: artifact.Version,
    RulesetId: artifact.RulesetId,
    InstallTargetKind: install.InstalledTargetKind,
    InstallTargetId: install.InstalledTargetId,
    RuntimeFingerprint: install.RuntimeFingerprint,
    RequiredRuntimeFingerprint: artifact.RuntimeFingerprint,
    CurrentEngineApiVersion: "engine-v5",
    RequiredEngineApiVersion: "engine-v5",
    CompatibilityState: ArtifactCompatibilityStates.Compatible,
    InstallAllowed: true,
    UpgradeAvailable: false,
    RequiresMigration: false,
    MissingDependencies: [],
    ConflictingArtifacts: [],
    Issues: []);

ArtifactInstallCompatibilityProjection installCompatibilityProjection = new(
    Install: installProjection,
    Compatibility: compatibilityProjection);

RuntimeBundleHeadProjection head = new(
    BundleFamilyId: "session:alpha/scene:redmond",
    SessionId: "alpha",
    SceneId: "redmond",
    Head: RuntimeBundleHeadKind.Session,
    CurrentArtifactId: artifact.Id,
    CurrentVersion: artifact.Version,
    SourceBundleVersion: "runtime-lock-7",
    ProjectionFingerprint: "sha256:projection",
    ProjectionVersion: 3,
    Ready: true,
    OfflineCapable: true,
    CollaborationMode: "gm-led",
    SupportedExchangeFormats: ["json", "zip"],
    IssuedAtUtc: DateTimeOffset.UnixEpoch,
    PreviousArtifactId: null);

HubPublicationResult<RuntimeBundleHeadProjection> implemented = HubPublicationResult<RuntimeBundleHeadProjection>.Implemented(head);
Assert(implemented.IsImplemented, "Implemented result wrappers must report IsImplemented.");
Assert(implemented.Payload == head, "Implemented result wrappers must preserve payloads.");
Assert(installCompatibilityProjection.Compatibility.InstallAllowed, "Install compatibility projections must preserve install decisions.");
Assert(
    installCompatibilityProjection.Compatibility.CompatibilityState == ArtifactCompatibilityStates.Compatible,
    "Install compatibility projections must preserve compatibility state.");

HubPublicationResult<RuntimeBundleHeadProjection> notImplemented = HubPublicationResult<RuntimeBundleHeadProjection>.FromNotImplemented(
    new HubPublicationNotImplementedReceipt("not-implemented", HubPublicationOperations.ListModerationQueue, "queued"));
Assert(!notImplemented.IsImplemented, "Fallback result wrappers must report not implemented.");

Console.WriteLine("Registry contract verification passed.");

static void VerifySealedRecord(Type type)
{
    Assert(type.IsSealed, $"{type.Name} must be sealed.");
    MethodInfo? printMembers = type.GetMethod(
        "PrintMembers",
        BindingFlags.Instance | BindingFlags.NonPublic,
        binder: null,
        [typeof(StringBuilder)],
        modifiers: null);
    Assert(printMembers is not null, $"{type.Name} must remain a record type.");
}

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
