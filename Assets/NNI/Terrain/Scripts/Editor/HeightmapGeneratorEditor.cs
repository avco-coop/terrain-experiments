//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;

namespace NNI.Terrain.Editor {

  [CustomEditor(typeof(HeightmapGenerator))]
  public class HeightmapGeneratorEditor : UnityEditor.Editor {

    HeightmapGenerator generator;

    Task task;

    public void OnEnable () {
      generator = target as HeightmapGenerator;
    }

    public void Generate () {
      task?.Dispose();
      task = new Task(generator.Generate, this, "Generating heightmap");
    }

    public override void OnInspectorGUI () {
      serializedObject.Update();
      if (GUILayout.Button("Generate"))
        Generate();
      DrawDefaultInspector();
      // serializedObject.ApplyModifiedProperties();
    }
  }

}
