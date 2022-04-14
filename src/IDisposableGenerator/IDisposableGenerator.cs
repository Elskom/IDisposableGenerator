namespace IDisposableGenerator;

[Generator]
public class IDisposableGenerator : ISourceGenerator
{
    private delegate void WriteDisposableCode(
        WorkItemCollection workItemCollection,
        ref GeneratorExecutionContext context);

    public void Execute(GeneratorExecutionContext context)
    {
        // retrieve the populated receiver
        var receiver = (context.SyntaxContextReceiver as SyntaxReceiver)!;
        var compilation = (context.Compilation as CSharpCompilation)!;

        // begin creating the source we'll inject into the users compilation
        WriteDisposableCode writeDisposableCode = compilation.LanguageVersion > LanguageVersion.CSharp9
            ? DisposableCodeWriter.WriteDisposableCodeCSharp10
            : DisposableCodeWriter.WriteDisposableCodeCSharp9;
        writeDisposableCode(receiver.WorkItemCollection, ref context);
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
