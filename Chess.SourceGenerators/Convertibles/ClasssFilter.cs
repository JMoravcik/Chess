using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.SourceGenerators.Convertibles
{
    public class ClassFilter : ISyntaxContextReceiver
    {
        public List<INamedTypeSymbol> Types { get; } = new List<INamedTypeSymbol>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (!(context.Node is ClassDeclarationSyntax declarationSyntax)) return;

            var classSymbol = context.SemanticModel.GetDeclaredSymbol(declarationSyntax) as INamedTypeSymbol;
            
            if (classSymbol == null || classSymbol.IsAbstract) return;
            if (classSymbol.BaseType == null) return;

            var attrs = classSymbol.BaseType.GetAttributes();
            if (attrs.Any(a => a.AttributeClass.ToDisplayString() == "Convertible.ConvertibleAttribute"))
            {
                Types.Add(classSymbol);
            }
        }
    }
}
