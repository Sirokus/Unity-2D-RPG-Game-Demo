public class SingletonBase<T> where T : new()
{
    private static T ins;
    private static readonly object locker = new object();
    public static T instance
    {
        get
        {
            if (ins == null)
            {
                lock (locker)
                {
                    if (ins == null)
                        ins = new T();
                }
            }
            return ins;
        }
    }
}
