using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindingDateTimeNow.Tests
{
	[TestClass]
	public sealed class FindingNewDateTimeAnalyzerTests
	{
		[TestMethod]
		public void VerifySyntaxKindsOfInterest()
		{
			var analyzer = new FindingNewDateTimeAnalyzer();
			var syntaxKinds = analyzer.SyntaxKindsOfInterest;
			Assert.AreEqual(1, syntaxKinds.Length);
			Assert.AreEqual(SyntaxKind.ObjectCreationExpression, syntaxKinds[0]);
		}

		[TestMethod]
		public void VerifySupportedDiagnostics()
		{
			var analyzer = new FindingNewDateTimeAnalyzer();
			var diagnostics = analyzer.SupportedDiagnostics;
			Assert.AreEqual(2, diagnostics.Length);

			var newDiagnostic = diagnostics[0];
			Assert.AreEqual(newDiagnostic.Id, FindingNewDateTimeConstants.DiagnosticId);
			Assert.AreEqual(newDiagnostic.Description, FindingNewDateTimeConstants.Description);
			Assert.AreEqual(newDiagnostic.MessageFormat, FindingNewDateTimeConstants.FindingDateTimeNowMessage);
			Assert.AreEqual(newDiagnostic.Category, "Usage");
			Assert.AreEqual(newDiagnostic.DefaultSeverity, DiagnosticSeverity.Error);

			var unspecifiedDiagnostic = diagnostics[1];
			Assert.AreEqual(unspecifiedDiagnostic.Id, FindingNewDateTimeConstants.DiagnosticId);
			Assert.AreEqual(unspecifiedDiagnostic.Description, FindingNewDateTimeConstants.Description);
			Assert.AreEqual(unspecifiedDiagnostic.MessageFormat, FindingNewDateTimeConstants.UnspecifiedKindMessage);
			Assert.AreEqual(unspecifiedDiagnostic.Category, "Usage");
			Assert.AreEqual(unspecifiedDiagnostic.DefaultSeverity, DiagnosticSeverity.Error);
		}

		[TestMethod]
		public async Task AnalyzeWhenUsingLocalDateTimeKind()
		{
			var code = @"
using System;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = new DateTime(10000, DateTimeKind.Local);
	}
}";

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(97, 8));
			Assert.AreEqual(1, diagnostics.Count);
			var diagnostic = diagnostics[0];
			Assert.AreEqual(FindingNewDateTimeConstants.FindingDateTimeNowMessage,
				diagnostic.GetMessage());
			Assert.AreEqual(115, diagnostic.Location.SourceSpan.Start);
			Assert.AreEqual(133, diagnostic.Location.SourceSpan.End);
		}

		[TestMethod]
		public async Task AnalyzeWhenUsingUnspecifiedDateTimeKind()
		{
			var code = @"
using System;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = new DateTime(10000, DateTimeKind.Unspecified);
	}
}";

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(97, 8));
			Assert.AreEqual(1, diagnostics.Count);
			var diagnostic = diagnostics[0];
			Assert.AreEqual(FindingNewDateTimeConstants.FindingDateTimeNowMessage,
				diagnostic.GetMessage());
			Assert.AreEqual(115, diagnostic.Location.SourceSpan.Start);
			Assert.AreEqual(139, diagnostic.Location.SourceSpan.End);
		}

		[TestMethod]
		public async Task AnalyzeWithNoSpecifiedKind()
		{
			var code = @"
using System;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = new DateTime(10000);
	}
}";

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(97, 8));
			Assert.AreEqual(1, diagnostics.Count);
			var diagnostic = diagnostics[0];
			Assert.AreEqual(FindingNewDateTimeConstants.UnspecifiedKindMessage,
				diagnostic.GetMessage());
			Assert.AreEqual(95, diagnostic.Location.SourceSpan.Start);
			Assert.AreEqual(114, diagnostic.Location.SourceSpan.End);
		}

		[TestMethod]
		public async Task AnalyzeWhenUsingUtcDateTimeKind()
		{
			var code = @"
using System;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = new DateTime(10000, DateTimeKind.Utc);
	}
}";

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(97, 8));
			Assert.AreEqual(0, diagnostics.Count);
		}

		[TestMethod]
		public async Task AnalyzeWhenUsingLocalDateTimeKindAndAlias()
		{
			var code = @"
using System;
using DT = System.DateTime;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = new DT(10000, DateTimeKind.Local);
	}
}";

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(126, 2));
			Assert.AreEqual(1, diagnostics.Count);
			var diagnostic = diagnostics[0];
			Assert.AreEqual(FindingNewDateTimeConstants.FindingDateTimeNowMessage,
				diagnostic.GetMessage());
			Assert.AreEqual(138, diagnostic.Location.SourceSpan.Start);
			Assert.AreEqual(156, diagnostic.Location.SourceSpan.End);
		}

		[TestMethod]
		public async Task AnalyzeWhenUsingUnspecifiedDateTimeKindAndAlias()
		{
			var code = @"
using System;
using DT = System.DateTime;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = new DT(10000, DateTimeKind.Unspecified);
	}
}";

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(126, 2));
			Assert.AreEqual(1, diagnostics.Count);
			var diagnostic = diagnostics[0];
			Assert.AreEqual(FindingNewDateTimeConstants.FindingDateTimeNowMessage,
				diagnostic.GetMessage());
			Assert.AreEqual(138, diagnostic.Location.SourceSpan.Start);
			Assert.AreEqual(162, diagnostic.Location.SourceSpan.End);
		}

		[TestMethod]
		public async Task AnalyzeWithNoSpecifiedDateTimeKindAndAlias()
		{
			var code = @"
using System;
using DT = System.DateTime;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = new DT(10000);
	}
}";

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(126, 2));
			Assert.AreEqual(1, diagnostics.Count);
			var diagnostic = diagnostics[0];
			Assert.AreEqual(FindingNewDateTimeConstants.UnspecifiedKindMessage,
				diagnostic.GetMessage());
			Assert.AreEqual(124, diagnostic.Location.SourceSpan.Start);
			Assert.AreEqual(137, diagnostic.Location.SourceSpan.End);
		}

		[TestMethod]
		public async Task AnalyzeWhenUsingUtcDateTimeKindAndAlias()
		{
			var code = @"
using System;
using DT = System.DateTime;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = new DT(10000, DateTimeKind.Utc);
	}
}";

			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, new TextSpan(126, 2));
			Assert.AreEqual(0, diagnostics.Count);
		}
	}
}
