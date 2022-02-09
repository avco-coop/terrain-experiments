using System;
using System.Collections.Generic;
using UnityEngine;

public static class IListExtensions
{

  public static T Sample<T> (this IList<T> source) {
    if (source.Count == 0)
      return default;
    return source[UnityEngine.Random.Range(0, source.Count)];
  }

}
