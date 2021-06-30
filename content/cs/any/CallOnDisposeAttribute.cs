#pragma warning disable SA1636
// File is used by the source generator only so Disable CS8618.
#pragma warning disable 8618
namespace IDisposableGenerator
{
    using System;

    // used only by a source generator to generate Dispose() and Dispose(bool).
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal class CallOnDisposeAttribute : Attribute
    {
        public CallOnDisposeAttribute()
        {
        }
    }
}
#pragma warning restore SA1636
#pragma warning restore 8618
