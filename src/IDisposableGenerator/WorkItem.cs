namespace IDisposableGenerator;

using System;

internal class WorkItem
{
    public string Namespace { get; set; } = null!;
    public List<ClassItems> Classes { get; } = [];

    public ClassItems? GetClassItems(INamedTypeSymbol testClass)
        => this.Classes.FirstOrDefault(
            classItem => classItem.NameEquals(testClass.Name));

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        var sb = new StringBuilder($"Namespace: Name: {this.Namespace}");
        foreach (var classItems in this.Classes)
        {
            _ = sb.AppendLine();
            _ = sb.Append($"Class Item {this.Classes.IndexOf(classItems)}: {classItems}");
        }

        return sb.ToString();
    }

    internal static string ReduceIndentation(string code)
    {
        var eol = (code.Contains("\r\n"), code.Contains("\n")) switch
        {
            (true, true) => "\r\n",
            (false, true) => "\n",
            (false, false) => "\r",
            _ => throw new InvalidOperationException("bug"),
        };

        var lines = code.Split([eol], StringSplitOptions.None);
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            line = ReplaceLeadingTabsWithSpaces(line);
            if (line.StartsWith("    ", StringComparison.Ordinal))
            {
                lines[i] = line.Substring(4);
            }
        }

        return string.Join(eol, lines);
    }

    [ExcludeFromCodeCoverage]
    private static string ReplaceLeadingTabsWithSpaces(string code)
    {
        var result = new StringBuilder();
        var index = 0;
        while (index < code.Length && code[index] == '\t')
        {
            _ = result.Append("    ");
            index++;
        }

        _ = result.Append(code.Substring(index));
        return result.ToString();
    }
}
