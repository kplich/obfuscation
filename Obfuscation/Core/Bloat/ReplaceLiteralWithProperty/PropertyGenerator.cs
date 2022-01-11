﻿using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Obfuscation.Core.Name;

namespace Obfuscation.Core.Bloat.ReplaceLiteralWithProperty
{
    public abstract class PropertyGenerator
    {
        private readonly IImmutableList<IIdentifierGenerator> _identifierGenerators;
        protected readonly string DoNotObfuscateAttributeName;

        protected PropertyGenerator(IImmutableList<IIdentifierGenerator> identifierGenerators, string doNotObfuscateAttributeName)
        {
            _identifierGenerators = identifierGenerators;
            DoNotObfuscateAttributeName = doNotObfuscateAttributeName;
        }

        public abstract PropertyDeclarationSyntax GenerateProperty(LiteralExpressionSyntax literal);

        public abstract ClassDeclarationSyntax PrepareClass(ClassDeclarationSyntax classDeclaration);

        protected IIdentifierGenerator ChooseGenerator()
        {
            return _identifierGenerators[new Random().Next(_identifierGenerators.Count)];
        }
        
        public virtual bool SupportsCharacterLiterals()
        {
            return false;
        }

        public virtual bool SupportsDefaultLiterals()
        {
            return false;
        }

        public virtual bool SupportsFalseLiterals()
        {
            return false;
        }

        public virtual bool SupportsNullLiterals()
        {
            return false;
        }

        public virtual bool SupportsNumericLiterals()
        {
            return false;
        }

        public virtual bool SupportsStringLiterals()
        {
            return false;
        }

        public virtual bool SupportsTrueLiterals()
        {
            return false;
        }

        public bool Supports(LiteralExpressionSyntax literal)
        {
            return literal.Kind() switch
            {
                SyntaxKind.CharacterLiteralExpression => SupportsCharacterLiterals(),
                SyntaxKind.DefaultLiteralExpression => SupportsDefaultLiterals(),
                SyntaxKind.FalseLiteralExpression => SupportsFalseLiterals(),
                SyntaxKind.NullLiteralExpression => SupportsNullLiterals(),
                SyntaxKind.NumericLiteralExpression => SupportsNumericLiterals(),
                SyntaxKind.StringLiteralExpression => SupportsStringLiterals(),
                SyntaxKind.TrueLiteralExpression => SupportsTrueLiterals(),
                _ => false
            };
        }
    }
}