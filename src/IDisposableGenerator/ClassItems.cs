namespace IDisposableGenerator;

internal class ClassItems
{
    public string? Name { get; set; }
    public Accessibility Accessibility { get; set; }
    public bool Stream { get; set; }
    public List<string> Owns { get; } = new();
    public List<string> Fields { get; } = new();
    public List<string> SetNull { get ; } = new();
    public List<string> Methods { get; } = new();

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
        return $@"Class: Name {(
            this.Name)}, Accessibility: {(
            this.Accessibility)}, Stream: {(
            this.Stream)}, Owns Count: {(
            this.Owns.Count)}, Fields Count: {(
            this.Fields.Count)}, SetNull Count: {(
            this.SetNull.Count)}, Methods Count: {(
            this.Methods.Count)}";
    }
}
