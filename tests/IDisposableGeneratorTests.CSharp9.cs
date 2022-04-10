namespace IDisposableGenerator.Tests;

public partial class IDisposableGeneratorTests
{
    [Fact]
    public async Task TestGeneratingPublicDisposableNotOwnsCSharp9()
        => await RunTest<CSGeneratorTest>(@"// <autogenerated/>
namespace MyApp
{
    using global::System;

    public partial class TestDisposable : IDisposable
    {
        private bool isDisposed;

        /// <summary>
        /// Cleans up the resources used by <see cref=""TestDisposable""/>.
        /// </summary>
        public void Dispose() => this.Dispose(true);

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed && disposing)
            {
                this.testDispose?.Dispose();
                this.testDispose = null;
                this.testsetnull = null;
                this.isDisposed = true;
            }
        }
    }
}
", @"namespace MyApp
{
    using System;
    using IDisposableGenerator;

    [GenerateDispose(false)]
    public partial class TestDisposable
    {
        [DisposeField(false)]
        private IDisposable testDispose;

        [SetNullOnDispose]
        char[] testsetnull = new char[] { 't', 'e', 's', 't', 'i', 'n', 'g' };
    }
}
").ConfigureAwait(false);

    [Fact]
    public async Task TestGeneratingDisposableNotOwnsCSharp9()
        => await RunTest<CSGeneratorTest>(@"// <autogenerated/>
namespace MyApp
{
    using global::System;

    internal partial class TestDisposable : IDisposable
    {
        private bool isDisposed;

        /// <summary>
        /// Cleans up the resources used by <see cref=""TestDisposable""/>.
        /// </summary>
        public void Dispose() => this.Dispose(true);

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed && disposing)
            {
                this.testDispose?.Dispose();
                this.testDispose = null;
                this.testsetnull = null;
                this.isDisposed = true;
            }
        }
    }
}
", @"namespace MyApp
{
    using System;
    using IDisposableGenerator;

    [GenerateDispose(false)]
    internal partial class TestDisposable
    {
        [DisposeField(false)]
        private IDisposable testDispose;

        [SetNullOnDispose]
        char[] testsetnull = new char[] { 't', 'e', 's', 't', 'i', 'n', 'g' };
    }
}
").ConfigureAwait(false);

    [Fact]
    public async Task TestGeneratingDisposableOwnsCSharp9()
        => await RunTest<CSGeneratorTest>(@"// <autogenerated/>
namespace MyApp
{
    using global::System;

    internal partial class TestDisposable : IDisposable
    {
        private bool isDisposed;

        internal bool IsOwned { get; set; }

        /// <summary>
        /// Cleans up the resources used by <see cref=""TestDisposable""/>.
        /// </summary>
        public void Dispose() => this.Dispose(true);

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed && disposing)
            {
                if (this.IsOwned)
                {
                    this.testDispose?.Dispose();
                    this.testDispose = null;
                }
                this.testsetnull = null;
                this.isDisposed = true;
            }
        }
    }
}
", @"namespace MyApp
{
    using System;
    using IDisposableGenerator;

    [GenerateDispose(false)]
    internal partial class TestDisposable
    {
        [DisposeField(true)]
        private IDisposable testDispose;

        [SetNullOnDispose]
        char[] testsetnull = new char[] { 't', 'e', 's', 't', 'i', 'n', 'g' };
    }
}
").ConfigureAwait(false);

    [Fact]
    public async Task TestGeneratingStreamNotOwnsCSharp9()
        => await RunTest<CSGeneratorTest>(@"// <autogenerated/>
namespace MyApp
{
    using global::System;

    internal partial class TestDisposable
    {
        private bool isDisposed;

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!this.isDisposed && disposing)
            {
                this.testDispose?.Dispose();
                this.testDispose = null;
                this.testsetnull = null;
                this.isDisposed = true;
            }

            // On Streams call base.Dispose(disposing)!!!
            base.Dispose(disposing);
        }
    }
}
", @"namespace MyApp
{
    using System;
    using System.IO;
    using IDisposableGenerator;

    [GenerateDispose(true)]
    internal partial class TestDisposable : Stream
    {
        [DisposeField(false)]
        private IDisposable testDispose;

        [SetNullOnDispose]
        char[] testsetnull = new char[] { 't', 'e', 's', 't', 'i', 'n', 'g' };

        public override bool CanRead { get => throw new NotSupportedException(); }
        public override bool CanSeek { get => throw new NotSupportedException(); }
        public override bool CanWrite { get => throw new NotSupportedException(); }
        public override void Flush() => throw new NotSupportedException();
        public override long Length { get => throw new NotSupportedException(); }
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override int Read(byte[] _, int _1, int _2) => throw new NotSupportedException();
        public override long Seek(long _, SeekOrigin _1) => throw new NotSupportedException();
        public override void SetLength(long _) => throw new NotSupportedException();
        public override void Write(byte[] _, int _1, int _2) => throw new NotSupportedException();
    }
}
").ConfigureAwait(false);

    [Fact]
    public async Task TestGeneratingStreamOwnsCSharp9()
        => await RunTest<CSGeneratorTest>(@"// <autogenerated/>
namespace MyApp
{
    using global::System;

    internal partial class TestDisposable
    {
        private bool isDisposed;

        internal bool KeepOpen { get; }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!this.isDisposed && disposing)
            {
                if (this.KeepOpen)
                {
                    this.testDispose?.Dispose();
                    this.testDispose = null;
                }
                this.testsetnull = null;
                this.isDisposed = true;
            }

            // On Streams call base.Dispose(disposing)!!!
            base.Dispose(disposing);
        }
    }
}
", @"namespace MyApp
{
    using System;
    using System.IO;
    using IDisposableGenerator;

    [GenerateDispose(true)]
    internal partial class TestDisposable : Stream
    {
        [DisposeField(true)]
        private IDisposable testDispose;

        [SetNullOnDispose]
        char[] testsetnull = new char[] { 't', 'e', 's', 't', 'i', 'n', 'g' };

        public override bool CanRead { get => throw new NotSupportedException(); }
        public override bool CanSeek { get => throw new NotSupportedException(); }
        public override bool CanWrite { get => throw new NotSupportedException(); }
        public override void Flush() => throw new NotSupportedException();
        public override long Length { get => throw new NotSupportedException(); }
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override int Read(byte[] _, int _1, int _2) => throw new NotSupportedException();
        public override long Seek(long _, SeekOrigin _1) => throw new NotSupportedException();
        public override void SetLength(long _) => throw new NotSupportedException();
        public override void Write(byte[] _, int _1, int _2) => throw new NotSupportedException();
    }
}
").ConfigureAwait(false);

    [Fact]
    public async Task TestGeneratingCallOnDisposeCSharp9()
        => await RunTest<CSGeneratorTest>(@"// <autogenerated/>
namespace MyApp
{
    using global::System;

    internal partial class TestDisposable : IDisposable
    {
        private bool isDisposed;

        /// <summary>
        /// Cleans up the resources used by <see cref=""TestDisposable""/>.
        /// </summary>
        public void Dispose() => this.Dispose(true);

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed && disposing)
            {
                this.TestCallThisOnDispose();
                this.testDispose?.Dispose();
                this.testDispose = null;
                this.testsetnull = null;
                this.isDisposed = true;
            }
        }
    }
}
", @"namespace MyApp
{
    using System;
    using IDisposableGenerator;

    [GenerateDispose(false)]
    internal partial class TestDisposable
    {
        [DisposeField(false)]
        private IDisposable testDispose;

        [SetNullOnDispose]
        char[] testsetnull = new char[] { 't', 'e', 's', 't', 'i', 'n', 'g' };

        [CallOnDispose]
        private void TestCallThisOnDispose()
        {
            // Intentionally left this empty (cannot throw exceptions
            // here as this is called inside of Dispose(bool)).
        }
    }
}
").ConfigureAwait(false);

    [Fact]
    public async Task TestAttributeOnDisposableMemberFromBCLCSharp9()
        => await RunTest<CSGeneratorTest>(@"// <autogenerated/>
namespace MyApp
{
    using global::System;

    internal partial class TestDisposable : IDisposable
    {
        private bool isDisposed;

        /// <summary>
        /// Cleans up the resources used by <see cref=""TestDisposable""/>.
        /// </summary>
        public void Dispose() => this.Dispose(true);

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed && disposing)
            {
                this.test = null;
                this.isDisposed = true;
            }
        }
    }
}
", @"namespace MyApp
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using IDisposableGenerator;

    [GenerateDispose(false)]
    internal partial class TestDisposable
    {
        [SetNullOnDispose]
        [StringLength(50)]
        public string? test { get; set; } = ""stuff here."";
    }
}
").ConfigureAwait(false);
}