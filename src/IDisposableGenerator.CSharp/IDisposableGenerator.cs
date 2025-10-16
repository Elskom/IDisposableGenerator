namespace IDisposableGenerator;

using System.Text;

[Generator]
public class IDisposableGenerator : IIncrementalGenerator
{
    private delegate void WriteDisposableCode(
        WorkItemCollection workItemCollection,
        ref SourceProductionContext context);

    // on MacOS add "SpinWait.SpinUntil(() => Debugger.IsAttached);" to debug in rider.
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var workItemCollection = context.CompilationProvider.Select(
            static (c, _) => new WorkItemCollection(c));
        var workItems = context.SyntaxProvider.CreateSyntaxProvider(
            static (n, _) => n is ClassDeclarationSyntax,
            (n, ct) => (INamedTypeSymbol)n.SemanticModel.GetDeclaredSymbol(n.Node, ct)!
            ).Combine(workItemCollection).Select(
            static (testClass, ct) =>
            {
                testClass.Right.Process(testClass.Left, ct);
                return true;
            });
        var combined = workItems.Collect().Combine(workItemCollection);
        context.RegisterSourceOutput(combined, (ctx, items) =>
        {
            // NOTE: for debugging the tests.
            // var workItemQuery =
            //     from item in items.Right.GetWorkItems()
            //     where items.Right.Count > 1
            //     select (WorkItem: item, Index: items.Right.IndexOf(item));
            // foreach (var (workItem, index) in workItemQuery)
            // {
            //     Console.WriteLine($"Work Item {index}: {workItem}");
            // }

            WriteDisposableCode writeDisposableCode =
                ((CSharpCompilation)items.Right.Compilation).LanguageVersion > LanguageVersion.CSharp9
                ? DisposableCodeWriter.WriteDisposableCodeCSharp10
                : DisposableCodeWriter.WriteDisposableCodeCSharp9;

            // TODO: Find some way to ensure if the 0th index is actually the right one all the time.
            writeDisposableCode(items.Right, ref ctx);
        });
        context.RegisterPostInitializationOutput(ctx =>
        {
            // Always generate the attributes.
            var attributeSource = new StringBuilder();
            _ = attributeSource.Append(Properties.Resources.AttributeCodeCSharp!);
            attributeSource.ToSourceFile("GeneratedAttributes.g.cs", ref ctx);
        });
    }
}
