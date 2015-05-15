using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace FindingDateTimeNow.Tests
{
	[TestClass]
	public sealed class FindingNewDateTimeAnalyzerTests
	{
		[TestMethod]
		public void VerifySupportedDiagnostics()
		{
			var analyzer = new FindingNewDateTimeAnalyzer();
			var diagnostics = analyzer.SupportedDiagnostics;
			Assert.AreEqual(2, diagnostics.Length);

			var newDiagnostic = diagnostics[0];
			Assert.AreEqual(newDiagnostic.Id, FindingNewDateTimeConstants.DiagnosticId, 
				nameof(DiagnosticDescriptor.Id));
			Assert.AreEqual(newDiagnostic.Title.ToString(), FindingNewDateTimeConstants.Title, 
				nameof(DiagnosticDescriptor.Title));
			Assert.AreEqual(newDiagnostic.MessageFormat.ToString(), FindingNewDateTimeConstants.FindingDateTimeNowMessage, 
				nameof(DiagnosticDescriptor.Title));
			Assert.AreEqual(newDiagnostic.Category, FindingNewDateTimeConstants.Category, 
				nameof(DiagnosticDescriptor.Category));
			Assert.AreEqual(newDiagnostic.DefaultSeverity, DiagnosticSeverity.Error, 
				nameof(DiagnosticDescriptor.DefaultSeverity));

			var unspecifiedDiagnostic = diagnostics[1];
			Assert.AreEqual(unspecifiedDiagnostic.Id, FindingNewDateTimeConstants.UnspecifiedDiagnosticId, 
				nameof(DiagnosticDescriptor.Id));
			Assert.AreEqual(unspecifiedDiagnostic.Title.ToString(), FindingNewDateTimeConstants.Title, 
				nameof(DiagnosticDescriptor.Title));
			Assert.AreEqual(unspecifiedDiagnostic.MessageFormat.ToString(), FindingNewDateTimeConstants.UnspecifiedKindMessage, 
				nameof(DiagnosticDescriptor.MessageFormat));
			Assert.AreEqual(unspecifiedDiagnostic.Category, FindingNewDateTimeConstants.Category, 
				nameof(DiagnosticDescriptor.Category));
			Assert.AreEqual(unspecifiedDiagnostic.DefaultSeverity, DiagnosticSeverity.Error, 
				nameof(DiagnosticDescriptor.DefaultSeverity));
		}

		[TestMethod]
		public async Task AnalyzeWhenUsingLocalDateTimeKind()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(FindingNewDateTimeAnalyzerTests)}.{nameof(FindingNewDateTimeAnalyzerTests.AnalyzeWhenUsingLocalDateTimeKind)}.cs");

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(97, 8));
			Assert.AreEqual(1, diagnostics.Count);
			var diagnostic = diagnostics[0];
			Assert.AreEqual(FindingNewDateTimeConstants.FindingDateTimeNowMessage,
				diagnostic.GetMessage());
			Assert.AreEqual(126, diagnostic.Location.SourceSpan.Start);
			Assert.AreEqual(131, diagnostic.Location.SourceSpan.End);
		}

		[TestMethod]
		public async Task AnalyzeWhenUsingUnspecifiedDateTimeKind()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(FindingNewDateTimeAnalyzerTests)}.{nameof(FindingNewDateTimeAnalyzerTests.AnalyzeWhenUsingUnspecifiedDateTimeKind)}.cs");

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(97, 8));
			Assert.AreEqual(1, diagnostics.Count);
			var diagnostic = diagnostics[0];
			Assert.AreEqual(FindingNewDateTimeConstants.FindingDateTimeNowMessage,
				diagnostic.GetMessage());
			Assert.AreEqual(126, diagnostic.Location.SourceSpan.Start);
			Assert.AreEqual(137, diagnostic.Location.SourceSpan.End);
		}

		[TestMethod]
		public async Task AnalyzeWithNoSpecifiedKind()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(FindingNewDateTimeAnalyzerTests)}.{nameof(FindingNewDateTimeAnalyzerTests.AnalyzeWithNoSpecifiedKind)}.cs");

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(97, 8));
			Assert.AreEqual(1, diagnostics.Count);
			var diagnostic = diagnostics[0];
			Assert.AreEqual(FindingNewDateTimeConstants.UnspecifiedKindMessage,
				diagnostic.GetMessage());
			Assert.AreEqual(93, diagnostic.Location.SourceSpan.Start);
			Assert.AreEqual(112, diagnostic.Location.SourceSpan.End);
		}

		[TestMethod]
		public async Task AnalyzeWhenUsingUtcDateTimeKind()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(FindingNewDateTimeAnalyzerTests)}.{nameof(FindingNewDateTimeAnalyzerTests.AnalyzeWhenUsingUtcDateTimeKind)}.cs");

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(97, 8));
			Assert.AreEqual(0, diagnostics.Count);
		}

		[TestMethod]
		public async Task AnalyzeWhenUsingLocalDateTimeKindAndAlias()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(FindingNewDateTimeAnalyzerTests)}.{nameof(FindingNewDateTimeAnalyzerTests.AnalyzeWhenUsingLocalDateTimeKindAndAlias)}.cs");

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(126, 2));
			Assert.AreEqual(1, diagnostics.Count);
			var diagnostic = diagnostics[0];
			Assert.AreEqual(FindingNewDateTimeConstants.FindingDateTimeNowMessage,
				diagnostic.GetMessage());
			Assert.AreEqual(149, diagnostic.Location.SourceSpan.Start);
			Assert.AreEqual(154, diagnostic.Location.SourceSpan.End);
		}

		[TestMethod]
		public async Task AnalyzeWhenUsingUnspecifiedDateTimeKindAndAlias()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(FindingNewDateTimeAnalyzerTests)}.{nameof(FindingNewDateTimeAnalyzerTests.AnalyzeWhenUsingUnspecifiedDateTimeKindAndAlias)}.cs");

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(126, 2));
			Assert.AreEqual(1, diagnostics.Count);
			var diagnostic = diagnostics[0];
			Assert.AreEqual(FindingNewDateTimeConstants.FindingDateTimeNowMessage,
				diagnostic.GetMessage());
			Assert.AreEqual(149, diagnostic.Location.SourceSpan.Start);
			Assert.AreEqual(160, diagnostic.Location.SourceSpan.End);
		}

		[TestMethod]
		public async Task AnalyzeWithNoSpecifiedDateTimeKindAndAlias()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(FindingNewDateTimeAnalyzerTests)}.{nameof(FindingNewDateTimeAnalyzerTests.AnalyzeWithNoSpecifiedDateTimeKindAndAlias)}.cs");

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(126, 2));
			Assert.AreEqual(1, diagnostics.Count);
			var diagnostic = diagnostics[0];
			Assert.AreEqual(FindingNewDateTimeConstants.UnspecifiedKindMessage,
				diagnostic.GetMessage());
			Assert.AreEqual(122, diagnostic.Location.SourceSpan.Start);
			Assert.AreEqual(135, diagnostic.Location.SourceSpan.End);
		}

		[TestMethod]
		public async Task AnalyzeWhenUsingUtcDateTimeKindAndAlias()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(FindingNewDateTimeAnalyzerTests)}.{nameof(FindingNewDateTimeAnalyzerTests.AnalyzeWhenUsingUtcDateTimeKindAndAlias)}.cs");

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(126, 2));
			Assert.AreEqual(0, diagnostics.Count);
		}
	}
}
