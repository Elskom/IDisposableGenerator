namespace IDisposableGenerator;

internal class WorkItem
{
    public string? Namespace { get; set; }
    public List<ClassItems> Classes { get; } = new();

    public ClassItems? GetClassItems(INamedTypeSymbol testClass)
        => this.Classes.FirstOrDefault(
            classItem => classItem.NameEquals(testClass.Name));
}
