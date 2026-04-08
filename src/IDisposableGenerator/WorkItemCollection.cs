namespace IDisposableGenerator;

internal class WorkItemCollection(IGeneratedCodeWriter generatedCodeWriter)
{
    internal IGeneratedCodeWriter GeneratedCodeWriter { get; } = generatedCodeWriter;
    private List<WorkItem> WorkItems { get; } = [];

    public int Count => this.WorkItems.Count;

    public void Process(ImmutableArray<ClassSymbolCache> testClasses, CancellationToken ct)
    {
        foreach (var testClass in testClasses)
        {
            ct.ThrowIfCancellationRequested();
            var workItem = this.FindWithNamespace(testClass.Namespace)
                ?? new WorkItem
                {
                    Namespace = testClass.Namespace,
                };
            ct.ThrowIfCancellationRequested();
            var classItem = GetClassItem(testClass);
            if (classItem is null)
            {
                continue;
            }

            ct.ThrowIfCancellationRequested();
            workItem.Classes.Add(classItem);
            foreach (var memberPair in testClass.MemberAttributes)
            {
                ct.ThrowIfCancellationRequested();
                CheckAttributesOnMember(memberPair, testClass, ref workItem, ct);
            }

            this.WorkItems.Add(workItem);
        }
    }

    public List<WorkItem> GetWorkItems()
        => this.WorkItems;

    public int IndexOf(WorkItem item)
        => this.WorkItems.IndexOf(item);

    private static ClassItems? GetClassItem(ClassSymbolCache testClass)
    {
        var result = new ClassItems();
        var hasDisposalGeneration = false;

        foreach (var attr in testClass.ClassAttributes)
        {
#pragma warning disable IDE0010 // Add missing cases
            switch (attr.AttributeClass!.Name)
            {
                case "GenerateDisposeAttribute":
                    hasDisposalGeneration = true;
                    result.Name = testClass.Name;
                    result.Accessibility = testClass.DeclaredAccessibility;
                    result.Stream = (bool)attr.ConstructorArguments[0].Value!;
                    result.IsAsyncDisposable = (bool)attr.ConstructorArguments[1].Value!;
                    result.BaseIsDisposable = testClass.BaseIsDisposable;
                    result.BaseIsAsyncDisposable = testClass.BaseIsAsyncDisposable;
                    break;
                case "WithoutThrowIfDisposedAttribute":
                    result.WithoutThrowIfDisposed = true;
                    break;
            }
#pragma warning restore IDE0010 // Add missing cases
        }

        return hasDisposalGeneration ? result : null;
    }

    private static void CheckAttributesOnMember(KeyValuePair<ISymbol, ImmutableArray<AttributeData>> memberPair,
        ClassSymbolCache testClass,
        ref WorkItem workItem,
        CancellationToken ct)
    {
        var classItem = workItem.GetClassItems(testClass)!;
        ct.ThrowIfCancellationRequested();
        foreach (var attr in memberPair.Value)
        {
            ct.ThrowIfCancellationRequested();
            _ = attr!.AttributeClass!.Name switch
            {
                "DisposeFieldAttribute" => classItem.AddField(attr.ConstructorArguments[0], memberPair.Key),
                "NullOnDisposeAttribute" => classItem.AddSetNull(memberPair.Key),
                "CallOnDisposeAttribute" => classItem.AddMethod(memberPair.Key),

                // cannot throw here because the attribute in this case should be ignored.
                _ => false,
            };
        }
    }

    private WorkItem? FindWithNamespace(string nameSpace)
        => this.WorkItems.FirstOrDefault(
            workItem => workItem.Namespace.Equals(
                nameSpace,
                StringComparison.Ordinal));
}
