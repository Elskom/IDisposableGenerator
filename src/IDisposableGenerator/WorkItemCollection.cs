namespace IDisposableGenerator;

internal class WorkItemCollection
{
    internal Compilation Compilation { get; }
    private List<WorkItem> WorkItems { get; } = new();

    public WorkItemCollection(Compilation compilation)
        => this.Compilation = compilation;

    public int Count => this.WorkItems.Count;

    public void Process(INamedTypeSymbol testClass, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        AddFromNamespace(testClass.FullNamespace());
        var workItem = FindWithNamespace(testClass.FullNamespace());
        ct.ThrowIfCancellationRequested();

        // Avoid a bug that would set namespace to "IDisposableGenerator"
        // instead of the namespace that the WorkItem's classes are in.
        if (testClass.FullNamespaceEquals("IDisposableGenerator"))
        {
            return;
        }

        ct.ThrowIfCancellationRequested();
        var classItemsQuery =
            from att in testClass.GetAttributes()
            where att.AttributeClass!.Name.Equals(
                "GenerateDisposeAttribute")
            select GetClassItem(att, testClass);
        var memberQuery =
            from member in testClass.GetMembers()
            select member;
        foreach (var classItem in classItemsQuery)
        {
            ct.ThrowIfCancellationRequested();
            workItem!.Classes.Add(classItem);
        }

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

    private static ClassItems GetClassItem(AttributeData attr, INamedTypeSymbol testClass)
    {
        var result = new ClassItems
        {
            Name = testClass.Name,
            Accessibility = testClass.DeclaredAccessibility,
            Stream = (bool)attr.ConstructorArguments[0].Value!,
        };

        return result;
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

        WorkItems.Add(new WorkItem
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
