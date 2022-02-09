using System;
using System.Collections.Generic;
using UnityEngine;

public static class ICollectionExtensions
{

  public static void Add<T> (this ICollection<T> target, params T[] items) {
    foreach (var item in items)
      target.Add(item);
  }

}
