﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace FindingDateTimeNow
{
	[ExportCodeFixProvider(FindingDateTimeNowConstants.DiagnosticId, LanguageNames.CSharp)]
	[Shared]
	public sealed class FindingDateTimeNowCodeFixProvider
		: CodeFixProvider
	{
		public override ImmutableArray<string> FixableDiagnosticIds
		{
			get
			{
				return ImmutableArray.Create(FindingDateTimeNowConstants.DiagnosticId);
			}
		}

		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			if (context.CancellationToken.IsCancellationRequested)
			{
				return;
			}

			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			var nowToken = root.FindToken(diagnosticSpan.Start)
				.Parent.AncestorsAndSelf().OfType<IdentifierNameSyntax>().First().GetFirstToken();

			var utcNowToken = SyntaxFactory.Identifier(nowToken.LeadingTrivia,
				"UtcNow", nowToken.TrailingTrivia);

			var newRoot = root.ReplaceToken(nowToken, utcNowToken);

			context.RegisterCodeFix(
				CodeAction.Create(
					FindingDateTimeNowConstants.Title,
					_ => Task.FromResult<Document>(context.Document.WithSyntaxRoot(newRoot))), diagnostic);
		}
	}
}
