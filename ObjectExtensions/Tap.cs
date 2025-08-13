namespace ObjectExtensions;

public static partial class ObjectExtensions
{
    public static T Tap<T>(this T instance, Action<T> f)
    {
        f(instance);
        return instance;
    }
}