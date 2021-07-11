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

        public void AddSetNull(ISymbol member)
            => this.SetNull.Add(member.Name);

        public void AddMethod(ISymbol member)
            => this.Methods.Add(member.Name);

        public void AddField(TypedConstant arg, ISymbol member)
        {
            if ((bool)arg.Value!)
            {
                this.Owns.Add(member.Name);
            }
            else
            {
                this.Fields.Add(member.Name);
            }
        }
    }
}
