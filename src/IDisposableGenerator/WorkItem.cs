namespace IDisposibleGenerator;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal class WorkItem
{
    public string? Namespace { get; set; }
    public List<ClassItems> Classes { get; } = new();

    public ClassItems? GetClassItems(INamedTypeSymbol testClass)
    {
        return this.Classes.FirstOrDefault(classItem => classItem.Name!.Equals(testClass.Name, StringComparison.Ordinal));
    }

    public bool ContainsClass(INamedTypeSymbol testClass)
    {
        return GetClassItems(testClass) is not null;
    }
}
