using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace FindingDateTimeNow.Tests
{
	internal static class TestHelpers
	{
		internal static async Task<List<Diagnostic>> GetDiagnosticsAsync<TAnalyzer>(
			string code, TextSpan methodDeclarationSpan)
			where TAnalyzer : ISyntaxNodeAnalyzer<SyntaxKind>, new()
		{
			return await TestHelpers.GetDiagnosticsAsync<TAnalyzer>(code,
				TestHelpers.CreateDocument(code), methodDeclarationSpan);
		}

		internal static async Task<List<Diagnostic>> GetDiagnosticsAsync<TAnalyzer>(
			string code, Document document, TextSpan methodDeclarationSpan)
			where TAnalyzer : ISyntaxNodeAnalyzer<SyntaxKind>, new()
		{
			var root = await document.GetSyntaxRootAsync();
			var node = root.FindNode(methodDeclarationSpan);
			var model = await document.GetSemanticModelAsync();

			var diagnostics = new List<Diagnostic>();
			var adder = new Action<Diagnostic>(_ => diagnostics.Add(_));
			var analyzer = new TAnalyzer();
			analyzer.AnalyzeNode(node, model, adder, null,
				new CancellationToken(false));

			return diagnostics;
		}

		internal static Document CreateDocument(string code)
		{
			var projectName = "Test";
			var projectId = ProjectId.CreateNewId(projectName);

			var solution = new CustomWorkspace()
				 .CurrentSolution
				 .AddProject(projectId, projectName, projectName, LanguageNames.CSharp)
				 .AddMetadataReference(projectId,
					new MetadataFileReference(typeof(object).Assembly.Location, MetadataImageKind.Assembly))
				 .AddMetadataReference(projectId,
					new MetadataFileReference(typeof(Enumerable).Assembly.Location, MetadataImageKind.Assembly))
				 .AddMetadataReference(projectId,
					new MetadataFileReference(typeof(CSharpCompilation).Assembly.Location, MetadataImageKind.Assembly))
				 .AddMetadataReference(projectId,
					new MetadataFileReference(typeof(Compilation).Assembly.Location, MetadataImageKind.Assembly));

			var documentId = DocumentId.CreateNewId(projectId);
			solution = solution.AddDocument(documentId, "Test.cs", SourceText.From(code));

			return solution.GetProject(projectId).Documents.First();
		}
	}
}
