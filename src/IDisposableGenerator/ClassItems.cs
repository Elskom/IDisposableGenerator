namespace IDisposibleGenerator
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    internal class ClassItems
    {
        public string? Name { get; set; }
        public Accessibility Accessibility { get; set; }
        public bool Stream { get; set; }
        public List<string> Owns { get; } = new();
        public List<string> Fields { get; } = new();
        public List<string> SetNull { get ; } = new();
        public List<string> Methods { get; } = new();
    }
}
