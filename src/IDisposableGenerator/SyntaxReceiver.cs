namespace IDisposableGenerator;

/// <summary>
/// This is used to process the syntax tree. The output is "work items", which are fed into the code generators.
/// </summary>
/// <remarks>
/// Created on demand before each generation pass
/// </remarks>
internal class SyntaxReceiver : ISyntaxContextReceiver
{
    public WorkItem WorkItem { get; } = new();

    /// <summary>
    /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
    /// </summary>
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        // any field with at least one attribute is a candidate for property generation
        if (context.Node is not ClassDeclarationSyntax classDeclarationSyntax)
        {
            return;
        }

        var testClass = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax)!;
        
        // Avoid a bug that would set namespace to "IDisposableGenerator"
        // instead of the namespace that the WorkItem's classes are in.
        if (string.IsNullOrEmpty(this.WorkItem.Namespace)
            && !testClass.FullNamespaceEquals("IDisposableGenerator"))
        {
            this.WorkItem.Namespace = testClass.FullNamespace();
        }
        
        var classItemsQuery =
            from att in testClass.GetAttributes()
            where att.AttributeClass!.Name switch
            {
                "GenerateDisposeAttribute" => true,
                _ => false,
            }
            select GetClassItem(att, testClass);
        var memberQuery =
            from member in testClass.GetMembers()
            select member;
        foreach (var classItem in classItemsQuery)
        {
            this.WorkItem.Classes.Add(classItem);
        }
        
        foreach (var member in memberQuery)
        {
            this.CheckAttributesOnMember(member, testClass);
        }
    }

    private static ClassItems GetClassItem(AttributeData attr, INamedTypeSymbol testClass)
    {
        var result = new ClassItems
        {
            Name = testClass.Name,
            Accessibility = testClass.DeclaredAccessibility,
            Stream = (bool)attr.ConstructorArguments[0].Value!,
        };

        return result;
    }

    private void CheckAttributesOnMember(ISymbol member, INamedTypeSymbol testClass)
    {
        var classItem = this.WorkItem.GetClassItems(testClass)!;
        foreach (var attr in member.GetAttributes())
        {
            _ = attr!.AttributeClass!.Name switch
            {
                "DisposeFieldAttribute" => classItem.AddField(attr.ConstructorArguments[0], member),
                "SetNullOnDisposeAttribute" => classItem.AddSetNull(member),
                "CallOnDisposeAttribute" => classItem.AddMethod(member),

                // cannot throw here because the attribute in this case should be ignored.
                _ => false,
            };
        }
    }
}
