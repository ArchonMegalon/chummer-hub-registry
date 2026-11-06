using System.Xml.Linq;
using Chummer.Hub.Registry.Contracts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class RegistryOwnershipGate
{
    private const string PackageId = "Chummer.Hub.Registry.Contracts";
    private static readonly HashSet<string> ContractNames = typeof(HubArtifactRecord).Assembly
        .GetExportedTypes()
        .Where(type => type.Namespace == typeof(HubArtifactRecord).Namespace)
        .Select(type => type.Name.Split('`')[0])
        .ToHashSet(StringComparer.Ordinal);

    internal static void Verify(string? consumerRoot)
    {
        if (string.IsNullOrWhiteSpace(consumerRoot) || !Directory.Exists(consumerRoot))
        {
            throw new InvalidOperationException(
                "Set CHUMMER_RUN_SERVICES_ROOT to the consumer checkout. " +
                "Missing/invalid input cannot pass ownership verification; use --contracts-only for an explicit local-only check.");
        }

        string root = Path.GetFullPath(consumerRoot);
        string[] files = EnumerateFiles(root).Order(StringComparer.Ordinal).ToArray();
        string[] sources = files.Where(path => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)).ToArray();
        string[] projects = files.Where(path => path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)).ToArray();
        if (sources.Length == 0 || projects.Length == 0)
        {
            throw new InvalidOperationException("Consumer checkout must contain C# source and a C# project; an empty scan cannot pass.");
        }

        List<string> violations = [];
        foreach (string source in sources)
        {
            string[] owned;
            try
            {
                owned = FindOwnedDeclarations(File.ReadAllText(source));
            }
            catch (InvalidOperationException exception)
            {
                throw new InvalidOperationException($"{Path.GetRelativePath(root, source)}: {exception.Message}", exception);
            }

            foreach (string name in owned)
            {
                violations.Add($"{Path.GetRelativePath(root, source)}: source-owns {name}");
            }
        }

        bool hasPackageReference = false;
        foreach (string project in projects)
        {
            XDocument document = XDocument.Load(project);
            foreach (XElement item in document.Descendants())
            {
                string include = (string?)item.Attribute("Include") ?? string.Empty;
                if (item.Name.LocalName == "PackageReference" && include.Equals(PackageId, StringComparison.OrdinalIgnoreCase))
                {
                    hasPackageReference = true;
                }

                if ((item.Name.LocalName is "ProjectReference" or "Compile") &&
                    (include.Contains(PackageId, StringComparison.OrdinalIgnoreCase) ||
                     include.Contains("Chummer.Run.Registry", StringComparison.OrdinalIgnoreCase)))
                {
                    violations.Add($"{Path.GetRelativePath(root, project)}: source reference to registry contracts ({include})");
                }
            }
        }

        if (!hasPackageReference)
        {
            violations.Add($"No consumer project declares a PackageReference to {PackageId}.");
        }

        if (violations.Count > 0)
        {
            throw new InvalidOperationException("Registry consumer boundary violations:\n" + string.Join("\n", violations));
        }

        Console.WriteLine($"Registry ownership checked {sources.Length} source files and {projects.Length} projects under {root}.");
    }

    internal static string[] FindOwnedDeclarations(string source)
    {
        SyntaxNode syntax = CSharpSyntaxTree.ParseText(source).GetRoot();
        string[] symbols = syntax.DescendantTrivia()
            .Select(trivia => trivia.GetStructure())
            .Select(directive => directive switch
            {
                IfDirectiveTriviaSyntax condition => condition.Condition,
                ElifDirectiveTriviaSyntax condition => condition.Condition,
                _ => null
            })
            .OfType<ExpressionSyntax>()
            .SelectMany(condition => condition.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>())
            .Select(identifier => identifier.Identifier.ValueText)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        // Bound the analysis, but never report a pass for configurations we did not inspect.
        if (symbols.Length > 8)
        {
            throw new InvalidOperationException("Consumer source has more than 8 conditional symbols; explicit configuration review is required.");
        }

        HashSet<string> names = FindDeclarations(syntax).ToHashSet(StringComparer.Ordinal);
        for (int configuration = 1; configuration < (1 << symbols.Length); configuration++)
        {
            IEnumerable<string> defined = symbols.Where((_, index) => (configuration & (1 << index)) != 0);
            SyntaxNode configured = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(preprocessorSymbols: defined)).GetRoot();
            names.UnionWith(FindDeclarations(configured));
        }

        // Whole-file configurations also catch declarations split across #if directives.
        // Parse disabled fragments to reject complete copies even under #if false.
        foreach (SyntaxTrivia trivia in syntax.DescendantTrivia().Where(trivia => trivia.IsKind(SyntaxKind.DisabledTextTrivia)))
        {
            names.UnionWith(FindOwnedDeclarations(trivia.ToFullString()));
        }

        return names.Order(StringComparer.Ordinal).ToArray();
    }

    private static IEnumerable<string> FindDeclarations(SyntaxNode syntax)
        => syntax.DescendantNodes().Select(node => node switch
        {
            BaseTypeDeclarationSyntax type => type.Identifier.ValueText,
            DelegateDeclarationSyntax type => type.Identifier.ValueText,
            _ => string.Empty
        }).Where(ContractNames.Contains);

    private static IEnumerable<string> EnumerateFiles(string directory)
    {
        foreach (string path in Directory.EnumerateFileSystemEntries(directory))
        {
            string name = Path.GetFileName(path);
            if (name is ".git" or "bin" or "obj")
            {
                continue;
            }

            FileAttributes attributes = File.GetAttributes(path);
            if ((attributes & FileAttributes.ReparsePoint) != 0)
            {
                throw new InvalidOperationException($"Consumer scan cannot verify a linked path: {path}");
            }

            if ((attributes & FileAttributes.Directory) != 0)
            {
                foreach (string file in EnumerateFiles(path))
                {
                    yield return file;
                }
            }
            else
            {
                yield return path;
            }
        }
    }
}
