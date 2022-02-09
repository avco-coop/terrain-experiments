using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
using Unity.EditorCoroutines.Editor;
#endif

public class Task {

  public bool canceled;

  public Stopwatch timer = Stopwatch.StartNew();

  public MonoBehaviour owner;

  public Coroutine coroutine;

#if UNITY_EDITOR
  public int progress;

  public EditorCoroutine editorCoroutine;

  public bool shouldWait {
    get {
      if (timer.ElapsedMilliseconds > 100) {
        timer.Restart();
        return true;
      }
      return false;
    }
  }
#endif

#if UNITY_EDITOR
  public Task (Func<Task, IEnumerator> e, UnityEngine.Object owner, string name) {
    progress = Progress.Start(name);
    editorCoroutine = EditorCoroutineUtility.StartCoroutine(e(this), owner);
    Progress.RegisterCancelCallback(progress, () => canceled = true);
  }
#endif

  public Task (Func<Task, IEnumerator> e, MonoBehaviour owner, string name) {
    this.owner = owner;
#if UNITY_EDITOR
    if (!Application.IsPlaying(owner)) {
      progress = Progress.Start(name);
      editorCoroutine = EditorCoroutineUtility.StartCoroutine(e(this), owner);
      Progress.RegisterCancelCallback(progress, () => canceled = true);
      return;
    }
#endif
    coroutine = owner.StartCoroutine(e(this));
  }

  public void Finish () {
#if UNITY_EDITOR
    if (progress != 0) {
      Progress.Remove(progress);
      progress = 0;
    }
#endif
  }

  public void Dispose () {
    Finish();
#if UNITY_EDITOR
    if (editorCoroutine != null) {
      EditorCoroutineUtility.StopCoroutine(editorCoroutine);
      editorCoroutine = null;
    }
#endif
    if (coroutine != null) {
      owner.StopCoroutine(coroutine);
      coroutine = null;
    }
  }

  public void Update (float progress, string description = "") {
#if UNITY_EDITOR
    if (this.progress != 0) {
      Progress.Report(this.progress, progress, description);
    }
#endif
  }

}
