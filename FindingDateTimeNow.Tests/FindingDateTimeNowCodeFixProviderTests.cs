using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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

			var document = TestHelpers.Create(code);
			var tree = await document.GetSyntaxTreeAsync();
			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(
				document, new TextSpan(102, 3));
			var sourceSpan = diagnostics[0].Location.SourceSpan;

			var actions = new List<CodeAction>();
			var codeActionRegistration = new Action<CodeAction, IEnumerable<Diagnostic>>(
				(a, _) => { actions.Add(a); });

			var fix = new FindingDateTimeNowCodeFixProvider();
			var codeFixContext = new CodeFixContext(document, diagnostics[0], codeActionRegistration, new CancellationToken(false));
			await fix.ComputeFixesAsync(codeFixContext);

			Assert.AreEqual(1, actions.Count);
			var action = actions[0];

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
