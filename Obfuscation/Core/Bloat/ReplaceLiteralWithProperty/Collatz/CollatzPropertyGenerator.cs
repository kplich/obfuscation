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
using static Obfuscation.Utils.SyntaxGenerationUtils;

namespace Obfuscation.Core.Bloat.ReplaceLiteralWithProperty.Collatz
{
    public sealed class CollatzPropertyGenerator : PropertyGenerator
    {
        private class CollatzPropertyGeneratorBuilder : IBuilder<CollatzPropertyGenerator>
        {
            public string DisplayName => "Collatz property generator";
            public CollatzPropertyGenerator Build(IImmutableList<IIdentifierGenerator> identifierGenerators, string doNotObfuscateAttributeName)
            {
                return new CollatzPropertyGenerator(identifierGenerators, doNotObfuscateAttributeName);
            }
        }
        
        private readonly string _collatzFunctionName;

        private CollatzPropertyGenerator(IImmutableList<IIdentifierGenerator> identifierGenerators, string doNotObfuscateAttributeName) : base(identifierGenerators, doNotObfuscateAttributeName)
        {
            _collatzFunctionName = ChooseGenerator().TransformMethodName(string.Empty);
        }

        public override bool SupportsNumericLiterals()
        {
            return true;
        }
        
        public override ClassDeclarationSyntax PrepareClass(ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration.DoesNotContainAnAttributeWithName(DoNotObfuscateAttributeName))
            {
                classDeclaration = classDeclaration.AddDoNotObfuscateAttribute(DoNotObfuscateAttributeName);
            }

            if (classDeclaration.Members.OfType<MethodDeclarationSyntax>().All(method => method.Identifier.Text != _collatzFunctionName))
            {
                classDeclaration = classDeclaration.AddMembers(
                    GenerateCollatzCalculatingFunction(_collatzFunctionName, DoNotObfuscateAttributeName));
            }

            return classDeclaration;
        }

        public override string DisplayName => "Collatz property generator";

        public override PropertyDeclarationSyntax GenerateProperty(LiteralExpressionSyntax literal)
        {
            if (!Supports(literal)) return null;

            var newName = ChooseGenerator().TransformName(string.Empty);

            var attributeLists = AttributeListWithSingleAttribute(DoNotObfuscateAttributeName);
            var modifiers = new SyntaxTokenList(
                Token(SyntaxKind.PublicKeyword)
                    .WithLeadingTrivia(TabulatorTrivia(2))
                    .WithTrailingTrivia(SpaceTrivia()));

            // sometimes, the property needs to be static!
            if (literal.NeedsToBeStatic())
            {
                modifiers = modifiers.Add(Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(SpaceTrivia()));
            }

            var nodeText = literal.Token.Text;
            var type = PredefinedType(nodeText.Contains(".")
                ? Token(SyntaxKind.DoubleKeyword)
                : Token(SyntaxKind.IntKeyword)).WithTrailingTrivia(Space);

            var identifierToken = Identifier(SyntaxTriviaList.Empty, newName, SpaceTrivia());
            
            var literalValue = int.Parse(literal.Token.Text);
            
            var propertyBlock = GenerateCollatzPropertyBlockForNumber(literalValue);

            var accessorList = AccessorList(
                new SyntaxList<AccessorDeclarationSyntax>(
                    AccessorDeclaration(
                        SyntaxKind.GetAccessorDeclaration,
                        propertyBlock
                    )
                )
            );

            return PropertyDeclaration(
                attributeLists,
                modifiers,
                type,
                null,
                identifierToken,
                accessorList,
                null,
                null
            ).WithLeadingTrivia(CarriageReturn, CarriageReturn);
        }

        private BlockSyntax GenerateCollatzPropertyBlockForNumber(int number)
        {
            if (number.IsSmall())
            {
                var numbers = GenerateRandomValuesForCollatzPropertyWithSmallestNumber(number);
                return GenerateCollatzPropertyBlockForSmallestNumber(numbers);
            }

            if (number.IsMedium())
            {
                var numbers = GenerateRandomValuesForCollatzPropertyWithMediumNumber(number);
                return GenerateCollatzPropertyBlockForMediumNumber(numbers);
            }
            
            if (number.IsBig())
            {
                var numbers = GenerateRandomValuesForCollatzPropertyWithBigNumber(number);
                return GenerateCollatzPropertyBlockForBigNumber(numbers);
            }

            throw new ArgumentException("number can't be negative!");
        }

        private const int CollatzArraySize = CollatzUtils.GreatestSmallNumber + 1;

        private static int[] GenerateRandomValuesForCollatzPropertyWithSmallestNumber(int number)
        {
            if (!number.IsSmall())
            {
                throw new ArgumentOutOfRangeException(nameof(number), "number must be small (from 0 to 10)");
            }
            
            var numbersAndSequenceLengths = new Dictionary<int, int>();

            // generate 11 numbers with DIFFERENT sequence lengths
            while (numbersAndSequenceLengths.Count != CollatzArraySize)
            {
                var nextRandomValue = new Random().Next();
                var collatzValue = CollatzUtils.CalculateSequenceLength(nextRandomValue);

                if (!numbersAndSequenceLengths.ContainsValue(collatzValue))
                {
                    numbersAndSequenceLengths[nextRandomValue] = collatzValue;
                }
            }

            // sort the values by sequence length
            var sortedListOfPairs = numbersAndSequenceLengths.OrderBy(pair => pair.Value).ToList();
            
            // get the number with the smallest sequence length
            var numberWithSmallestCollatzLength = sortedListOfPairs[0].Key;

            // shuffle the values again
            var shuffledListOfPairs = sortedListOfPairs.Shuffle();

            // get the index of the value with the smallest sequence length
            var indexOfNumberWithSmallestCollatzLength =
                shuffledListOfPairs.FindIndex(pair => pair.Key == numberWithSmallestCollatzLength);

            // put the value with the smallest sequence length at the given index
            return shuffledListOfPairs.SwapElements(indexOfNumberWithSmallestCollatzLength, number)
                .Select(pair => pair.Key).ToArray();
        }

        private static int[] GenerateRandomValuesForCollatzPropertyWithMediumNumber(int number)
        {
            if (!number.IsMedium())
            {
                throw new ArgumentOutOfRangeException(nameof(number), "number must be medium (from 11 to 200)");
            }
            
            var numbersAndSequenceLengths = new Dictionary<int, int>();

            // find a number that has a sequence length equal to a given number
            var numberWithCollatzLength = CollatzUtils.GenerateNumberWithSequenceLength(number);
            
            // add it to the dictionary
            numbersAndSequenceLengths[numberWithCollatzLength] = number;

            // fill the array with numbers with GREATER sequence lengths
            while (numbersAndSequenceLengths.Count != CollatzArraySize)
            {
                var nextRandomValue = new Random().Next();
                var collatzValue = CollatzUtils.CalculateSequenceLength(nextRandomValue);

                if (numbersAndSequenceLengths.ContainsValue(collatzValue)) continue;
                
                if (collatzValue > number)
                {
                    numbersAndSequenceLengths[nextRandomValue] = collatzValue;
                }
            }

            // shuffle the pairs
            var shuffledListOfPairs = numbersAndSequenceLengths.ToList().Shuffle();

            // return the numbers
            return shuffledListOfPairs.Select(pair => pair.Key).ToArray();
        }
        
        private static int[] GenerateRandomValuesForCollatzPropertyWithBigNumber(int number)
        {
            if (!number.IsBig())
            {
                throw new ArgumentOutOfRangeException(nameof(number), "number must be big (greater than 200)");
            }
            
            var numbersAndSequenceLengths = new Dictionary<int, int>();

            // calculate the length of the sequence for the given number
            var sequenceLengthOfGivenNumber = CollatzUtils.CalculateSequenceLength(number);
            
            // and add it to the dictionary
            numbersAndSequenceLengths[number] = sequenceLengthOfGivenNumber;

            // fill the array with numbers with GREATER sequence lengths
            while (numbersAndSequenceLengths.Count != CollatzArraySize)
            {
                var nextRandomValue = new Random().Next();
                var collatzValue = CollatzUtils.CalculateSequenceLength(nextRandomValue);

                if (numbersAndSequenceLengths.ContainsValue(collatzValue)) continue;
                if (collatzValue > sequenceLengthOfGivenNumber)
                {
                    numbersAndSequenceLengths[nextRandomValue] = collatzValue;
                }
            }

            // shuffle the values
            var shuffledListOfPairs = numbersAndSequenceLengths.ToList().Shuffle();

            // put the value with the smallest collatz sequence length at the given index
            return shuffledListOfPairs.Select(pair => pair.Key).ToArray();
        }

        // IMPORTANT: all the properties look for the SMALLEST values

        private BlockSyntax GenerateCollatzPropertyBlockForSmallestNumber(int[] numbers)
        {
            const string arrayName = "array";
            
            var numbersJoined = string.Join(',', numbers);
            var arrayDeclaration = $"var {arrayName} = new[] {{{numbersJoined}}};";

            const string loopIndexName = "i";

            return Block(
                ParseStatement(arrayDeclaration).WithTrailingTrivia(CarriageReturn),
                ParseStatement("var length = int.MaxValue;").WithTrailingTrivia(CarriageReturn),
                ParseStatement("var index = 0;").WithTrailingTrivia(CarriageReturn, CarriageReturn),
                ArrayBasedForLoop(loopIndexName, arrayName, 
                    Block(
                        ParseStatement($"var newLength = {_collatzFunctionName}({arrayName}[{loopIndexName}]);").WithTrailingTrivia(CarriageReturn),
                        IfStatement(ParseExpression("newLength < length"),
                            Block(
                                ParseStatement("length = newLength;").WithTrailingTrivia(CarriageReturn),
                                ParseStatement($"index = {loopIndexName};").WithTrailingTrivia(CarriageReturn)
                            ).WithLeadingTrivia(CarriageReturn).WithTrailingTrivia(CarriageReturn)
                        ).WithTrailingTrivia(CarriageReturn)
                    ).WithLeadingTrivia(CarriageReturn).WithTrailingTrivia(CarriageReturn)
                ),
                ReturnStatement(ParseExpression("index").WithLeadingTrivia(Space)).WithTrailingTrivia(CarriageReturn)
            ).WithTrailingTrivia(CarriageReturn);
        }

        private BlockSyntax GenerateCollatzPropertyBlockForMediumNumber(int[] numbers)
        {
            const string arrayName = "array";
            
            var numbersJoined = string.Join(',', numbers);
            var arrayDeclaration = $"var {arrayName} = new[] {{{numbersJoined}}};";

            const string loopIndexName = "i";

            return Block(
                ParseStatement(arrayDeclaration).WithTrailingTrivia(CarriageReturn),
                ParseStatement("var length = int.MaxValue;").WithTrailingTrivia(CarriageReturn),
                ArrayBasedForLoop(loopIndexName, arrayName, Block(
                    ParseStatement($"var newLength = {_collatzFunctionName}({arrayName}[{loopIndexName}]);").WithTrailingTrivia(CarriageReturn),
                    IfStatement(ParseExpression("newLength < length"),
                        Block(
                            ParseStatement("length = newLength;").WithTrailingTrivia(CarriageReturn)
                        ).WithLeadingTrivia(CarriageReturn).WithTrailingTrivia(CarriageReturn)
                    ).WithTrailingTrivia(CarriageReturn)
                ).WithLeadingTrivia(CarriageReturn).WithTrailingTrivia(CarriageReturn)),
                ReturnStatement(ParseExpression("length").WithLeadingTrivia(Space)).WithTrailingTrivia(CarriageReturn)
            ).WithTrailingTrivia(CarriageReturn);
        }

        private BlockSyntax GenerateCollatzPropertyBlockForBigNumber(int[] numbers)
        {
            const string arrayName = "array";
            
            var numbersJoined = string.Join(',', numbers);
            var arrayDeclaration = $"var {arrayName} = new[] {{{numbersJoined}}};";

            const string loopIndexName = "i";

            return Block(
                ParseStatement(arrayDeclaration).WithTrailingTrivia(CarriageReturn),
                ParseStatement("var length = int.MaxValue;").WithTrailingTrivia(CarriageReturn),
                ParseStatement("var index = 0;").WithTrailingTrivia(CarriageReturn, CarriageReturn),
                ArrayBasedForLoop(loopIndexName, arrayName, 
                    Block(
                        ParseStatement($"var newLength = {_collatzFunctionName}({arrayName}[{loopIndexName}]);").WithTrailingTrivia(CarriageReturn),
                        IfStatement(ParseExpression("newLength < length"),
                            Block(
                                ParseStatement("length = newLength;").WithTrailingTrivia(CarriageReturn),
                                ParseStatement($"index = {loopIndexName};").WithTrailingTrivia(CarriageReturn)
                            ).WithLeadingTrivia(CarriageReturn).WithTrailingTrivia(CarriageReturn)
                        ).WithTrailingTrivia(CarriageReturn)
                    ).WithLeadingTrivia(CarriageReturn).WithTrailingTrivia(CarriageReturn)
                ),
                ReturnStatement(ParseExpression($"{arrayName}[index]").WithLeadingTrivia(Space)).WithTrailingTrivia(CarriageReturn)
            ).WithTrailingTrivia(CarriageReturn);
        }
        
        private static MethodDeclarationSyntax GenerateCollatzCalculatingFunction(string collatzFunctionName,
            string doNotObfuscateAttributeName)
        {
            // TODO: obfuscate parameter names
            
            var attributeLists = AttributeListWithSingleAttribute(doNotObfuscateAttributeName);

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