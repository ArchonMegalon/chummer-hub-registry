internal static class RegistryOwnershipGateChecks
{
    internal static void Run()
    {
        foreach (string declaration in new[]
        {
            "public sealed record HubArtifactRecord();",
            "public record class HubArtifactRecord;",
            "public record /* comment */ class HubArtifactRecord;",
            "public readonly record struct HubArtifactRecord;",
            "public partial class @HubArtifactRecord {}",
            "public class HubArtifactRec\\u006frd {}",
            "public struct HubArtifactRecord {}",
            "public interface HubArtifactRecord {}",
            "public enum HubArtifactRecord {}",
            "public delegate void HubArtifactRecord();",
            "#if OTHER_BUILD\npublic record HubArtifactRecord;\n#endif",
            "#if false\npublic record HubArtifactRecord;\n#endif",
            "#if A && B && !C\npublic record\n#endif\nHubArtifactRecord();",
            "public class Container { public record HubArtifactRecord; }"
        })
        {
            Check(RegistryOwnershipGate.FindOwnedDeclarations(declaration).SequenceEqual(["HubArtifactRecord"]),
                $"Ownership gate missed declaration: {declaration}");
        }

        ExpectFailure(() => RegistryOwnershipGate.FindOwnedDeclarations(
            "#if A || B || C || D || E || F || G || H || I\nclass Consumer {}\n#endif"), "more than 8 conditional symbols");

        foreach (string name in new[]
        {
            "HubArtifactKind", "HubArtifactManifest", "HubPublicationStates", "HubModerationStates",
            "HubModerationQueue", "HubModerationCaseRecord", "HubPublicationNotImplementedReceipt",
            "HubPublicationResult<T>", "ArtifactInstallState", "RuntimeBundleHeadProjection"
        })
        {
            Check(RegistryOwnershipGate.FindOwnedDeclarations($"public record {name};").Length == 1,
                $"Ownership gate omitted canonical type: {name}");
        }

        Check(RegistryOwnershipGate.FindOwnedDeclarations(""""
            // public record HubArtifactRecord;
            /* public record class HubArtifactRecord; */
            using Chummer.Hub.Registry.Contracts;
            using Record = Chummer.Hub.Registry.Contracts.HubArtifactRecord;
            public class Consumer
            {
                string text = "public record HubArtifactRecord;";
                string verbatim = @"public class HubArtifactRecord {}";
                string raw = """public record HubArtifactRecord;""";
                string interpolated = $"class HubArtifactRecord {1}";
                HubArtifactRecord? value;
            }
            """").Length == 0, "Ownership gate must allow package usage, comments, and string literals.");

        string fixture = Path.Combine(Path.GetTempPath(), $"chummer-registry-gate-{Guid.NewGuid():N}");
        Directory.CreateDirectory(fixture);
        try
        {
            ExpectFailure(() => RegistryOwnershipGate.Verify(null), "CHUMMER_RUN_SERVICES_ROOT");
            ExpectFailure(() => RegistryOwnershipGate.Verify(Path.Combine(fixture, "missing")), "CHUMMER_RUN_SERVICES_ROOT");
            ExpectFailure(() => RegistryOwnershipGate.Verify(fixture), "empty scan cannot pass");
            string project = Path.Combine(fixture, "Consumer.csproj");
            string source = Path.Combine(fixture, "Consumer.cs");
            File.WriteAllText(project, "<Project />");
            File.WriteAllText(source, "using Chummer.Hub.Registry.Contracts; class Consumer { HubArtifactRecord? value; }");
            ExpectFailure(() => RegistryOwnershipGate.Verify(fixture), "No consumer project declares a PackageReference");
            File.WriteAllText(project, """
                <Project><ItemGroup>
                  <PackageReference Include="Chummer.Hub.Registry.Contracts" Version="1.0.0" />
                </ItemGroup></Project>
                """);
            RegistryOwnershipGate.Verify(fixture);
            foreach (string ignored in new[] { "bin", "obj", ".git" })
            {
                string ignoredDirectory = Path.Combine(fixture, ignored);
                Directory.CreateDirectory(ignoredDirectory);
                File.WriteAllText(Path.Combine(ignoredDirectory, "Generated.cs"), "public record HubArtifactRecord;");
            }
            RegistryOwnershipGate.Verify(fixture);
            File.WriteAllText(source, "public record class HubArtifactRecord;");
            ExpectFailure(() => RegistryOwnershipGate.Verify(fixture), "Consumer.cs: source-owns HubArtifactRecord");
            File.WriteAllText(source, "class Consumer {}");
            File.WriteAllText(project, """
                <Project><ItemGroup>
                  <PackageReference Include="Chummer.Hub.Registry.Contracts" Version="1.0.0" />
                  <ProjectReference Include="../Chummer.Hub.Registry.Contracts/Chummer.Hub.Registry.Contracts.csproj" />
                </ItemGroup></Project>
                """);
            ExpectFailure(() => RegistryOwnershipGate.Verify(fixture), "Consumer.csproj: source reference to registry contracts");
        }
        finally
        {
            Directory.Delete(fixture, recursive: true);
        }

        Console.WriteLine("Registry ownership gate regression checks passed.");
    }

    private static void ExpectFailure(Action action, string expectedMessage)
    {
        try
        {
            action();
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains(expectedMessage, StringComparison.Ordinal))
        {
            return;
        }

        throw new InvalidOperationException($"Ownership gate falsely passed; expected failure containing: {expectedMessage}.");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
