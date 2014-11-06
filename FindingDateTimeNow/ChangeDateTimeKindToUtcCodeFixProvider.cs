using Microsoft.CodeAnalysis;
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
	[ExportCodeFixProvider(FindingNewDateTimeConstants.DiagnosticId, LanguageNames.CSharp)]
	[Shared]
	public sealed class ChangeDateTimeKindToUtcCodeFixProvider
		: CodeFixProvider
	{
		public sealed override ImmutableArray<string> GetFixableDiagnosticIds()
		{
			return ImmutableArray.Create(FindingNewDateTimeConstants.DiagnosticId);
		}

		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task ComputeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			if (context.CancellationToken.IsCancellationRequested)
			{
				return;
			}

			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;
			var kindToken = root.FindNode(diagnosticSpan) as IdentifierNameSyntax;

			var newKindToken = SyntaxFactory.IdentifierName("Utc");

			var newRoot = root.ReplaceNode(kindToken, newKindToken);

			context.RegisterFix(
				CodeAction.Create(FindingNewDateTimeConstants.CodeFixDescription,
					context.Document.WithSyntaxRoot(newRoot)), diagnostic);
      }
	}
}
