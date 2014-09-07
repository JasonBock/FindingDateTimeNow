using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FindingDateTimeNow
{
	[ExportCodeFixProvider(FindingNewDateTimeConstants.DiagnosticId, LanguageNames.CSharp)]
	public sealed class ChangeDateTimeKindToUtcCodeFixProvider
		: ICodeFixProvider
	{
		public IEnumerable<string> GetFixableDiagnosticIds()
		{
			return new[] { FindingNewDateTimeConstants.DiagnosticId };
		}

		public async Task<IEnumerable<CodeAction>> GetFixesAsync(Document document, TextSpan span, 
			IEnumerable<Diagnostic> diagnostics, CancellationToken cancellationToken)
		{
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

			if (cancellationToken.IsCancellationRequested)
			{
				return Enumerable.Empty<CodeAction>();
			}

			var diagnostic = diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			var kindToken = (root.FindToken(diagnosticSpan.Start)
				.Parent.AncestorsAndSelf().OfType<ArgumentSyntax>().First().Expression as MemberAccessExpressionSyntax).Name;

			var newKindToken = SyntaxFactory.IdentifierName("Utc");

			var newRoot = root.ReplaceNode(kindToken, newKindToken);

			return new[]
			{
				CodeAction.Create(FindingNewDateTimeConstants.CodeFixDescription,
					document.WithSyntaxRoot(newRoot))
			};
      }
	}
}
