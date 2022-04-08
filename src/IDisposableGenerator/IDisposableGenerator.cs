namespace IDisposableGenerator;

[Generator]
public class IDisposableGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        // retrieve the populated receiver
        var receiver = (context.SyntaxContextReceiver as SyntaxReceiver)!;

        // begin creating the source we'll inject into the users compilation
        var sourceBuilder = DisposableCodeWriter.WriteDisposableCodeCSharp9(receiver.WorkItem);

        // inject the created source into the users compilation
        sourceBuilder.ToString().ToSourceFile("Disposables.g.cs", ref context);
    }

    // on MacOS add "SpinWait.SpinUntil(() => Debugger.IsAttached);" to debug in rider.
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        context.RegisterForPostInitialization(ctx =>
        {
            // Always generate the attributes.
            var attributeSource = Properties.Resources.AttributeCodeCSharp9!;
            attributeSource.ToSourceFile("GeneratedAttributes.g.cs", ref ctx);
        });
    }
}
