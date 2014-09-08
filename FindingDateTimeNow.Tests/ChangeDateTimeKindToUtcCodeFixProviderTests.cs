﻿using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FindingDateTimeNow.Tests
{
	[TestClass]
	public sealed class ChangeDateTimeKindToUtcCodeFixProviderTests
	{
		[TestMethod]
		public void VerifyGetFixableDiagnosticIds()
		{
			var fix = new ChangeDateTimeKindToUtcCodeFixProvider();
			var ids = fix.GetFixableDiagnosticIds().ToList();

			Assert.AreEqual(1, ids.Count);
			Assert.AreEqual(FindingNewDateTimeConstants.DiagnosticId, ids[0]);
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
		var x = new DateTime(10000, DateTimeKind.Local);
	}
}";

			var document = TestHelpers.CreateDocument(code);
			var tree = await document.GetSyntaxTreeAsync();
			var diagnostics = await TestHelpers.GetDiagnosticsAsync<FindingNewDateTimeAnalyzer>(
				code, document, new TextSpan(97, 8));
			var sourceSpan = diagnostics[0].Location.SourceSpan;

			var fix = new ChangeDateTimeKindToUtcCodeFixProvider();
			var actions = (await fix.GetFixesAsync(document, sourceSpan, diagnostics,
				new CancellationToken(false))).ToList();

			Assert.AreEqual(1, actions.Count);
			var action = actions[0];
			Assert.AreEqual(FindingNewDateTimeConstants.CodeFixDescription,
				action.Description);

			var operation = (await action.GetOperationsAsync(
				new CancellationToken(false))).ToArray()[0] as ApplyChangesOperation;
			var newDoc = operation.ChangedSolution.GetDocument(document.Id);
			var newTree = await newDoc.GetSyntaxTreeAsync();
			var changes = newTree.GetChanges(tree);

			Assert.AreEqual(1, changes.Count);
			var change = changes[0];
			Assert.AreEqual("Utc", change.NewText);
			Assert.AreEqual(128, change.Span.Start);
			Assert.AreEqual(133, change.Span.End);
		}
	}
}
