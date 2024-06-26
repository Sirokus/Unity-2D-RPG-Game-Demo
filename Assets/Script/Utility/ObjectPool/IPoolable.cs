public enum EPoolObjState
{
    Avaiable,
    Ready,
    Using
}

public interface IPoolable
{
    public EPoolObjState state { get; set; }
    public void OnGet();
    public void OnReturn();
}
