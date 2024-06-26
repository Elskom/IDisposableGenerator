namespace IDisposableGenerator.Tests;

public partial class IDisposableGeneratorTests
{
    [Fact]
    public async Task TestGeneratingPublicDisposableNotOwnsVisualBasic()
        => await RunTest<VBGeneratorTest>(@"' <autogenerated/>
Imports System

Namespace MyApp

    Public Partial Class TestDisposable
        Implements IDisposable

        Private isDisposed As Boolean

        ''' <summary>
        ''' Cleans up the resources used by <see cref=""TestDisposable""/>.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            Me.Dispose(True)
        End Sub

        Private Sub Dispose(ByVal disposing As Boolean)
            If Not Me.isDisposed AndAlso disposing Then
                Me.testDispose?.Dispose()
                Me.testDispose = Nothing
                Me.testsetnull = Nothing
                Me.isDisposed = True
            End If
        End Sub

        Friend Sub ThrowIfDisposed()
            If Me.isDisposed Then
                Throw New ObjectDisposedException(NameOf(TestDisposable))
            End If
        End Sub
    End Class
End Namespace
", @"Imports System
Imports IDisposableGenerator

Namespace MyApp

    <GenerateDispose(False)>
    Public Partial Class TestDisposable
        <DisposeField(False)>
        Private testDispose As IDisposable

        <NullOnDispose>
        Private testsetnull As Char() = { ""t""c, ""e""c, ""s""c, ""t""c, ""i""c, ""n""c, ""g""c }
    End Class
End Namespace
", null);

    [Fact]
    public async Task TestGeneratingDisposableNotOwnsVisualBasic()
        => await RunTest<VBGeneratorTest>(@"' <autogenerated/>
Imports System

Namespace MyApp

    Friend Partial Class TestDisposable
        Implements IDisposable

        Private isDisposed As Boolean

        ''' <summary>
        ''' Cleans up the resources used by <see cref=""TestDisposable""/>.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            Me.Dispose(True)
        End Sub

        Private Sub Dispose(ByVal disposing As Boolean)
            If Not Me.isDisposed AndAlso disposing Then
                Me.testDispose?.Dispose()
                Me.testDispose = Nothing
                Me.testsetnull = Nothing
                Me.isDisposed = True
            End If
        End Sub

        Friend Sub ThrowIfDisposed()
            If Me.isDisposed Then
                Throw New ObjectDisposedException(NameOf(TestDisposable))
            End If
        End Sub
    End Class
End Namespace
", @"Imports System
Imports IDisposableGenerator

Namespace MyApp

    <GenerateDispose(False)>
    Friend Partial Class TestDisposable
        <DisposeField(False)>
        Private testDispose As IDisposable

        <NullOnDispose>
        Private testsetnull As Char() = { ""t""c, ""e""c, ""s""c, ""t""c, ""i""c, ""n""c, ""g""c }
    End Class
End Namespace
", null);

    [Fact]
    public async Task TestGeneratingDisposableOwnsVisualBasic()
        => await RunTest<VBGeneratorTest>(@"' <autogenerated/>
Imports System

Namespace MyApp

    Friend Partial Class TestDisposable
        Implements IDisposable

        Private isDisposed As Boolean

        Friend Property IsOwned As Boolean

        ''' <summary>
        ''' Cleans up the resources used by <see cref=""TestDisposable""/>.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            Me.Dispose(True)
        End Sub

        Private Sub Dispose(ByVal disposing As Boolean)
            If Not Me.isDisposed AndAlso disposing Then
                If Me.IsOwned Then
                    Me.testDispose?.Dispose()
                    Me.testDispose = Nothing
                End If
                Me.testsetnull = Nothing
                Me.isDisposed = True
            End If
        End Sub

        Friend Sub ThrowIfDisposed()
            If Me.isDisposed Then
                Throw New ObjectDisposedException(NameOf(TestDisposable))
            End If
        End Sub
    End Class
End Namespace
", @"Imports System
Imports IDisposableGenerator

Namespace MyApp

    <GenerateDispose(False)>
    Friend Partial Class TestDisposable
        <DisposeField(True)>
        Private testDispose As IDisposable

        <NullOnDispose>
        Private testsetnull As Char() = { ""t""c, ""e""c, ""s""c, ""t""c, ""i""c, ""n""c, ""g""c }
    End Class
End Namespace
", null);

    [Fact]
    public async Task TestGeneratingStreamNotOwnsVisualBasic()
        => await RunTest<VBGeneratorTest>(@"' <autogenerated/>
Imports System

Namespace MyApp

    Friend Partial Class TestDisposable
        Private isDisposed As Boolean

        ''' <inheritdoc/>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If Not Me.isDisposed AndAlso disposing Then
                Me.testDispose?.Dispose()
                Me.testDispose = Nothing
                Me.testsetnull = Nothing
                Me.isDisposed = True
            End If

            ' On Streams call MyBase.Dispose(disposing)!!!
            MyBase.Dispose(disposing)
        End Sub

        Friend Sub ThrowIfDisposed()
            If Me.isDisposed Then
                Throw New ObjectDisposedException(NameOf(TestDisposable))
            End If
        End Sub
    End Class
End Namespace
", @"Imports System
Imports System.IO
Imports IDisposableGenerator

Namespace MyApp

    <GenerateDispose(True)>
    Friend Partial Class TestDisposable
        Inherits Stream

        <DisposeField(False)>
        Private testDispose As IDisposable

        <NullOnDispose>
        Private testsetnull As Char() = { ""t""c, ""e""c, ""s""c, ""t""c, ""i""c, ""n""c, ""g""c }

        Public Overrides ReadOnly Property CanRead As Boolean
            Get
                Throw New NotSupportedException()
            End Get
        End Property
        Public Overrides ReadOnly Property CanSeek As Boolean
            Get
                Throw New NotSupportedException()
            End Get
        End Property
        Public Overrides ReadOnly Property CanWrite As Boolean
            Get
                Throw New NotSupportedException()
            End Get
        End Property
        Public Overrides Sub Flush()
            Throw New NotSupportedException()
        End Sub
        Public Overrides ReadOnly Property Length As Long
            Get
                Throw New NotSupportedException()
            End Get
        End Property
        Public Overrides Property Position As Long
            Get
                Throw New NotSupportedException()
            End Get
            Set(ByVal value As Long)
                Throw New NotSupportedException()
            End Set
        End Property
        Public Overrides Function Read(ByVal __ As Byte(), ByVal _1 As Integer, ByVal _2 As Integer) As Integer
            Throw New NotSupportedException()
        End Function
        Public Overrides Function Seek(ByVal __ As Long, ByVal _1 As SeekOrigin) As Long
            Throw New NotSupportedException()
        End Function
        Public Overrides Sub SetLength(ByVal __ As Long)
            Throw New NotSupportedException()
        End Sub
        Public Overrides Sub Write(ByVal __ As Byte(), ByVal _1 As Integer, ByVal _2 As Integer)
            Throw New NotSupportedException()
        End Sub
    End Class
End Namespace
", null);

    [Fact]
    public async Task TestGeneratingStreamOwnsVisualBasic()
        => await RunTest<VBGeneratorTest>(@"' <autogenerated/>
Imports System

Namespace MyApp

    Friend Partial Class TestDisposable
        Private isDisposed As Boolean

        Friend ReadOnly Property KeepOpen As Boolean

        ''' <inheritdoc/>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If Not Me.isDisposed AndAlso disposing Then
                If Not Me.KeepOpen Then
                    Me.testDispose?.Dispose()
                    Me.testDispose = Nothing
                End If
                Me.testsetnull = Nothing
                Me.isDisposed = True
            End If

            ' On Streams call MyBase.Dispose(disposing)!!!
            MyBase.Dispose(disposing)
        End Sub

        Friend Sub ThrowIfDisposed()
            If Me.isDisposed Then
                Throw New ObjectDisposedException(NameOf(TestDisposable))
            End If
        End Sub
    End Class
End Namespace
", @"Imports System
Imports System.IO
Imports IDisposableGenerator

Namespace MyApp

    <GenerateDispose(True)>
    Friend Partial Class TestDisposable
        Inherits Stream

        <DisposeField(true)>
        Private testDispose As IDisposable

        <NullOnDispose>
        Private testsetnull As Char() = { ""t""c, ""e""c, ""s""c, ""t""c, ""i""c, ""n""c, ""g""c }

        Public Overrides ReadOnly Property CanRead As Boolean
            Get
                Throw New NotSupportedException()
            End Get
        End Property
        Public Overrides ReadOnly Property CanSeek As Boolean
            Get
                Throw New NotSupportedException()
            End Get
        End Property
        Public Overrides ReadOnly Property CanWrite As Boolean
            Get
                Throw New NotSupportedException()
            End Get
        End Property
        Public Overrides Sub Flush()
            Throw New NotSupportedException()
        End Sub
        Public Overrides ReadOnly Property Length As Long
            Get
                Throw New NotSupportedException()
            End Get
        End Property
        Public Overrides Property Position As Long
            Get
                Throw New NotSupportedException()
            End Get
            Set(ByVal value As Long)
                Throw New NotSupportedException()
            End Set
        End Property
        Public Overrides Function Read(ByVal __ As Byte(), ByVal _1 As Integer, ByVal _2 As Integer) As Integer
            Throw New NotSupportedException()
        End Function
        Public Overrides Function Seek(ByVal __ As Long, ByVal _1 As SeekOrigin) As Long
            Throw New NotSupportedException()
        End Function
        Public Overrides Sub SetLength(ByVal __ As Long)
            Throw New NotSupportedException()
        End Sub
        Public Overrides Sub Write(ByVal __ As Byte(), ByVal _1 As Integer, ByVal _2 As Integer)
            Throw New NotSupportedException()
        End Sub
    End Class
End Namespace
", null);

    [Fact]
    public async Task TestGeneratingCallOnDisposeVisualBasic()
        => await RunTest<VBGeneratorTest>(@"' <autogenerated/>
Imports System

Namespace MyApp

    Friend Partial Class TestDisposable
        Implements IDisposable

        Private isDisposed As Boolean

        ''' <summary>
        ''' Cleans up the resources used by <see cref=""TestDisposable""/>.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            Me.Dispose(True)
        End Sub

        Private Sub Dispose(ByVal disposing As Boolean)
            If Not Me.isDisposed AndAlso disposing Then
                Me.TestCallThisOnDispose()
                Me.testDispose?.Dispose()
                Me.testDispose = Nothing
                Me.testsetnull = Nothing
                Me.isDisposed = True
            End If
        End Sub

        Friend Sub ThrowIfDisposed()
            If Me.isDisposed Then
                Throw New ObjectDisposedException(NameOf(TestDisposable))
            End If
        End Sub
    End Class
End Namespace
", @"Imports System
Imports IDisposableGenerator

Namespace MyApp
    <GenerateDispose(False)>
    Friend Partial Class TestDisposable
        <DisposeField(False)>
        Private testDispose As IDisposable

        <NullOnDispose>
        Private testsetnull As Char() = { ""t""c, ""e""c, ""s""c, ""t""c, ""i""c, ""n""c, ""g""c }

        <CallOnDispose>
        Private Sub TestCallThisOnDispose()
            ' Intentionally left this empty (cannot throw exceptions
            ' here as this is called inside of Dispose(Boolean)).
        End Sub
    End Class
End Namespace
", null);

    [Fact]
    public async Task TestAttributeOnDisposableMemberFromBCLVisualBasic()
        => await RunTest<VBGeneratorTest>(@"' <autogenerated/>
Imports System

Namespace MyApp

    Friend Partial Class TestDisposable
        Implements IDisposable

        Private isDisposed As Boolean

        ''' <summary>
        ''' Cleans up the resources used by <see cref=""TestDisposable""/>.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            Me.Dispose(True)
        End Sub

        Private Sub Dispose(ByVal disposing As Boolean)
            If Not Me.isDisposed AndAlso disposing Then
                Me.test = Nothing
                Me.isDisposed = True
            End If
        End Sub

        Friend Sub ThrowIfDisposed()
            If Me.isDisposed Then
                Throw New ObjectDisposedException(NameOf(TestDisposable))
            End If
        End Sub
    End Class
End Namespace
", @"Imports System
Imports System.ComponentModel.DataAnnotations
Imports IDisposableGenerator

Namespace MyApp

    <GenerateDispose(False)>
    Friend Partial Class TestDisposable
        <NullOnDispose>
        <StringLength(50)>
        Public Property test As String = ""stuff here.""
    End Class
End Namespace
", null);

    [Fact]
    public async Task TestWithoutThrowIfDisposedVisualBasic()
    {
        const string generatedSource = """
            ' <autogenerated/>
            Imports System

            Namespace MyApp

                Friend Partial Class TestDisposable
                    Implements IDisposable

                    Private isDisposed As Boolean

                    ''' <summary>
                    ''' Cleans up the resources used by <see cref="TestDisposable"/>.
                    ''' </summary>
                    Public Sub Dispose() Implements IDisposable.Dispose
                        Me.Dispose(True)
                    End Sub

                    Private Sub Dispose(ByVal disposing As Boolean)
                        If Not Me.isDisposed AndAlso disposing Then
                            Me.test = Nothing
                            Me.isDisposed = True
                        End If
                    End Sub
                End Class
            End Namespace

            """;

        const string testSource = """
            Imports System
            Imports System.ComponentModel.DataAnnotations
            Imports IDisposableGenerator

            Namespace MyApp
                <GenerateDispose(False)>
                <WithoutThrowIfDisposed>
                Friend Partial Class TestDisposable
                    <NullOnDispose>
                    <StringLength(50)>
                    Public Property test As String = "stuff here."
                End Class
            End Namespace

            """;

        await RunTest<VBGeneratorTest>(generatedSource, testSource, null);
    }
}
