using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace FindingDateTimeNow.Tests
{
	[TestClass]
	public sealed class FindingDateTimeNowAnalyzerTests
	{
		[TestMethod]
		public void VerifySyntaxKindsOfInterest()
		{
			var analyzer = new FindingDateTimeNowAnalyzer();
			var syntaxKinds = analyzer.SyntaxKindsOfInterest;
			Assert.AreEqual(1, syntaxKinds.Length);
			Assert.AreEqual(SyntaxKind.SimpleMemberAccessExpression, syntaxKinds[0]);
		}

		[TestMethod]
		public void VerifySupportedDiagnostics()
		{
			var analyzer = new FindingDateTimeNowAnalyzer();
			var diagnostics = analyzer.SupportedDiagnostics;
			Assert.AreEqual(1, diagnostics.Length);

			var diagnostic = diagnostics[0];
			Assert.AreEqual(diagnostic.Id, FindingDateTimeNowConstants.DiagnosticId);
			Assert.AreEqual(diagnostic.Description, FindingDateTimeNowConstants.Description);
			Assert.AreEqual(diagnostic.MessageFormat, FindingDateTimeNowConstants.Message);
			Assert.AreEqual(diagnostic.Category, "Usage");
			Assert.AreEqual(diagnostic.DefaultSeverity, DiagnosticSeverity.Error);
		}

		[TestMethod]
		public async Task AnalyzeWhenCallingDateTimeNow()
		{
			var code = @"
using System;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = DateTime.Now;
	}
}";

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(
				code, new TextSpan(102, 3));
			Assert.AreEqual(1, diagnostics.Count);
		}

		[TestMethod]
		public async Task AnalyzeWhenCallingDateTimeNowWithAlias()
		{
			var code = @"
using DT = System.DateTime;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = DT.Now;
	}
}";

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(
				code, new TextSpan(110, 3));
			Assert.AreEqual(1, diagnostics.Count);
		}

		[TestMethod]
		public async Task AnalyzeWhenCallingDateTimeUtcNow()
		{
			var code = @"
using System;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = DateTime.UtcNow;
	}
}";

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(
				code, new TextSpan(102, 6));
			Assert.AreEqual(0, diagnostics.Count);
		}

		[TestMethod]
		public async Task AnalyzeWhenCallingDateTimeUtcNowWithAlias()
		{
			var code = @"
using DT = System.DateTime;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = DT.UtcNow;
	}
}";

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(
				code, new TextSpan(110, 6));
			Assert.AreEqual(0, diagnostics.Count);
		}

		[TestMethod]
		public async Task AnalyzeWhenCallingNowAsAProperty()
		{
			var code = @"
using System;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = this.Now;
	}

	public string Now { get; set; }
}";

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(
				code, new TextSpan(98, 3));
			Assert.AreEqual(0, diagnostics.Count);
		}
	}
}
