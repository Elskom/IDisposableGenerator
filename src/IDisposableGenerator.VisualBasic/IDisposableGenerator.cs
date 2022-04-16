namespace IDisposableGenerator;

[Generator(LanguageNames.VisualBasic)]
public class IDisposableGeneratorVB : IIncrementalGenerator
{
    // on MacOS add "SpinWait.SpinUntil(() => Debugger.IsAttached);" to debug in rider.
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var workItemCollection = context.CompilationProvider.Select(
            static (c, _) => new WorkItemCollection(c));
        var workItems = context.SyntaxProvider.CreateSyntaxProvider(
            static (n, _) => n is ClassBlockSyntax,
            static (n, ct) => (INamedTypeSymbol)n.SemanticModel.GetDeclaredSymbol(n.Node, ct)!
            ).Combine(workItemCollection).Select(
            static (testClass, ct) =>
            {
                testClass.Right.Process(testClass.Left, ct);
                return true;
            });
        var combined = workItems.Collect().Combine(workItemCollection);
        context.RegisterSourceOutput(combined, (ctx, items) =>
        {
            // begin creating the source we'll inject into the users compilation
            DisposableCodeWriter.WriteDisposableCodeVisualBasic(
                items.Right,
                ref ctx);
        });
        context.RegisterPostInitializationOutput(ctx =>
        {
            // Always generate the attributes.
            var attributeSource = Properties.Resources.AttributeCodeVisualBasic!;
            attributeSource.ToSourceFile("GeneratedAttributes.g.vb", ref ctx);
        });
    }
}
