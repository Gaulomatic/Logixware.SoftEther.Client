namespace System.Reactive.Linq
{
	public static class ObservableExtensions
	{
		public static IObservable<TSource> NotNull<TSource>(this IObservable<TSource> source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			return source.SkipWhile(x => x == null);
		}
	}
}