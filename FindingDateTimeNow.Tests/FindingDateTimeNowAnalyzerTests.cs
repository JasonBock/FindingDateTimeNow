using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace FindingDateTimeNow.Tests
{
	[TestClass]
	public sealed class FindingDateTimeNowAnalyzerTests
	{
		[TestMethod]
		public void VerifySupportedDiagnostics()
		{
			var analyzer = new FindingDateTimeNowAnalyzer();
			var diagnostics = analyzer.SupportedDiagnostics;
			Assert.AreEqual(1, diagnostics.Length);

			var diagnostic = diagnostics[0];
			Assert.AreEqual(diagnostic.Id, FindingDateTimeNowConstants.DiagnosticId, nameof(DiagnosticDescriptor.Id));
			Assert.AreEqual(diagnostic.Title.ToString(), FindingDateTimeNowConstants.Title, nameof(DiagnosticDescriptor.Title));
			Assert.AreEqual(diagnostic.MessageFormat.ToString(), FindingDateTimeNowConstants.Message, nameof(DiagnosticDescriptor.MessageFormat));
			Assert.AreEqual(diagnostic.Category, FindingDateTimeNowConstants.Category, nameof(DiagnosticDescriptor.Category));
			Assert.AreEqual(diagnostic.DefaultSeverity, DiagnosticSeverity.Error);
		}

		[TestMethod]
		public async Task AnalyzeWhenCallingDateTimeNow()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(FindingDateTimeNowAnalyzerTests)}.{nameof(FindingDateTimeNowAnalyzerTests.AnalyzeWhenCallingDateTimeNow)}.cs");

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(
				code, new TextSpan(102, 3));
			Assert.AreEqual(1, diagnostics.Count);
		}

		[TestMethod]
		public async Task AnalyzeWhenCallingDateTimeNowWithAlias()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(FindingDateTimeNowAnalyzerTests)}.{nameof(FindingDateTimeNowAnalyzerTests.AnalyzeWhenCallingDateTimeNowWithAlias)}.cs");

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(
				code, new TextSpan(110, 3));
			Assert.AreEqual(1, diagnostics.Count);
		}

		[TestMethod]
		public async Task AnalyzeWhenCallingDateTimeUtcNow()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(FindingDateTimeNowAnalyzerTests)}.{nameof(FindingDateTimeNowAnalyzerTests.AnalyzeWhenCallingDateTimeUtcNow)}.cs");

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(
				code, new TextSpan(102, 6));
			Assert.AreEqual(0, diagnostics.Count);
		}

		[TestMethod]
		public async Task AnalyzeWhenCallingDateTimeUtcNowWithAlias()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(FindingDateTimeNowAnalyzerTests)}.{nameof(FindingDateTimeNowAnalyzerTests.AnalyzeWhenCallingDateTimeUtcNowWithAlias)}.cs");

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(
				code, new TextSpan(110, 6));
			Assert.AreEqual(0, diagnostics.Count);
		}

		[TestMethod]
		public async Task AnalyzeWhenCallingNowAsAProperty()
		{
			var code = File.ReadAllText(
				$@"Targets\{nameof(FindingDateTimeNowAnalyzerTests)}.{nameof(FindingDateTimeNowAnalyzerTests.AnalyzeWhenCallingNowAsAProperty)}.cs");

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(
				code, new TextSpan(98, 3));
			Assert.AreEqual(0, diagnostics.Count);
		}
	}
}
