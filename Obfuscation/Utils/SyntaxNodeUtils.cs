using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Obfuscation.Utils
{
    public static class SyntaxNodeUtils
    {
        public static T GetParent<T>(this SyntaxNode node) where T : SyntaxNode
        {
            var parentNode = node.Parent;
            while (parentNode is not T && parentNode != null)
            {
                parentNode = parentNode.Parent;
            }

            return parentNode as T;
        }

        public static bool IsWithin<T>(this SyntaxNode node) where T : SyntaxNode
        {
            return node.GetParent<T>() != null;
        }

        internal static bool IsStatic(this MethodDeclarationSyntax node)
        {
            return node.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.StaticKeyword));
        }

        internal static bool HasAParentWithAttributeName(this SyntaxNode node, string attributeName)
        {
            while (node != null)
            {
                switch (node)
                {
                    case MemberDeclarationSyntax member when member.HasAnAttributeWithName(attributeName):
                    case ParameterSyntax parameter when parameter.HasAnAttributeWithName(attributeName):
                    case ReturnStatementSyntax statement when statement.HasAnAttributeWithName(attributeName):
                        return true;
                    default:
                        node = node.Parent;
                        break;
                }
            }

            return false;
        }

        internal static bool ContainsAnAttributeWithName(this ClassDeclarationSyntax classDeclaration,
            string attributeName)
        {
            return classDeclaration.Members.OfType<ClassDeclarationSyntax>()
                .Any(cd => cd.IsAnAttributeWithName(attributeName));
        }

        internal static bool DoesNotContainAnAttributeWithName(this ClassDeclarationSyntax classDeclarationSyntax,
            string attributeName)
        {
            return !classDeclarationSyntax.ContainsAnAttributeWithName(attributeName);
        }

        internal static bool IsAnAttributeWithName(this ClassDeclarationSyntax member, string attributeName)
        {
            var baseList = member.BaseList;

            return baseList != null &&
                   baseList.Types.Any(baseListType => baseListType.Type.ToString().Equals("Attribute")) &&
                   member.Identifier.Text.Equals(attributeName);
        }

        internal static bool HasAnAttributeWithName(this MemberDeclarationSyntax member, string attributeName)
        {
            return member.AttributeLists.Any(list =>
                list.Attributes.Any(attribute => attribute.Name.ToString() == attributeName));
        }

        internal static bool HasAnAttributeWithName(this ParameterSyntax parameter, string attributeName)
        {
            return parameter.AttributeLists.Any(list =>
                list.Attributes.Any(attribute => attribute.Name.ToString() == attributeName));
        }

        internal static bool HasAnAttributeWithName(this ReturnStatementSyntax statement, string attributeName)
        {
            return statement.AttributeLists.Any(list =>
                list.Attributes.Any(attribute => attribute.Name.ToString() == attributeName));
        }
    }
}