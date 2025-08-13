namespace ObjectExtensions;

public static partial class ObjectExtensions
{
    public static T Tap<T>(this T instance, Action<T> f)
    {
        f(instance);
        return instance;
    }
    
    public static async Task<T> Tap<T>(this T instance, Func<T, Task> f)
    {
        await f(instance);
        return instance;
    }
}