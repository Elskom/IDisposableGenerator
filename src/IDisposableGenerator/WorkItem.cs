namespace IDisposableGenerator;

internal class WorkItem
{
    public string Namespace { get; set; } = null!;
    public List<ClassItems> Classes { get; } = new();

    public ClassItems? GetClassItems(INamedTypeSymbol testClass)
        => this.Classes.FirstOrDefault(
            classItem => classItem.NameEquals(testClass.Name));

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        var sb = new StringBuilder($"Namespace: Name: {this.Namespace}");
        foreach (var classItems in this.Classes)
        {
            sb.AppendLine();
            sb.Append($"Class Item {this.Classes.IndexOf(classItems)}: {classItems}");
        }

        return sb.ToString();
    }
}
