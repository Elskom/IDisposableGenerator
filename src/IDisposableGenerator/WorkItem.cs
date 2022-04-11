namespace IDisposableGenerator;

internal class WorkItem
{
    public string Namespace { get; set; } = null!;
    public List<ClassItems> Classes { get; } = new();

    public ClassItems? GetClassItems(INamedTypeSymbol testClass)
        => this.Classes.FirstOrDefault(
            classItem => classItem.NameEquals(testClass.Name));
}
