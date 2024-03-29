﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Obfuscation.Utils;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Obfuscation.Core.Bloat
{
    public class SyntaxTriviaUtils
    {
        public static SyntaxTriviaList SpaceTrivia()
        {
            return new SyntaxTriviaList(SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " "));
        }
        
        public static SyntaxTriviaList TabulatorTrivia(int numberOfTabs, int spacesInTab = 4)
        {
            var whitespaceString = " ".Repeat(numberOfTabs * spacesInTab);
            return new SyntaxTriviaList(SyntaxTrivia(SyntaxKind.WhitespaceTrivia, whitespaceString));
        }
    }
}