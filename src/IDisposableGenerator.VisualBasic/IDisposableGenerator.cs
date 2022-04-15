namespace IDisposableGenerator;

[Generator(LanguageNames.VisualBasic)]
public class IDisposableGeneratorVB : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        // retrieve the populated receiver
        var receiver = (context.SyntaxContextReceiver as SyntaxReceiver)!;

        // begin creating the source we'll inject into the users compilation
        DisposableCodeWriter.WriteDisposableCodeVisualBasic(
            receiver.WorkItemCollection,
            ref context);
    }

    // on MacOS add "SpinWait.SpinUntil(() => Debugger.IsAttached);" to debug in rider.
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        context.RegisterForPostInitialization(ctx =>
        {
            // Always generate the attributes.
            var attributeSource = Properties.Resources.AttributeCodeVisualBasic!;
            attributeSource.ToSourceFile("GeneratedAttributes.g.vb", ref ctx);
        });
    }
}
