namespace IDisposableGenerator;

internal class WorkItemCollection(Compilation compilation)
{
    internal Compilation Compilation { get; } = compilation;
    private List<WorkItem> WorkItems { get; } = [];

    public int Count => this.WorkItems.Count;

    public void Process(INamedTypeSymbol testClass, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        this.AddFromNamespace(testClass.FullNamespace());
        var workItem = this.FindWithNamespace(testClass.FullNamespace());
        ct.ThrowIfCancellationRequested();

        // Avoid a bug that would set namespace to "IDisposableGenerator"
        // instead of the namespace that the WorkItem's classes are in.
        if (testClass.FullNamespaceEquals("IDisposableGenerator"))
        {
            return;
        }

        ct.ThrowIfCancellationRequested();
        var classItem = GetClassItem(testClass);

        if (classItem is null)
        {
            return;
        }

        ct.ThrowIfCancellationRequested();
        workItem!.Classes.Add(classItem);

        var memberQuery =
            from member in testClass.GetMembers()
            select member;

        foreach (var member in memberQuery)
        {
            ct.ThrowIfCancellationRequested();
            CheckAttributesOnMember(member, testClass, ref workItem!, ct);
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
                case "GenerateThrowIfDisposedAttribute":
                    result.ThrowIfDisposed = true;
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

    private void AddFromNamespace(string nameSpace)
    {
        if (this.FindWithNamespace(nameSpace) is not null
            || nameSpace.Equals("IDisposableGenerator", StringComparison.Ordinal))
        {
            return;
        }

        this.WorkItems.Add(new WorkItem
        {
            Namespace = nameSpace,
        });
    }

    private WorkItem? FindWithNamespace(string nameSpace)
        => this.WorkItems.FirstOrDefault(
            workItem => workItem.Namespace.Equals(
                nameSpace,
                StringComparison.Ordinal));
}
