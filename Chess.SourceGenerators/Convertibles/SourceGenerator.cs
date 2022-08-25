using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.SourceGenerators.Convertibles
{
    public class SourceGenerator
    {
        private readonly INamedTypeSymbol typeSymbol;
        private readonly ITypeSymbol typeArgument;

        public SourceGenerator(INamedTypeSymbol typeSymbol, ITypeSymbol typeArgument)
        {
            this.typeSymbol = typeSymbol;
            this.typeArgument = typeArgument;
        }

        public string GetSource() => $@"
using System;

namespace {typeSymbol.ContainingNamespace.ToDisplayString()}
{{
    public partial class {typeSymbol.Name}
    {{
        public static implicit operator {typeSymbol.Name}({typeArgument.ContainingNamespace.ToDisplayString()}.{typeArgument.Name} arg)
        {{
            if (arg == null) return null;
            var result = new {typeSymbol.Name}();

{GenerateConvertion("result", "arg")}
            return result;
        }}

        public static explicit operator {typeArgument.ContainingNamespace.ToDisplayString()}.{typeArgument.Name}({typeSymbol.Name} arg)
        {{
            if (arg == null) return null;
            var result = new {typeArgument.ContainingNamespace.ToDisplayString()}.{typeArgument.Name}();

{GenerateConvertion("result", "arg")}
            return result;
        }}

        public override {typeArgument.ContainingNamespace.ToDisplayString()}.{typeArgument.Name} WriteTo({typeArgument.ContainingNamespace.ToDisplayString()}.{typeArgument.Name} arg)
        {{
            if (arg == null) return arg;

{GenerateConvertion("arg", "this")}
            return arg;
        }}

        public override void SetFrom({typeArgument.ContainingNamespace.ToDisplayString()}.{typeArgument.Name} arg)
        {{
            if (arg == null) return;

{GenerateConvertion("this", "arg")}
        }}
    }}
}}
";

        private string GenerateConvertion(string typeSymbolName, string typeParameterName)
        {
            StringBuilder result = new StringBuilder();
            var members = typeSymbol.GetMembers();
            if (members == null) return "";
            foreach (var property in GetPropertySymbols(typeSymbol))
            {
                var typeProperty = typeArgument.GetMembers(property.Name).FirstOrDefault() as IPropertySymbol;
                if (typeProperty == null)
                {
                    var baseType = typeArgument.BaseType;
                    if (baseType == null) continue;
                    typeProperty = baseType.GetMembers(property.Name).FirstOrDefault() as IPropertySymbol;
                }

                if (typeProperty == null || typeProperty.DeclaredAccessibility != Accessibility.Public) continue;
                if (typeProperty.Type.Name != property.Type.Name) continue;
                
                result.Append($"            {typeSymbolName}.{typeProperty.Name} = {typeParameterName}.{typeProperty.Name};\n");
            }
            return result.ToString();
        }

        private IEnumerable<IPropertySymbol> GetPropertySymbols(INamedTypeSymbol classSymbol)
        {
            var members = classSymbol.GetMembers();
            if (members == null) yield break;
            foreach (var symbol in members)
            {
                var propertySymbol = symbol as IPropertySymbol;
                if (propertySymbol == null) continue;
                if (propertySymbol.DeclaredAccessibility != Accessibility.Public) continue;
                yield return propertySymbol;
            }

            var baseMembers = classSymbol.BaseType.GetMembers();
            if (baseMembers == null) yield break;
            foreach (var symbol in baseMembers)
            {
                var propertySymbol = symbol as IPropertySymbol;
                if (propertySymbol == null) continue;
                if (propertySymbol.DeclaredAccessibility != Accessibility.Public) continue;
                yield return propertySymbol;
            }
        }
    }
}
