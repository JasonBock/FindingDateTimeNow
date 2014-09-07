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
	[ExportCodeFixProvider(FindingDateTimeNowConstants.DiagnosticId, LanguageNames.CSharp)]
	public sealed class FindingDateTimeNowCodeFixProvider
		: ICodeFixProvider
	{
		public IEnumerable<string> GetFixableDiagnosticIds()
		{
			return new[] { FindingDateTimeNowConstants.DiagnosticId };
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

			var nowToken = root.FindToken(diagnosticSpan.Start)
				.Parent.AncestorsAndSelf().OfType<IdentifierNameSyntax>().First().GetFirstToken();

			var utcNowToken = SyntaxFactory.Identifier(nowToken.LeadingTrivia, 
				"UtcNow", nowToken.TrailingTrivia);

			var newRoot = root.ReplaceToken(nowToken, utcNowToken);

			return new[]
			{
				CodeAction.Create(FindingDateTimeNowConstants.CodeFixDescription,
					document.WithSyntaxRoot(newRoot))
			};
      }
	}
}
