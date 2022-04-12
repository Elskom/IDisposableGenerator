namespace IDisposableGenerator;

[Generator]
public class IDisposableGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        // retrieve the populated receiver
        var receiver = (context.SyntaxContextReceiver as SyntaxReceiver)!;
        var compilation = (context.Compilation as CSharpCompilation)!;

        // begin creating the source we'll inject into the users compilation
        _ = compilation.LanguageVersion is LanguageVersion.CSharp10
            or LanguageVersion.Latest or LanguageVersion.Preview
            ? DisposableCodeWriter.WriteDisposableCodeCSharp10(
                receiver.WorkItemCollection,
                ref context)
            : DisposableCodeWriter.WriteDisposableCodeCSharp9(
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
            var attributeSource = Properties.Resources.AttributeCodeCSharp!;
            attributeSource.ToSourceFile("GeneratedAttributes.g.cs", ref ctx);
        });
    }
}
