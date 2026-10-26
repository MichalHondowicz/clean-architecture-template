using System.Text.Json;
using HookTests.Infrastructure;
using Shouldly;

namespace HookTests;

public sealed class CheckDiffTests
{
    private const string Script = "check-diff.ps1";

    [Fact]
    public async Task CheckDiff_Should_ReportProtectedPath_WhenRangeChangesIt()
    {
        // Arrange
        using var repository = DodRepository.Create([], protectedPaths: [".editorconfig"]);
        repository.WriteFile(".editorconfig", "root = true\n");
        string baseCommit = repository.CommitAll("base");
        repository.WriteFile(".editorconfig", "root = true\nindent_size = 2\n");
        repository.CommitAll("ticket");

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, repository.Root, "-Base", baseCommit);

        // Assert
        result.ExitCode.ShouldBe(1);
        Violations(result).ShouldContain(("protected-path", ".editorconfig"));
    }

    [Fact]
    public async Task CheckDiff_Should_JudgeByTheBaseDod_WhenRangeAlsoEmptiesTheProtectedList()
    {
        // Arrange
        using var repository = DodRepository.Create([], protectedPaths: ["AGENTS.md", "docs/agents/**"]);
        repository.WriteFile("AGENTS.md", "# Rules\n");
        string baseCommit = repository.CommitAll("base");
        repository.WriteFile("docs/agents/dod.json", DodRepository.DodJson([], protectedPaths: []));
        repository.WriteFile("AGENTS.md", "# Rules\nAnything goes.\n");
        repository.CommitAll("ticket");

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, repository.Root, "-Base", baseCommit);

        // Assert
        result.ExitCode.ShouldBe(1);
        Violations(result).ShouldContain(("protected-path", "AGENTS.md"));
        Violations(result).ShouldContain(("protected-path", "docs/agents/dod.json"));
    }

    [Theory]
    [InlineData(".scratch/archive-todo/issues/01-archive.md", "Never add `#pragma warning disable` or `<NoWarn>`.")]
    [InlineData("docs/notes.txt", "SuppressMessage is forbidden")]
    public async Task CheckDiff_Should_IgnoreSuppressionText_WhenFileIsNotCodeOrBuildConfig(string relativePath, string addedLine)
    {
        // Arrange
        using var repository = DodRepository.Create([]);
        string baseCommit = repository.CommitAll("base");
        repository.WriteFile(relativePath, $"{addedLine}\n");
        repository.CommitAll("ticket");

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, repository.Root, "-Base", baseCommit);

        // Assert
        result.ExitCode.ShouldBe(0);
        Violations(result).ShouldBeEmpty();
    }

    [Theory]
    [InlineData("src/Application/Feature.cs", "#pragma warning disable CA1822")]
    [InlineData("src/Application/Feature.cs", "[SuppressMessage(\"Design\", \"CA1000\")]")]
    [InlineData("src/Application/Application.csproj", "<NoWarn>CA1000</NoWarn>")]
    [InlineData("src/Application/Application.csproj", "<WarningsNotAsErrors>CA1000</WarningsNotAsErrors>")]
    [InlineData("src/Application/Application.csproj", "<TreatWarningsAsErrors>false</TreatWarningsAsErrors>")]
    [InlineData("src/Application/Application.csproj", "<CodeAnalysisTreatWarningsAsErrors>false</CodeAnalysisTreatWarningsAsErrors>")]
    [InlineData("src/Application/Application.csproj", "<RunAnalyzers>false</RunAnalyzers>")]
    [InlineData("src/Application/Application.csproj", "<EnableNETAnalyzers>false</EnableNETAnalyzers>")]
    [InlineData("src/Application/Application.csproj", "<AnalysisMode>None</AnalysisMode>")]
    [InlineData("src/Application/Application.csproj", "<AnalysisLevel>none</AnalysisLevel>")]
    [InlineData("src/.editorconfig", "dotnet_diagnostic.CA1822.severity = none")]
    [InlineData("src/Application/Application.csproj", "<NoWarn Condition=\"'$(Configuration)' == 'Release'\">CA1000</NoWarn>")]
    [InlineData("src/Application/Application.csproj", "<WarningsNotAsErrors Condition=\"true\">CA1000</WarningsNotAsErrors>")]
    [InlineData("src/Application/Application.csproj", "<TreatWarningsAsErrors Condition=\"true\">false</TreatWarningsAsErrors>")]
    [InlineData("src/Application/My Feature.cs", "#pragma warning disable CA1822")]
    public async Task CheckDiff_Should_ReportSuppression_WhenRangeAddsOne(string relativePath, string addedLine)
    {
        // Arrange
        using var repository = DodRepository.Create([]);
        repository.WriteFile(relativePath, "first line\n");
        string baseCommit = repository.CommitAll("base");
        repository.WriteFile(relativePath, $"first line\n{addedLine}\n");
        repository.CommitAll("ticket");

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, repository.Root, "-Base", baseCommit);

        // Assert
        result.ExitCode.ShouldBe(1);
        Violations(result).ShouldContain(("suppression", relativePath));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CheckDiff_Should_IgnoreSuppression_WhenFileIsToolGenerated(bool withBom)
    {
        // Arrange
        using var repository = DodRepository.Create([]);
        string baseCommit = repository.CommitAll("base");
        repository.WriteFile(
            "src/Infrastructure/Database/Migrations/20261026120000_Add_Archive.Designer.cs",
            "// <auto-generated />\nusing System;\n#pragma warning disable 612, 618\n",
            withBom);
        repository.CommitAll("ticket");

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, repository.Root, "-Base", baseCommit);

        // Assert
        result.ExitCode.ShouldBe(0);
        Violations(result).ShouldBeEmpty();
    }

    [Fact]
    public async Task CheckDiff_Should_IgnoreExistingSuppression_WhenFileChangesElsewhere()
    {
        // Arrange
        using var repository = DodRepository.Create([]);
        repository.WriteFile("src/Infrastructure/DependencyInjection.cs", "#pragma warning disable EXTEXP0018\nclass A { }\n");
        string baseCommit = repository.CommitAll("base");
        repository.WriteFile("src/Infrastructure/DependencyInjection.cs", "#pragma warning disable EXTEXP0018\nclass A { int B; }\n");
        repository.CommitAll("ticket");

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, repository.Root, "-Base", baseCommit);

        // Assert
        result.ExitCode.ShouldBe(0);
        Violations(result).ShouldBeEmpty();
    }

    [Fact]
    public async Task CheckDiff_Should_ListChangedExistingTestsForReview_WhenRangeTouchesThem()
    {
        // Arrange
        using var repository = DodRepository.Create([]);
        repository.WriteFile("tests/IntegrationTests/Todos/CreateTodoTests.cs", "class CreateTodoTests { }\n");
        repository.WriteFile("tests/IntegrationTests/Todos/DeleteTodoTests.cs", "class DeleteTodoTests { }\n");
        string baseCommit = repository.CommitAll("base");
        repository.WriteFile("tests/IntegrationTests/Todos/CreateTodoTests.cs", "class CreateTodoTests { int Weakened; }\n");
        File.Delete(repository.PathOf("tests/IntegrationTests/Todos/DeleteTodoTests.cs"));
        repository.WriteFile("tests/IntegrationTests/Todos/ArchiveTodoTests.cs", "class ArchiveTodoTests { }\n");
        repository.CommitAll("ticket");

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, repository.Root, "-Base", baseCommit);

        // Assert
        result.ExitCode.ShouldBe(0);
        using var output = JsonDocument.Parse(result.Stdout);
        string[] testsToReview = [.. output.RootElement.GetProperty("testsToReview").EnumerateArray().Select(t => t.GetString()!)];
        testsToReview.ShouldBe(
            ["tests/IntegrationTests/Todos/CreateTodoTests.cs", "tests/IntegrationTests/Todos/DeleteTodoTests.cs"],
            ignoreOrder: true);
    }

    [Fact]
    public async Task CheckDiff_Should_ReportRemovedSolutionProject_WhenRangeDropsOneFromTheSolution()
    {
        // Arrange
        using var repository = DodRepository.Create([]);
        repository.WriteFile(
            "CleanArchitecture.slnx",
            "<Solution>\n  <Project Path=\"src/Domain/Domain.csproj\" />\n  <Project Path=\"tests/IntegrationTests/IntegrationTests.csproj\" />\n</Solution>\n");
        string baseCommit = repository.CommitAll("base");
        repository.WriteFile(
            "CleanArchitecture.slnx",
            "<Solution>\n  <Project Path=\"src/Domain/Domain.csproj\" />\n  <Project Path=\"src/Mobile.Core/Mobile.Core.csproj\" />\n</Solution>\n");
        repository.CommitAll("ticket");

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, repository.Root, "-Base", baseCommit);

        // Assert
        result.ExitCode.ShouldBe(1);
        List<(string Kind, string Path)> violations = Violations(result);
        violations.ShouldContain(("solution-project-removed", "CleanArchitecture.slnx"));
        violations.Count.ShouldBe(1);
    }

    private static List<(string Kind, string Path)> Violations(HookResult result)
    {
        using var output = JsonDocument.Parse(result.Stdout);
        return [.. output.RootElement.GetProperty("violations").EnumerateArray()
            .Select(v => (v.GetProperty("kind").GetString()!, v.GetProperty("path").GetString()!))];
    }
}
