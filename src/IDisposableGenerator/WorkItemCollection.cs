namespace IDisposableGenerator;

internal class WorkItemCollection(IGeneratedCodeWriter generatedCodeWriter)
{
    internal IGeneratedCodeWriter GeneratedCodeWriter { get; } = generatedCodeWriter;
    private List<WorkItem> WorkItems { get; } = [];

    public int Count => this.WorkItems.Count;

    public void Process(ImmutableArray<INamedTypeSymbol> testClasses, CancellationToken ct)
    {
        foreach (var testClass in testClasses)
        {
            ct.ThrowIfCancellationRequested();
            var workItem = this.FindWithNamespace(testClass.FullNamespace());
            if (workItem is null || !testClass.FullNamespace().Equals("IDisposableGenerator", StringComparison.Ordinal))
            {
                workItem = new WorkItem
                {
                    Namespace = testClass.FullNamespace(),
                };
            }
            ct.ThrowIfCancellationRequested();

            // Avoid a bug that would set namespace to "IDisposableGenerator"
            // instead of the namespace that the WorkItem's classes are in.
            if (testClass.FullNamespaceEquals("IDisposableGenerator"))
            {
                continue;
            }

            ct.ThrowIfCancellationRequested();
            var classItem = GetClassItem(testClass);

            if (classItem is null)
            {
                continue;
            }

            ct.ThrowIfCancellationRequested();
            workItem.Classes.Add(classItem);

            var memberQuery =
                from member in testClass.GetMembers()
                select member;

            foreach (var member in memberQuery)
            {
                ct.ThrowIfCancellationRequested();
                CheckAttributesOnMember(member, testClass, ref workItem, ct);
            }

            this.WorkItems.Add(workItem);
        }
    }

    public List<WorkItem> GetWorkItems()
        => this.WorkItems;

    public int IndexOf(WorkItem item)
        => this.WorkItems.IndexOf(item);

    private static ClassItems? GetClassItem(INamedTypeSymbol testClass)
    {
        var result = new ClassItems();
        var hasDisposalGeneration = false;

        foreach (var attr in testClass.GetAttributes())
        {
#pragma warning disable IDE0010 // Add missing cases
            switch (attr.AttributeClass!.Name)
            {
                case "GenerateDisposeAttribute":
                    hasDisposalGeneration = true;
                    result.Name = testClass.Name;
                    result.Accessibility = testClass.DeclaredAccessibility;
                    result.Stream = (bool)attr.ConstructorArguments[0].Value!;
                    break;
                case "WithoutThrowIfDisposedAttribute":
                    result.WithoutThrowIfDisposed = true;
                    break;
            }
#pragma warning restore IDE0010 // Add missing cases
        }

        return hasDisposalGeneration ? result : null;
    }

    private static void CheckAttributesOnMember(ISymbol member,
        INamedTypeSymbol testClass,
        ref WorkItem workItem,
        CancellationToken ct)
    {
        var classItem = workItem.GetClassItems(testClass)!;
        ct.ThrowIfCancellationRequested();
        foreach (var attr in member.GetAttributes())
        {
            ct.ThrowIfCancellationRequested();
            _ = attr!.AttributeClass!.Name switch
            {
                "DisposeFieldAttribute" => classItem.AddField(attr.ConstructorArguments[0], member),
                "NullOnDisposeAttribute" => classItem.AddSetNull(member),
                "CallOnDisposeAttribute" => classItem.AddMethod(member),

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
