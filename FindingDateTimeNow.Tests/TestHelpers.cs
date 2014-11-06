using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FindingDateTimeNow.Tests
{
	internal static class TestHelpers
	{
		internal static async Task<List<Diagnostic>> GetDiagnosticsAsync<TAnalyzer>(
			string code, TextSpan methodDeclarationSpan)
			where TAnalyzer : DiagnosticAnalyzer, new()
		{
			return await TestHelpers.GetDiagnosticsAsync<TAnalyzer>(
				TestHelpers.Create(code), methodDeclarationSpan);
		}

		internal static async Task<List<Diagnostic>> GetDiagnosticsAsync<TAnalyzer>(
			Document document, TextSpan methodDeclarationSpan)
			where TAnalyzer : DiagnosticAnalyzer, new()
		{
			var root = await document.GetSyntaxRootAsync();
			var node = root.FindNode(methodDeclarationSpan);

			var compilation = await document.Project.GetCompilationAsync();
			var analyzer = new TAnalyzer();

			var driver = AnalyzerDriver.Create(compilation, ImmutableArray.Create(analyzer as DiagnosticAnalyzer),
				null, out compilation, CancellationToken.None);
			compilation.GetDiagnostics();
			return (await driver.GetDiagnosticsAsync()).ToList();
		}

		internal static Document Create(string code)
		{
			var projectName = "Test";
			var projectId = ProjectId.CreateNewId(projectName);

			var solution = new CustomWorkspace()
				 .CurrentSolution
				 .AddProject(projectId, projectName, projectName, LanguageNames.CSharp)
				 .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
				 .AddMetadataReference(projectId, MetadataReference.CreateFromAssembly(typeof(object).Assembly))
				 .AddMetadataReference(projectId, MetadataReference.CreateFromAssembly(typeof(Enumerable).Assembly))
				 .AddMetadataReference(projectId, MetadataReference.CreateFromAssembly(typeof(CSharpCompilation).Assembly))
				 .AddMetadataReference(projectId, MetadataReference.CreateFromAssembly(typeof(Compilation).Assembly));

			var documentId = DocumentId.CreateNewId(projectId);
			solution = solution.AddDocument(documentId, "Test.cs", SourceText.From(code));

			return solution.GetProject(projectId).Documents.First();
		}
	}
}
