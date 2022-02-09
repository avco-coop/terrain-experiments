using System;

public static class ObjectExtensions {

  public static T2 With<T1, T2> (this T1 o, Func<T1, T2> f) {
    return f(o);
  }

  public static T Tap<T> (this T o, Action<T> f) {
    f(o);
    return o;
  }

}
