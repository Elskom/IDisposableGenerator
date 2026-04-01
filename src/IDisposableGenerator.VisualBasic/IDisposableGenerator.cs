namespace IDisposableGenerator;

[Generator(LanguageNames.VisualBasic)]
public class IDisposableGeneratorVB : IIncrementalGenerator
{
    // on MacOS add "SpinWait.SpinUntil(() => Debugger.IsAttached);" to debug in rider.
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var workItemCollection = context.CompilationProvider.Select(
            static (_, _) => new WorkItemCollection(new VisualBasicGeneratedCodeWriter()));
        context.RegisterCommonSourceOutput<ClassStatementSyntax>(workItemCollection);
        context.RegisterPostInitializationOutput(static ctx =>
        {
            // Always generate the attributes.
            var attributeSource = new StringBuilder();
            _ = attributeSource.Append(Properties.Resources.AttributeCodeVisualBasic);
            attributeSource.ToSourceFile("GeneratedAttributes.g.vb", ref ctx);
        });
    }
}
