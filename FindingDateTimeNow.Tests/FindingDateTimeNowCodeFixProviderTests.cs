using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FindingDateTimeNow.Tests
{
	[TestClass]
	public sealed class FindingDateTimeNowCodeFixProviderTests
	{
		[TestMethod]
		public void VerifyGetFixableDiagnosticIds()
		{
			var fix = new FindingDateTimeNowCodeFixProvider();
			var ids = fix.GetFixableDiagnosticIds().ToList();

			Assert.AreEqual(1, ids.Count);
			Assert.AreEqual(FindingDateTimeNowConstants.DiagnosticId, ids[0]);
		}

		[TestMethod]
		public async Task VerifyGetFixes()
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

			var document = TestHelpers.CreateDocument(code);
			var tree = await document.GetSyntaxTreeAsync();
			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(
				code, document, new TextSpan(102, 3));
			var sourceSpan = diagnostics[0].Location.SourceSpan;

			var fix = new FindingDateTimeNowCodeFixProvider();
			var actions = (await fix.GetFixesAsync(document, sourceSpan, diagnostics,
				new CancellationToken(false))).ToList();

			Assert.AreEqual(1, actions.Count);
			var action = actions[0];
			Assert.AreEqual(FindingDateTimeNowConstants.CodeFixDescription,
				action.Description);

			var operation = (await action.GetOperationsAsync(
				new CancellationToken(false))).ToArray()[0] as ApplyChangesOperation;
			var newDoc = operation.ChangedSolution.GetDocument(document.Id);
			var newTree = await newDoc.GetSyntaxTreeAsync();
			var changes = newTree.GetChanges(tree);

			Assert.AreEqual(1, changes.Count);
			var change = changes[0];
			Assert.AreEqual("Utc", change.NewText);
			Assert.AreEqual(104, change.Span.Start);
			Assert.AreEqual(104, change.Span.End);
		}
	}
}
