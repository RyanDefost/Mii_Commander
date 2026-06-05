[System.Serializable]
public struct Bounds<T1, T2>
{
    public T1 min;
    public T2 max;

    public Bounds(T1 minValue, T2 maxValue)
    {
        this.min = minValue;
        this.max = maxValue;
    }
}