namespace IDisposibleGenerator
{
    using System.Collections.Generic;

    internal class WorkItem
    {
        public string? Namespace { get; set; }
        public List<ClassItems> Classes { get; } = new();
    }
}
