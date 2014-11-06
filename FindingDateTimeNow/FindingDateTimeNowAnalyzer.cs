using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FindingDateTimeNow
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class FindingDateTimeNowAnalyzer
		: DiagnosticAnalyzer
	{
		private static DiagnosticDescriptor changeNowToUtcNowRule = new DiagnosticDescriptor(
			FindingDateTimeNowConstants.DiagnosticId, FindingDateTimeNowConstants.Title,
				FindingDateTimeNowConstants.Message, FindingDateTimeNowConstants.Category, 
				DiagnosticSeverity.Error, true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
		{
			get
			{
				return ImmutableArray.Create(FindingDateTimeNowAnalyzer.changeNowToUtcNowRule);
			}
		}

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction<SyntaxKind>(
				FindingDateTimeNowAnalyzer.AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);
		}

		private static void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
		{
			var memberNode = (MemberAccessExpressionSyntax)context.Node;

			if (memberNode.OperatorToken.IsKind(SyntaxKind.DotToken) &&
				memberNode.Name.Identifier.ValueText == "Now")
			{
				var symbol = context.SemanticModel.GetSymbolInfo(memberNode.Name).Symbol;

				if (symbol != null &&
					symbol.ContainingType.ToDisplayString() ==
						Values.ExpectedContainingDateTimeTypeDisplayString &&
					symbol.ContainingAssembly.ToDisplayString().Contains(
						Values.ExpectedContainingAssemblyDisplayString))
				{
					context.ReportDiagnostic(Diagnostic.Create(FindingDateTimeNowAnalyzer.changeNowToUtcNowRule,
						memberNode.Name.Identifier.GetLocation()));
				}
			}
		}
	}
}
