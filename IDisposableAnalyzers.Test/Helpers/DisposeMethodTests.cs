namespace IDisposableAnalyzers.Test.Helpers
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class DisposeMethodTests
    {
        [TestCase(Search.TopLevel)]
        [TestCase(Search.Recursive)]
        public static void TryFindIDisposableDispose(Search search)
        {
            var testCode = @"
namespace N
{
    using System;

    internal sealed class C : IDisposable
    {
        private bool disposed;

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var method = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("C"));
            Assert.AreEqual(true, DisposeMethod.TryFindIDisposableDispose(method, compilation, search, out var match));
            Assert.AreEqual("N.C.Dispose()", match.ToString());
        }

        [Explicit("Not sure if we want to find explicit.")]
        [TestCase(Search.TopLevel)]
        [TestCase(Search.Recursive)]
        public static void TryFindIDisposableDisposeWhenExplicit(Search search)
        {
            var testCode = @"
namespace N
{
    using System;

    internal sealed class C : IDisposable
    {
        private bool disposed;

        void IDisposable.Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var method = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("C"));
            Assert.AreEqual(true, DisposeMethod.TryFindIDisposableDispose(method, compilation, search, out var match));
            Assert.AreEqual("N.C.Dispose()", match.ToString());
        }

        [TestCase(Search.TopLevel)]
        [TestCase(Search.Recursive)]
        public static void TryFindVirtualDispose(Search search)
        {
            var testCode = @"
namespace N
{
    using System;

    internal class C : IDisposable
    {
        private bool disposed;

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
            }
        }

        protected virtual void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var method = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("C"));
            Assert.AreEqual(true, DisposeMethod.TryFindVirtualDispose(method, compilation, search, out var match));
            Assert.AreEqual("N.C.Dispose(bool)", match.ToString());
        }

        [TestCase(Search.TopLevel)]
        [TestCase(Search.Recursive)]
        public static void TryFindFirst(Search search)
        {
            var testCode = @"
namespace N
{
    using System;

    internal class C : IDisposable
    {
        private bool disposed;

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
            }
        }

        protected virtual void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var method = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("C"));
            Assert.AreEqual(true, DisposeMethod.TryFindFirst(method, compilation, search, out var match));
            Assert.AreEqual("N.C.Dispose()", match.ToString());
        }
    }
}
