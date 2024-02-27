namespace IDisposableGenerator;

internal static class DisposableCodeWriter
{
    public static void WriteDisposableCodeVisualBasic(
        WorkItemCollection workItemCollection,
        ref SourceProductionContext context)
    {
        StringBuilder sourceBuilder = new("' <autogenerated/>");
        _ = sourceBuilder.Append(@"
Imports System
");
        foreach (var workItem in workItemCollection.GetWorkItems())
        {
            _ = sourceBuilder.Append($@"
Namespace {workItem.Namespace}
");
            foreach (var classItem in workItem.Classes)
            {
                _ = sourceBuilder.Append($@"
    {(classItem.Accessibility == Accessibility.Public ? "Public" : "Friend")} Partial Class {classItem.Name}{(!classItem.Stream ? @"
        Implements IDisposable
" : "")}
        Private isDisposed As Boolean
");

                if (classItem.Owns.Count is not 0)
                {
                    _ = sourceBuilder.Append(classItem.Stream ? @"
        Friend ReadOnly Property KeepOpen As Boolean
" : @"
        Friend Property IsOwned As Boolean
");
                }

                _ = sourceBuilder.Append($@"
        {(!classItem.Stream ? $@"''' <summary>
        ''' Cleans up the resources used by <see cref=""{classItem.Name}""/>.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            Me.Dispose(True)
        End Sub

        Private" : @"''' <inheritdoc/>
        Protected Overrides")} Sub Dispose(ByVal disposing As Boolean)
            If Not Me.isDisposed AndAlso disposing Then
");
                if (classItem.Methods.Count is not 0)
                {
                    foreach (var methodItem in classItem.Methods)
                    {
                        _ = sourceBuilder.Append($@"                Me.{methodItem}()
");
                    }
                }

                if (classItem.Owns.Count is not 0)
                {
                    _ = sourceBuilder.Append($@"                If {(classItem.Stream ? "Not Me.KeepOpen" : "Me.IsOwned")} Then
");
                    foreach (var ownedItem in classItem.Owns)
                    {
                        // automatically set to null after Dispose().
                        _ = sourceBuilder.Append($@"                    Me.{ownedItem}?.Dispose()
                    Me.{ownedItem} = Nothing
");
                    }

                    _ = sourceBuilder.Append(@"                End If
");
                }

                if (classItem.Fields.Count is not 0)
                {
                    foreach (var fieldItem in classItem.Fields)
                    {
                        // automatically set to null after Dispose().
                        _ = sourceBuilder.Append($@"                Me.{fieldItem}?.Dispose()
                Me.{fieldItem} = Nothing
");
                    }
                }

                if (classItem.SetNull.Count is not 0)
                {
                    foreach (var nullItem in classItem.SetNull)
                    {
                        _ = sourceBuilder.Append($@"                Me.{nullItem} = Nothing
");
                    }
                }

                _ = sourceBuilder.Append(@"                Me.isDisposed = True
            End If
");
                if (classItem.Stream)
                {
                    _ = sourceBuilder.Append(@"
            ' On Streams call MyBase.Dispose(disposing)!!!
            MyBase.Dispose(disposing)
");
                }

                _ = sourceBuilder.Append("""
                            End Sub

                    """);

                if (!classItem.WithoutThrowIfDisposed)
                {
                    _ = sourceBuilder.Append($$"""

                                Friend Sub ThrowIfDisposed()
                                    If Me.isDisposed Then
                                        Throw New ObjectDisposedException(NameOf({{classItem.Name}}))
                                    End If
                                End Sub

                        """);
                }

                _ = sourceBuilder.Append("""
                        End Class

                    """);
            }

            _ = sourceBuilder.Append(@"End Namespace
");
        }

        // inject the created sources into the users compilation.
        sourceBuilder.ToString().ToSourceFile("Disposables.g.vb", ref context);
    }

    public static void WriteDisposableCodeCSharp10(
        WorkItemCollection workItemCollection,
        ref SourceProductionContext context)
    {
        foreach (var workItem in workItemCollection.GetWorkItems())
        {
            StringBuilder sourceBuilder = new("// <autogenerated/>");
            _ = sourceBuilder.Append($@"
namespace {workItem.Namespace};
");
            foreach (var classItem in workItem.Classes)
            {
                _ = sourceBuilder.Append($@"
{(classItem.Accessibility == Accessibility.Public ? "public" : "internal")} partial class {classItem.Name}{(!classItem.Stream ? " : IDisposable" : "")}
{{
    private bool isDisposed;
");
                if (classItem.Owns.Count is not 0)
                {
                    _ = sourceBuilder.Append(classItem.Stream ? @"
    internal bool KeepOpen { get; }
" : @"
    internal bool IsOwned { get; set; }
");
                }

                _ = sourceBuilder.Append($@"
    {(!classItem.Stream ? $@"/// <summary>
    /// Cleans up the resources used by <see cref=""{classItem.Name}""/>.
    /// </summary>
    public void Dispose() => this.Dispose(true);

    private" : @"/// <inheritdoc/>
    protected override")} void Dispose(bool disposing)
    {{
        if (!this.isDisposed && disposing)
        {{
");
                if (classItem.Methods.Count is not 0)
                {
                    foreach (var methodItem in classItem.Methods)
                    {
                        _ = sourceBuilder.Append($@"            this.{methodItem}();
");
                    }
                }

                if (classItem.Owns.Count is not 0)
                {
                    _ = sourceBuilder.Append($@"            if ({(classItem.Stream ? "!this.KeepOpen" : "this.IsOwned")})
            {{
");
                    foreach (var ownedItem in classItem.Owns)
                    {
                        // automatically set to null after Dispose().
                        _ = sourceBuilder.Append($@"                this.{ownedItem}?.Dispose();
                this.{ownedItem} = null;
");
                    }

                    _ = sourceBuilder.Append(@"            }
");
                }

                if (classItem.Fields.Count is not 0)
                {
                    foreach (var fieldItem in classItem.Fields)
                    {
                        // automatically set to null after Dispose().
                        _ = sourceBuilder.Append($@"            this.{fieldItem}?.Dispose();
            this.{fieldItem} = null;
");
                    }
                }

                if (classItem.SetNull.Count is not 0)
                {
                    foreach (var nullItem in classItem.SetNull)
                    {
                        _ = sourceBuilder.Append($@"            this.{nullItem} = null;
");
                    }
                }

                _ = sourceBuilder.Append(@"            this.isDisposed = true;
        }
");
                if (classItem.Stream)
                {
                    _ = sourceBuilder.Append(@"
        // On Streams call base.Dispose(disposing)!!!
        base.Dispose(disposing);
");
                }

                _ = sourceBuilder.Append("""
                        }

                    """);

                if (!classItem.WithoutThrowIfDisposed)
                {
                    _ = sourceBuilder.Append($$"""

                            internal void ThrowIfDisposed()
                            {
                                if (this.isDisposed)
                                {
                                    throw new ObjectDisposedException(nameof({{classItem.Name}}));
                                }
                            }

                        """);
                }

                _ = sourceBuilder.Append("""
                    }

                    """);
            }

            // inject the created sources into the users compilation.
            sourceBuilder.ToString().ToSourceFile($@"Disposables{(
                workItemCollection.Count > 1
                    ? $".{workItemCollection.IndexOf(workItem)}" :
                    string.Empty)}.g.cs", ref context);
        }
    }

    public static void WriteDisposableCodeCSharp9(
        WorkItemCollection workItemCollection,
        ref SourceProductionContext context)
    {
        StringBuilder sourceBuilder = new("// <autogenerated/>");
        foreach (var workItem in workItemCollection.GetWorkItems())
        {
            _ = sourceBuilder.Append($@"
namespace {workItem.Namespace}
{{
    using global::System;
");
            foreach (var classItem in workItem.Classes)
            {
                _ = sourceBuilder.Append($@"
    {(classItem.Accessibility == Accessibility.Public ? "public" : "internal")} partial class {classItem.Name}{(!classItem.Stream ? " : IDisposable" : "")}
    {{
        private bool isDisposed;
");
                if (classItem.Owns.Count is not 0)
                {
                    _ = sourceBuilder.Append(classItem.Stream ? @"
        internal bool KeepOpen { get; }
" : @"
        internal bool IsOwned { get; set; }
");
                }

                _ = sourceBuilder.Append($@"
        {(!classItem.Stream ? $@"/// <summary>
        /// Cleans up the resources used by <see cref=""{classItem.Name}""/>.
        /// </summary>
        public void Dispose() => this.Dispose(true);

        private" : @"/// <inheritdoc/>
        protected override")} void Dispose(bool disposing)
        {{
            if (!this.isDisposed && disposing)
            {{
");
                if (classItem.Methods.Count is not 0)
                {
                    foreach (var methodItem in classItem.Methods)
                    {
                        _ = sourceBuilder.Append($@"                this.{methodItem}();
");
                    }
                }

                if (classItem.Owns.Count is not 0)
                {
                    _ = sourceBuilder.Append($@"                if ({(classItem.Stream ? "!this.KeepOpen" : "this.IsOwned")})
                {{
");
                    foreach (var ownedItem in classItem.Owns)
                    {
                        // automatically set to null after Dispose().
                        _ = sourceBuilder.Append($@"                    this.{ownedItem}?.Dispose();
                    this.{ownedItem} = null;
");
                    }

                    _ = sourceBuilder.Append(@"                }
");
                }

                if (classItem.Fields.Count is not 0)
                {
                    foreach (var fieldItem in classItem.Fields)
                    {
                        // automatically set to null after Dispose().
                        _ = sourceBuilder.Append($@"                this.{fieldItem}?.Dispose();
                this.{fieldItem} = null;
");
                    }
                }

                if (classItem.SetNull.Count is not 0)
                {
                    foreach (var nullItem in classItem.SetNull)
                    {
                        _ = sourceBuilder.Append($@"                this.{nullItem} = null;
");
                    }
                }

                _ = sourceBuilder.Append(@"                this.isDisposed = true;
            }
");
                if (classItem.Stream)
                {
                    _ = sourceBuilder.Append(@"
            // On Streams call base.Dispose(disposing)!!!
            base.Dispose(disposing);
");
                }

                _ = sourceBuilder.Append("""
                            }

                    """);

                if (!classItem.WithoutThrowIfDisposed)
                {
                    _ = sourceBuilder.Append($$"""

                                internal void ThrowIfDisposed()
                                {
                                    if (this.isDisposed)
                                    {
                                        throw new ObjectDisposedException(nameof({{classItem.Name}}));
                                    }
                                }

                        """);
                }

                _ = sourceBuilder.Append("""
                        }

                    """);
            }

            _ = sourceBuilder.Append("""
                    }

                    """);
        }

        // inject the created source into the users compilation.
        sourceBuilder.ToString().ToSourceFile("Disposables.g.cs", ref context);
    }
}
