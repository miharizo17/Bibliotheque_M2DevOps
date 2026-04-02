using System.Collections.Generic;
public class TupleEqualityComparer<T1, T2> : IEqualityComparer<(T1, T2)>
{
    private readonly IEqualityComparer<T1> _comparer1;
    private readonly IEqualityComparer<T2> _comparer2;

    public TupleEqualityComparer(IEqualityComparer<T1>? comparer1 = null, IEqualityComparer<T2>? comparer2 = null)
    {
        _comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
        _comparer2 = comparer2 ?? EqualityComparer<T2>.Default;
    }

    public bool Equals((T1, T2) x, (T1, T2) y)
    {
        return _comparer1.Equals(x.Item1, y.Item1) && _comparer2.Equals(x.Item2, y.Item2);
    }

    public int GetHashCode((T1, T2) obj)
    {
        unchecked
        {
            int hash1 = _comparer1.GetHashCode(obj.Item1!);
            int hash2 = _comparer2.GetHashCode(obj.Item2!);
            return (hash1 * 397) ^ hash2;
        }
    }
}