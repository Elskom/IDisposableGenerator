namespace IDisposibleGenerator
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// This is used to process the syntax tree. The output is "work items", which are fed into the code generators.
    /// </summary>
    /// <remarks>
    /// Created on demand before each generation pass
    /// </remarks>
    internal class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<WorkItem> WorkItems { get; } = new();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            // any field with at least one attribute is a candidate for property generation
            if (context.Node is ClassDeclarationSyntax classDeclarationSyntax)
            {
                var testClass = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax)!;
                var members = testClass.GetMembers();
                foreach (var att in testClass.GetAttributes())
                {
                    if (!att.AttributeClass!.Name.Equals("GenerateDisposeAttribute") &&
                        !att.AttributeClass.FullNamespace().Equals("IDisposableGenerator"))
                    {
                        continue;
                    }

                    WorkItem workItem = new()
                    {
                        Namespace = testClass.FullNamespace(),
                        Classes = { GetClassItem(att, testClass) },
                    };
                    this.WorkItems.Add(workItem);
                }

                foreach (var (member, attr) in members.SelectMany(member => member.GetAttributes().Select(attr => (member, attr))))
                {
                    this.CheckAttributesOnMember(attr, member, testClass);
                }
            }
        }

        private static ClassItems GetClassItem(AttributeData att, INamedTypeSymbol testClass)
        {
            ClassItems classItems = new()
            {
                Name = testClass.Name,
                Accessibility = testClass.DeclaredAccessibility,
            };

            foreach (var arg in att.ConstructorArguments)
            {
                classItems.Stream = (bool)arg.Value!;
            }

            return classItems;
        }

        private void CheckAttributesOnMember(AttributeData? attr, ISymbol member, INamedTypeSymbol testClass)
        {
            switch (attr!.AttributeClass!.Name)
            {
                case "DisposeFieldAttribute" when attr.AttributeClass.FullNamespace().Equals("IDisposableGenerator", System.StringComparison.Ordinal):
                {
                    foreach (var (classItem, arg) in this.WorkItems.SelectMany(
                        item => item.Classes.Where(classItem => classItem.Name!.Equals(testClass.Name, System.StringComparison.Ordinal))).SelectMany(
                        classItem => attr.ConstructorArguments.TakeWhile(_ => attr.ConstructorArguments.Length <= 1).Select(arg => (classItem, arg))))
                    {
                        if ((bool)arg.Value!)
                        {
                            classItem.Owns.Add(member.Name);
                        }
                        else
                        {
                            classItem.Fields.Add(member.Name);
                        }
                    }

                    break;
                }
                case "SetNullOnDisposeAttribute" when attr.AttributeClass.FullNamespace().Equals("IDisposableGenerator", System.StringComparison.Ordinal):
                {
                    foreach (var classItem in this.WorkItems.SelectMany(
                        item => item.Classes.Where(classItem => classItem.Name!.Equals(testClass.Name, System.StringComparison.Ordinal))))
                    {
                        classItem.SetNull.Add(member.Name);
                    }

                    break;
                }
                case "CallOnDisposeAttribute" when attr.AttributeClass.FullNamespace().Equals("IDisposableGenerator", System.StringComparison.Ordinal):
                {
                    foreach (var classItem in this.WorkItems.SelectMany(
                        item => item.Classes.Where(classItem => classItem.Name!.Equals(testClass.Name, System.StringComparison.Ordinal))))
                    {
                        classItem.Methods.Add(member.Name);
                    }

                    break;
                }
            }
        }
    }
}
