using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FindingDateTimeNow
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class FindingNewDateTimeAnalyzer
		: DiagnosticAnalyzer
	{
		private static DiagnosticDescriptor changeDateTimeKindToUtcRule = new DiagnosticDescriptor(
			FindingNewDateTimeConstants.DiagnosticId, FindingNewDateTimeConstants.Title,
				FindingNewDateTimeConstants.FindingDateTimeNowMessage,
				FindingNewDateTimeConstants.Category, DiagnosticSeverity.Error, true);
		private static DiagnosticDescriptor unspecifiedKindRule = new DiagnosticDescriptor(
			FindingNewDateTimeConstants.UnspecifiedDiagnosticId, FindingNewDateTimeConstants.Title,
				FindingNewDateTimeConstants.UnspecifiedKindMessage,
				FindingNewDateTimeConstants.Category, DiagnosticSeverity.Error, true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
		{
			get
			{
				return ImmutableArray.Create(FindingNewDateTimeAnalyzer.changeDateTimeKindToUtcRule,
					FindingNewDateTimeAnalyzer.unspecifiedKindRule);
			}
		}

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction<SyntaxKind>(
				FindingNewDateTimeAnalyzer.AnalyzeObjectCreationExpression, SyntaxKind.ObjectCreationExpression);
		}

		private static void AnalyzeObjectCreationExpression(SyntaxNodeAnalysisContext context)
		{
			var creationNode = (ObjectCreationExpressionSyntax)context.Node;
			var creationNameNode = creationNode.Type as IdentifierNameSyntax;

			if (creationNameNode != null)
			{
				var creationSymbol = context.SemanticModel.GetSymbolInfo(creationNode).Symbol;

				if (creationSymbol != null &&
					creationSymbol.ContainingType.ToDisplayString() ==
						Values.ExpectedContainingDateTimeTypeDisplayString &&
					creationSymbol.ContainingAssembly.ToDisplayString().Contains(
						Values.ExpectedContainingAssemblyDisplayString))
				{
					var argument = FindingNewDateTimeAnalyzer.GetInvalidArgument(
						creationNode, context.SemanticModel);

					if (argument != null)
					{
						var argumentValue = argument.Value;
						if (argumentValue.ValueText == "Local" ||
							argumentValue.ValueText == "Unspecified")
						{
							context.ReportDiagnostic(Diagnostic.Create(FindingNewDateTimeAnalyzer.changeDateTimeKindToUtcRule,
								argumentValue.GetLocation()));
						}
					}
					else
					{
						context.ReportDiagnostic(Diagnostic.Create(FindingNewDateTimeAnalyzer.unspecifiedKindRule,
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
