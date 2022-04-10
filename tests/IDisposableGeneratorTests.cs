namespace IDisposableGenerator.Tests;

public class IDisposableGeneratorTests
{
    [Fact]
    public async Task TestGeneratingPublicDisposableNotOwns()
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
    public async Task TestGeneratingDisposableNotOwns()
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
    public async Task TestGeneratingDisposableOwns()
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
    public async Task TestGeneratingStreamNotOwns()
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
    public async Task TestGeneratingStreamOwns()
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
    public async Task TestGeneratingCallOnDispose()
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
            // here as this is called inside of Dipose(bool)).
        }
    }
}
").ConfigureAwait(false);

    [Fact]
    public async Task TestGeneratingNoInput()
        => _ = await Assert.ThrowsAsync<EqualWithMessageException>([ExcludeFromCodeCoverage] async () =>
        {
            await RunTest<CSGeneratorTest>(string.Empty, string.Empty).ConfigureAwait(false);
        }).ConfigureAwait(false);

    private static async Task RunTest<TestType>(
        string generatedSource,
        string testSource,
        LanguageVersion languageVersion = LanguageVersion.CSharp9)
        where TestType : SourceGeneratorTest<XUnitVerifier>, IGeneratorTestBase, new()
    {
        var test = new TestType
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
            TestState =
            {
                Sources =
                {
                    testSource
                },
            },
        };

        if (!string.IsNullOrEmpty(testSource) && test is CSGeneratorTest tst)
        {
            tst.LanguageVersion = languageVersion;
            
            // do not duplicate the code to the attributes here.
            test.TestState.Sources.Add(
                await File.ReadAllTextAsync(
                    "../../../../content/cs/any/CallOnDisposeAttribute.cs").ConfigureAwait(false));
            test.TestState.Sources.Add(
                await File.ReadAllTextAsync(
                    "../../../../content/cs/any/DisposeFieldAttribute.cs").ConfigureAwait(false));
            test.TestState.Sources.Add(
                await File.ReadAllTextAsync(
                    "../../../../content/cs/any/GenerateDisposeAttribute.cs").ConfigureAwait(false));
            test.TestState.Sources.Add(
                await File.ReadAllTextAsync(
                    "../../../../content/cs/any/SetNullOnDisposeAttribute.cs").ConfigureAwait(false));
            test.TestState.GeneratedSources.Add(
                (typeof(IDisposableGenerator), "Disposables.g.cs", generatedSource));
        }
        else
        {
            test.TestState.Sources.Clear();
        }

        await test.RunAsync();
    }
}
