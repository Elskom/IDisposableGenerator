namespace IDisposableGenerator;

internal class ClassItems
{
    public string? Name { get; set; }
    public Accessibility Accessibility { get; set; }
    public bool Stream { get; set; }
    public bool ThrowIfDisposed { get; set; }
    public List<string> Owns { get; } = [];
    public List<string> Fields { get; } = [];
    public List<string> SetNull { get; } = [];
    public List<string> Methods { get; } = [];

    public bool AddSetNull(ISymbol member)
    {
        this.SetNull.Add(member.Name);
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
            this.Owns.Add(member.Name);
        }
        else
        {
            this.Fields.Add(member.Name);
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
            .Append($", ThrowIfDisposed: {this.ThrowIfDisposed}")
            .Append($", Owns Count: {this.Owns.Count}")
            .Append($", Fields Count: {this.Fields.Count}")
            .Append($", SetNull Count: {this.SetNull.Count}")
            .Append($", Methods Count: {this.Methods.Count}");
        return result.ToString();
    }
}
