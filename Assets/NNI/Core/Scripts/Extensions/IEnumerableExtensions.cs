using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IEnumerableExtensions
{

  public static IEnumerable<TSource> NotNull<TSource> (this IEnumerable<TSource> source) {
    return source.Where(e => e != null);
  }

  public static IEnumerable<T> ForEach<T> (this IEnumerable<T> e, Action<T> f)
  {
    foreach (var v in e)
      f(v);
    return e;
  }

  public static IEnumerable<T> Shuffle<T> (this IEnumerable<T> source) {
    return source.Shuffle(new System.Random());
  }

  public static IEnumerable<T> Shuffle<T> (this IEnumerable<T> source, System.Random rng) {
    if (source == null)
      throw new ArgumentNullException(nameof(source));
    if (rng == null)
      throw new ArgumentNullException(nameof(rng));

    return source.ShuffleIterator(rng);
  }

  private static IEnumerable<T> ShuffleIterator<T> (
      this IEnumerable<T> source, System.Random rng) {
    var buffer = source.ToList();
    for (int i = 0; i < buffer.Count; i++) {
      int j = rng.Next(i, buffer.Count);
      yield return buffer[j];

      buffer[j] = buffer[i];
    }
  }

}
