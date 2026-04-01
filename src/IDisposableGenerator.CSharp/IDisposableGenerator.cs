namespace IDisposableGenerator;

[Generator]
public class IDisposableGeneratorCS : IIncrementalGenerator
{
    // on MacOS add "SpinWait.SpinUntil(() => Debugger.IsAttached);" to debug in rider.
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var workItemCollection = context.CompilationProvider.Select(
            static (c, _) => new WorkItemCollection(new CSharpGeneratedCodeWriter(((CSharpCompilation)c).LanguageVersion)));
        context.RegisterCommonSourceOutput<ClassDeclarationSyntax>(workItemCollection);
        context.RegisterPostInitializationOutput(static ctx =>
        {
            // Always generate the attributes.
            var attributeSource = new StringBuilder();
            _ = attributeSource.Append(Properties.Resources.AttributeCodeCSharp);
            attributeSource.ToSourceFile("GeneratedAttributes.g.cs", ref ctx);
        });
    }
}
