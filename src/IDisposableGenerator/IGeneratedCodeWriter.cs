namespace IDisposableGenerator;

internal interface IGeneratedCodeWriter
{
    /// <summary>
    /// Writes the generated code as a string using the .
    /// </summary>
    /// <returns></returns>
    List<(string file, StringBuilder code)> ToCodeStrings(SourceProductionContext ctx, ReadOnlyCollection<WorkItem> workItems);
}
