using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Obfuscation.Core.Name;
using Obfuscation.Utils;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Obfuscation.Core.Bloat.SyntaxTriviaUtils;
using static Obfuscation.Core.DoNotObfuscate;

namespace Obfuscation.Core.Bloat.ReplaceLiteralWithProperty.Collatz
{
    public class ReplaceLiteralWithCollatzProperty : CSharpSyntaxRewriter
    {
        private class LiteralExpressionInfo
        {
            public LiteralExpressionSyntax Literal { get; init; }
            public string NewName { get; init; }
        }

        private readonly IImmutableList<IIdentifierGenerator> _identifierGenerators;
        private readonly string _doNotObfuscateAttributeName;
        private readonly string _collatzFunctionName;

        private readonly IDictionary<string, LiteralExpressionInfo> _mapOfLiterals =
            new Dictionary<string, LiteralExpressionInfo>();

        public ReplaceLiteralWithCollatzProperty(IImmutableList<IIdentifierGenerator> generators)
        {
            _identifierGenerators = generators;
            _doNotObfuscateAttributeName = ChooseGenerator().TransformClassName(string.Empty);
            _collatzFunctionName = ChooseGenerator().TransformMethodName(string.Empty);
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node.HasAnAttributeWithName(_doNotObfuscateAttributeName)) return base.VisitClassDeclaration(node);

            var numericLiterals = node.DescendantNodes().OfType<LiteralExpressionSyntax>()
                .Where(syntaxNode => syntaxNode.Kind() == SyntaxKind.NumericLiteralExpression);

            foreach (var literalExpressionSyntax in numericLiterals)
            {
                if (!literalExpressionSyntax.IsWithin<ParameterSyntax>())
                {
                    var newPropertyName = ChooseGenerator().TransformName(string.Empty);
                    _mapOfLiterals[literalExpressionSyntax.Token.Text] = new LiteralExpressionInfo
                    {
                        Literal = literalExpressionSyntax,
                        NewName = newPropertyName
                    };
                }
            }

            var propertyGenerator = new CollatzPropertyGenerator(_collatzFunctionName, _doNotObfuscateAttributeName);

            var doNotObfuscateAttribute = GenerateDoNotObfuscateAttribute(_doNotObfuscateAttributeName);

            var literalsAsProperties =
                _mapOfLiterals
                    .Values
                    .Select(literalInfo =>
                        propertyGenerator.GenerateProperty(literalInfo.Literal, literalInfo.NewName))
                    .ToArray() as MemberDeclarationSyntax[];
            return base.VisitClassDeclaration(
                node.AddMembers(literalsAsProperties)
                    .AddMembers(doNotObfuscateAttribute)
                    .AddMembers(
                        GenerateCollatzCalculatingFunction(_collatzFunctionName, _doNotObfuscateAttributeName)));

        }

        public override SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            if (!_mapOfLiterals.ContainsKey(node.Token.Text) || node.HasAParentWithAttributeName(_doNotObfuscateAttributeName))
                return base.VisitLiteralExpression(node);

            var literalInfo = _mapOfLiterals[node.Token.Text];
            return IdentifierName(literalInfo.NewName).WithTrailingTrivia(SpaceTrivia());

        }

        public IIdentifierGenerator ChooseGenerator()
        {
            return _identifierGenerators[new Random().Next(_identifierGenerators.Count)];
        }

        private static MethodDeclarationSyntax GenerateCollatzCalculatingFunction(string collatzFunctionName,
            string doNotObfuscateAttributeName)
        {
            // TODO: obfuscate parameter names
            
            var attributeLists = new SyntaxList<AttributeListSyntax>(
                AttributeList(
                    new SeparatedSyntaxList<AttributeSyntax>().Add(
                        Attribute(IdentifierName(doNotObfuscateAttributeName)))
                ).WithTrailingTrivia(CarriageReturn)
            );

            var modifiers = new SyntaxTokenList(
                Token(SyntaxKind.PublicKeyword)
                    .WithLeadingTrivia(TabulatorTrivia(2))
                    .WithTrailingTrivia(SpaceTrivia()),
                Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(SpaceTrivia())
            );

            var returnType = PredefinedType(Token(SyntaxKind.IntKeyword)).WithTrailingTrivia(Space);

            var identifier = Identifier(collatzFunctionName);

            var parameters = new SeparatedSyntaxList<ParameterSyntax>();
            var parameterList = ParameterList(parameters.Add(Parameter(
                new SyntaxList<AttributeListSyntax>(),
                new SyntaxTokenList(),
                PredefinedType(Token(SyntaxKind.IntKeyword).WithTrailingTrivia(SpaceTrivia())),
                Identifier("x"),
                null)));

            var constraintClauses = new SyntaxList<TypeParameterConstraintClauseSyntax>();

            var body = Block(
                ParseStatement("var length = 0;").WithTrailingTrivia(CarriageReturn, CarriageReturn),
                ParseStatement("var temp = x;")
                    .WithTrailingTrivia(CarriageReturn),
                WhileStatement(ParseExpression("temp > 1"),
                    Block(
                        ParseStatement("length++;")
                            .WithTrailingTrivia(CarriageReturn, CarriageReturn),
                        IfStatement(ParseExpression("temp % 2 == 0"),
                            Block(
                                ParseStatement("temp /= 2;").WithTrailingTrivia(CarriageReturn)
                            ).WithTrailingTrivia(CarriageReturn),
                            ElseClause(
                                Block(
                                    ParseStatement("temp *= 3;").WithTrailingTrivia(CarriageReturn),
                                    ParseStatement("temp += 1;").WithTrailingTrivia(CarriageReturn)
                                )
                            )
                        ).WithTrailingTrivia(CarriageReturn)
                    ).WithTrailingTrivia(CarriageReturn)
                ).WithTrailingTrivia(CarriageReturn),
                ReturnStatement(
                    ParseExpression("length + 1").WithLeadingTrivia(Space)
                ).WithTrailingTrivia(CarriageReturn)
            ).WithTrailingTrivia(CarriageReturn);

            var methodDeclaration = MethodDeclaration(
                attributeLists,
                modifiers,
                returnType,
                null,
                identifier,
                null,
                parameterList,
                constraintClauses,
                body,
                null
            );

            return methodDeclaration;
        }
    }
}