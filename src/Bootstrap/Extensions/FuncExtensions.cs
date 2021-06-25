using System;

namespace Bootstrap.Extensions
{
    public static class FuncExtensions
    {
        public static Func<TResource, object> BuildKeySelector<TResource>() => BuildKeySelector<TResource, object>();

        public static Func<TResource, TKey> BuildKeySelector<TResource, TKey>() =>
            ExpressionExtensions.BuildKeySelector<TResource, TKey>().Compile();
    }
}