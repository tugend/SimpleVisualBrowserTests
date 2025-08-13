namespace ObjectExtensions;

public static partial class ObjectExtensions
{
    public static TOut Map<TIn, TOut>(this TIn instance, Func<TIn, TOut> f) => 
        f(instance);
}