namespace IDisposableGenerator;

internal class ClassItems
{
    public string? Name { get; set; }
    public Accessibility Accessibility { get; set; }
    public bool Stream { get; set; }
    public bool WithoutThrowIfDisposed { get; set; }
    public List<ISymbol> Owns { get; } = [];
    public List<ISymbol> Fields { get; } = [];
    public List<string> SetNull { get; } = [];
    public List<string> Methods { get; } = [];

    public static bool IsReadOnlyField(ISymbol member)
        => (member is IFieldSymbol fieldSymbol && fieldSymbol.IsReadOnly) ||
           (member is IPropertySymbol propertySymbol && propertySymbol.IsReadOnly);

    public bool AddSetNull(ISymbol member)
    {
        if (!IsReadOnlyField(member) || member is IEventSymbol)
        {
            this.SetNull.Add(member.Name);
        }

        return true;
    }

    public bool AddMethod(ISymbol member)
    {
        this.Methods.Add(member.Name);
        return true;
    }

    public bool AddField(TypedConstant arg, ISymbol member)
    {
        if ((bool)arg.Value!)
        {
            this.Owns.Add(member);
        }
        else
        {
            this.Fields.Add(member);
        }

        return true;
    }

    public bool NameEquals(string name)
        => this.Name!.Equals(name, StringComparison.Ordinal);

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        var result = new StringBuilder();
        _ = result.Append($"Class: Name {this.Name}")
            .Append($", Accessibility: {this.Accessibility}")
            .Append($", Stream: {this.Stream}")
            .Append($", Without ThrowIfDisposed: {this.WithoutThrowIfDisposed}")
            .Append($", Owns Count: {this.Owns.Count}")
            .Append($", Fields Count: {this.Fields.Count}")
            .Append($", SetNull Count: {this.SetNull.Count}")
            .Append($", Methods Count: {this.Methods.Count}");
        return result.ToString();
    }

    public string ToCSharp9CodeString()
    {
        var result = new StringBuilder();
        _ = result.Append($@"
    {(this.Accessibility == Accessibility.Public ? "public" : "internal")} partial class {this.Name}{(!this.Stream ? " : IDisposable" : "")}
    {{
        private bool isDisposed;
");
        if (this.Owns.Count is not 0)
        {
            _ = result.Append(this.Stream ? @"
        internal bool KeepOpen { get; }
" : @"
        internal bool IsOwned { get; set; }
");
        }

        _ = result.Append($@"
        {(!this.Stream ? $@"/// <summary>
        /// Cleans up the resources used by <see cref=""{this.Name}""/>.
        /// </summary>
        public void Dispose()
        {{
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }}

        private" : @"/// <inheritdoc/>
        protected override")} void Dispose(bool disposing)
        {{
            if (!this.isDisposed && disposing)
            {{
");
        if (this.Methods.Count is not 0)
        {
            foreach (var methodItem in this.Methods)
            {
                _ = result.Append($@"                this.{methodItem}();
");
            }
        }

        if (this.Owns.Count is not 0)
        {
            _ = result.Append($@"                if ({(this.Stream ? "!this.KeepOpen" : "this.IsOwned")})
                {{
");
            foreach (var ownedItem in this.Owns)
            {
                // automatically set to null after Dispose().
                _ = result.Append($@"                    this.{ownedItem.Name}?.Dispose();
{(!IsReadOnlyField(ownedItem) ? $@"                    this.{ownedItem.Name} = null;
" : string.Empty)}");
            }

            _ = result.Append(@"                }
");
        }

        if (this.Fields.Count is not 0)
        {
            foreach (var fieldItem in this.Fields)
            {
                // automatically set to null after Dispose().
                _ = result.Append($@"                this.{fieldItem.Name}?.Dispose();
{(!IsReadOnlyField(fieldItem) ? $@"                this.{fieldItem.Name} = null;
" : string.Empty)}");
            }
        }

        if (this.SetNull.Count is not 0)
        {
            foreach (var nullItem in this.SetNull)
            {
                _ = result.Append($@"                this.{nullItem} = null;
");
            }
        }

        _ = result.Append(@"                this.isDisposed = true;
            }
");
        if (this.Stream)
        {
            _ = result.Append(@"
            // On Streams call base.Dispose(disposing)!!!
            base.Dispose(disposing);
");
        }

        _ = result.Append(@"        }
");
        if (!this.WithoutThrowIfDisposed)
        {
            _ = result.Append($@"
        internal void ThrowIfDisposed()
        {{
            if (this.isDisposed)
            {{
                throw new ObjectDisposedException(nameof({this.Name}));
            }}
        }}
");
        }

        _ = result.Append(@"    }
");

        return result.ToString();
    }

    public string ToCSharp10CodeString()
    {
        var result = new StringBuilder();
        _ = result.Append($@"
{(this.Accessibility == Accessibility.Public ? "public" : "internal")} partial class {this.Name}{(!this.Stream ? " : IDisposable" : "")}
{{
    private bool isDisposed;
");
        if (this.Owns.Count is not 0)
        {
            _ = result.Append(this.Stream ? @"
    internal bool KeepOpen { get; }
" : @"
    internal bool IsOwned { get; set; }
");
        }

        _ = result.Append($@"
    {(!this.Stream ? $@"/// <summary>
    /// Cleans up the resources used by <see cref=""{this.Name}""/>.
    /// </summary>
    public void Dispose()
    {{
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }}

    private" : @"/// <inheritdoc/>
    protected override")} void Dispose(bool disposing)
    {{
        if (!this.isDisposed && disposing)
        {{
");
        if (this.Methods.Count is not 0)
        {
            foreach (var methodItem in this.Methods)
            {
                _ = result.Append($@"            this.{methodItem}();
");
            }
        }

        if (this.Owns.Count is not 0)
        {
            _ = result.Append($@"            if ({(this.Stream ? "!this.KeepOpen" : "this.IsOwned")})
            {{
");
            foreach (var ownedItem in this.Owns)
            {
                // automatically set to null after Dispose().
                _ = result.Append($@"                this.{ownedItem.Name}?.Dispose();
{(!IsReadOnlyField(ownedItem) ? $@"                this.{ownedItem.Name} = null;
" : string.Empty)}");
            }

            _ = result.Append(@"            }
");
        }

        if (this.Fields.Count is not 0)
        {
            foreach (var fieldItem in this.Fields)
            {
                // automatically set to null after Dispose().
                _ = result.Append($@"            this.{fieldItem.Name}?.Dispose();
{(!IsReadOnlyField(fieldItem) ? $@"            this.{fieldItem.Name} = null;
" : string.Empty)}");
            }
        }

        if (this.SetNull.Count is not 0)
        {
            foreach (var nullItem in this.SetNull)
            {
                _ = result.Append($@"            this.{nullItem} = null;
");
            }
        }

        _ = result.Append(@"            this.isDisposed = true;
        }
");
        if (this.Stream)
        {
            _ = result.Append(@"
        // On Streams call base.Dispose(disposing)!!!
        base.Dispose(disposing);
");
        }

        _ = result.Append("""
                        }

                    """);

        if (!this.WithoutThrowIfDisposed)
        {
            _ = result.Append($@"
    internal void ThrowIfDisposed()
    {{
        if (this.isDisposed)
        {{
            throw new ObjectDisposedException(nameof({this.Name}));
        }}
    }}
");
        }

        _ = result.Append(@"}
");
        return result.ToString();
    }

    public string ToVisualBasicCodeString()
    {
        var result = new StringBuilder();
        _ = result.Append($@"
    {(this.Accessibility == Accessibility.Public ? "Public" : "Friend")} Partial Class {this.Name}{(!this.Stream ? @"
        Implements IDisposable
" : "")}
        Private isDisposed As Boolean
");
        if (this.Owns.Count is not 0)
        {
            _ = result.Append(this.Stream ? @"
        Friend ReadOnly Property KeepOpen As Boolean
" : @"
        Friend Property IsOwned As Boolean
");
        }

        _ = result.Append($@"
        {(!this.Stream ? $@"''' <summary>
        ''' Cleans up the resources used by <see cref=""{this.Name}""/>.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            Me.Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Private" : @"''' <inheritdoc/>
        Protected Overrides")} Sub Dispose(ByVal disposing As Boolean)
            If Not Me.isDisposed AndAlso disposing Then
");
        if (this.Methods.Count is not 0)
        {
            foreach (var methodItem in this.Methods)
            {
                _ = result.Append($@"                Me.{methodItem}()
");
            }
        }

        if (this.Owns.Count is not 0)
        {
            _ = result.Append($@"                If {(this.Stream ? "Not Me.KeepOpen" : "Me.IsOwned")} Then
");
            foreach (var ownedItem in this.Owns)
            {
                // automatically set to null after Dispose().
                _ = result.Append($@"                    Me.{ownedItem.Name}?.Dispose()
{(!IsReadOnlyField(ownedItem) ? $@"                    Me.{ownedItem.Name} = Nothing
" : string.Empty)}");
            }

            _ = result.Append(@"                End If
");
        }

        if (this.Fields.Count is not 0)
        {
            foreach (var fieldItem in this.Fields)
            {
                // automatically set to null after Dispose().
                _ = result.Append($@"                Me.{fieldItem.Name}?.Dispose()
{(!IsReadOnlyField(fieldItem) ? $@"                Me.{fieldItem.Name} = Nothing
" : string.Empty)}");
            }
        }

        if (this.SetNull.Count is not 0)
        {
            foreach (var nullItem in this.SetNull)
            {
                _ = result.Append($@"                Me.{nullItem} = Nothing
");
            }
        }

        _ = result.Append(@"                Me.isDisposed = True
            End If
");
        if (this.Stream)
        {
            _ = result.Append(@"
            ' On Streams call MyBase.Dispose(disposing)!!!
            MyBase.Dispose(disposing)
");
        }

        _ = result.Append("""
                            End Sub

                    """);

        if (!this.WithoutThrowIfDisposed)
        {
            _ = result.Append($$"""

                                Friend Sub ThrowIfDisposed()
                                    If Me.isDisposed Then
                                        Throw New ObjectDisposedException(NameOf({{this.Name}}))
                                    End If
                                End Sub

                        """);
        }

        _ = result.Append("""
                        End Class

                    """);
        return result.ToString();
    }
}
