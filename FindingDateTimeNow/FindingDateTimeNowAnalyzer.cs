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
	[ExportDiagnosticAnalyzer(FindingDateTimeNowConstants.DiagnosticId, LanguageNames.CSharp)]
	public sealed class FindingDateTimeNowAnalyzer
		: ISyntaxNodeAnalyzer<SyntaxKind>
	{
		private static DiagnosticDescriptor changeNowToUtcNowRule = new DiagnosticDescriptor(
			FindingDateTimeNowConstants.DiagnosticId, FindingDateTimeNowConstants.Description,
				FindingDateTimeNowConstants.Message, "Usage", DiagnosticSeverity.Error, true);

		public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
		{
			get
			{
				return ImmutableArray.Create(FindingDateTimeNowAnalyzer.changeNowToUtcNowRule);
			}
		}

		public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest
		{
			get
			{
				return ImmutableArray.Create(SyntaxKind.SimpleMemberAccessExpression);
			}
		}

		public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic,
			AnalyzerOptions options, CancellationToken cancellationToken)
		{
			var memberNode = (MemberAccessExpressionSyntax)node;

			if (memberNode.OperatorToken.IsKind(SyntaxKind.DotToken) &&
				memberNode.Name.Identifier.ValueText == "Now")
			{
				var symbol = semanticModel.GetSymbolInfo(memberNode.Name).Symbol;

				if (symbol != null &&
					symbol.ContainingType.ToDisplayString() ==
						Values.ExpectedContainingDateTimeTypeDisplayString &&
					symbol.ContainingAssembly.ToDisplayString().Contains(
						Values.ExpectedContainingAssemblyDisplayString))
				{
					addDiagnostic(Diagnostic.Create(FindingDateTimeNowAnalyzer.changeNowToUtcNowRule,
						memberNode.Name.Identifier.GetLocation()));
				}
			}
		}
	}
}
