#pragma warning disable IDE0130
namespace System.Runtime.CompilerServices
#pragma warning restore IDE0130
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IUnion
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        object? Value { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed class UnionAttribute : Attribute
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
    }
}