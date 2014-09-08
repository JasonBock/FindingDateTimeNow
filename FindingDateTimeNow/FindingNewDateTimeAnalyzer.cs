using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FindingDateTimeNow
{
	[DiagnosticAnalyzer]
	[ExportDiagnosticAnalyzer(FindingNewDateTimeConstants.DiagnosticId, LanguageNames.CSharp)]
	public sealed class FindingNewDateTimeAnalyzer
		: ISyntaxNodeAnalyzer<SyntaxKind>
	{
		private static DiagnosticDescriptor changeDateTimeKindToUtcRule = new DiagnosticDescriptor(
			FindingNewDateTimeConstants.DiagnosticId, FindingNewDateTimeConstants.Description,
				FindingNewDateTimeConstants.FindingDateTimeNowMessage, "Usage", DiagnosticSeverity.Error, true);
		private static DiagnosticDescriptor unspecifiedKindRule = new DiagnosticDescriptor(
			FindingNewDateTimeConstants.DiagnosticId, FindingNewDateTimeConstants.Description,
				FindingNewDateTimeConstants.UnspecifiedKindMessage, "Usage", DiagnosticSeverity.Error, true);

		public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
		{
			get
			{
				return ImmutableArray.Create(FindingNewDateTimeAnalyzer.changeDateTimeKindToUtcRule,
					FindingNewDateTimeAnalyzer.unspecifiedKindRule);
			}
		}

		public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest
		{
			get
			{
				return ImmutableArray.Create(SyntaxKind.ObjectCreationExpression);
			}
		}

		public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, 
			AnalyzerOptions options, CancellationToken cancellationToken)
		{
			var creationNode = (ObjectCreationExpressionSyntax)node;
			var creationNameNode = creationNode.Type as IdentifierNameSyntax;

			if (creationNameNode != null)
			{
				var creationSymbol = semanticModel.GetSymbolInfo(creationNode).Symbol;

				if (creationSymbol != null &&
					creationSymbol.ContainingType.ToDisplayString() ==
						Values.ExpectedContainingDateTimeTypeDisplayString &&
					creationSymbol.ContainingAssembly.ToDisplayString().Contains(
						Values.ExpectedContainingAssemblyDisplayString))
				{
					var argument = FindingNewDateTimeAnalyzer.GetInvalidArgument(
						creationNode, semanticModel);

					if (argument != null)
					{
						if (argument.Value.ValueText == "Local" ||
							argument.Value.ValueText == "Unspecified")
						{
							addDiagnostic(Diagnostic.Create(FindingNewDateTimeAnalyzer.changeDateTimeKindToUtcRule,
								argument.Value.GetLocation()));
						}
					}
					else
					{
						addDiagnostic(Diagnostic.Create(FindingNewDateTimeAnalyzer.unspecifiedKindRule,
							creationNode.GetLocation()));
					}
				}
			}
		}

		private static SyntaxToken? GetInvalidArgument(
			ObjectCreationExpressionSyntax creationToken, SemanticModel model)
		{
			foreach (var argument in creationToken.ArgumentList.Arguments)
			{
				var argumentExpression = argument.Expression as MemberAccessExpressionSyntax;
            if (argumentExpression != null)
				{
					var argumentSymbolNode = model.GetSymbolInfo(argumentExpression).Symbol;

					if (argumentSymbolNode.ContainingType.ToDisplayString() ==
						Values.ExpectedContainingDateTimeKindTypeDisplayString)
					{
						return argumentExpression.Name.Identifier;
					}
				}
			}

			return null;
		}
	}
}
