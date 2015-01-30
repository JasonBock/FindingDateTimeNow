using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
			Assert.AreEqual(newDiagnostic.Id, FindingNewDateTimeConstants.DiagnosticId, nameof(DiagnosticDescriptor.Id));
			Assert.AreEqual(newDiagnostic.Title.ToString(), FindingNewDateTimeConstants.Title, nameof(DiagnosticDescriptor.Title));
			Assert.AreEqual(newDiagnostic.MessageFormat.ToString(), FindingNewDateTimeConstants.FindingDateTimeNowMessage, nameof(DiagnosticDescriptor.Title));
			Assert.AreEqual(newDiagnostic.Category, FindingNewDateTimeConstants.Category, nameof(DiagnosticDescriptor.Category));
			Assert.AreEqual(newDiagnostic.DefaultSeverity, DiagnosticSeverity.Error, nameof(DiagnosticDescriptor.DefaultSeverity));

			var unspecifiedDiagnostic = diagnostics[1];
			Assert.AreEqual(unspecifiedDiagnostic.Id, FindingNewDateTimeConstants.UnspecifiedDiagnosticId, nameof(DiagnosticDescriptor.Id));
			Assert.AreEqual(unspecifiedDiagnostic.Title.ToString(), FindingNewDateTimeConstants.Title, nameof(DiagnosticDescriptor.Title));
			Assert.AreEqual(unspecifiedDiagnostic.MessageFormat.ToString(), FindingNewDateTimeConstants.UnspecifiedKindMessage, nameof(DiagnosticDescriptor.MessageFormat));
			Assert.AreEqual(unspecifiedDiagnostic.Category, FindingNewDateTimeConstants.Category, nameof(DiagnosticDescriptor.Category));
			Assert.AreEqual(unspecifiedDiagnostic.DefaultSeverity, DiagnosticSeverity.Error, nameof(DiagnosticDescriptor.DefaultSeverity));
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
			Assert.AreEqual(128, diagnostic.Location.SourceSpan.Start);
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
			Assert.AreEqual(128, diagnostic.Location.SourceSpan.Start);
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
			Assert.AreEqual(151, diagnostic.Location.SourceSpan.Start);
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
			Assert.AreEqual(151, diagnostic.Location.SourceSpan.Start);
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
