using System.Collections;

namespace Pikcube.Common.Code.Unions;

/// <summary>
/// Union type consisting of either a single item or many items.
/// </summary>
/// <typeparam name="T">The type of the items.</typeparam>
public union OneOrMany<T>(T, IEnumerable<T>) : IEnumerable<T>
{
    /// <summary>
    /// Either return the original IEnumerable, or wrap the single item in an IEnumerable.
    /// </summary>
    /// <returns>An enumerable collection containing all the elements.</returns>
    public IEnumerable<T> AsEnumerable() => this switch
    {
        T item => WrapSingleItem(item),
        IEnumerable<T> items => items,
        _ => throw new InvalidCastException()
    };

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => this switch
    {
        T item => WrapSingleItem(item).GetEnumerator(),
        IEnumerable<T> items => items.GetEnumerator(),
        _ => throw new InvalidCastException()
    };

    private static IEnumerable<T> WrapSingleItem(T t)
    {
        yield return t;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}