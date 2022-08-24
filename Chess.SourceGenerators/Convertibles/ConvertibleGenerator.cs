using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.SourceGenerators.Convertibles
{
    [Generator]
    public class ConvertibleGenerator : ISourceGenerator
    {
        private const string attributeText = @"
using System;
namespace Convertible
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    [System.Diagnostics.Conditional(""ConvertibleGenerator_DEBUG"")]
    sealed class ConvertibleAttribute : Attribute
    {
        public ConvertibleAttribute()
        {
        }
    }
}
";
        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxContextReceiver is ClassFilter classFilter))
                return;

            foreach (var classSymbol in classFilter.Types)
            {
                var typeArgument = classSymbol.BaseType.TypeArguments.FirstOrDefault();
                if (typeArgument == null) continue;

                var text = new SourceGenerator(classSymbol, typeArgument).GetSource();
                context.AddSource($"{classSymbol.Name}.g.cs", SourceText.From(text, Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForPostInitialization((i) => i.AddSource("ConvertibleAttribute.g.cs", attributeText));
            context.RegisterForSyntaxNotifications(() => new ClassFilter());
        }
    }
}
