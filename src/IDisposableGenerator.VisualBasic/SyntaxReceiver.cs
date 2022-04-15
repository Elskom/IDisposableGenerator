namespace IDisposableGenerator;

/// <summary>
/// This is used to process the syntax tree. The output is "work items", which are fed into the code generators.
/// </summary>
/// <remarks>
/// Created on demand before each generation pass
/// </remarks>
internal class SyntaxReceiver : ISyntaxContextReceiver
{
    public WorkItemCollection WorkItemCollection { get; } = new();

    /// <summary>
    /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
    /// </summary>
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        // any field with at least one attribute is a candidate for property generation
        if (context.Node is not ClassBlockSyntax classDeclarationSyntax)
        {
            return;
        }

        var testClass = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax)!;
        WorkItemCollection.Process(testClass, CancellationToken.None);
    }
}
