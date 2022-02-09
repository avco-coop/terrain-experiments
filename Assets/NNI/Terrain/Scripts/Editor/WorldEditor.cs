//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;

namespace NNI.Terrain.Editor {

  [CustomEditor(typeof(World))]
  public class PlanetEditor : UnityEditor.Editor {

    World world;

    private MaterialEditor materialEditor;

    public void OnEnable () {
      world = target as World;
      if (world.material)
        materialEditor = (MaterialEditor) CreateEditor(world.material);
    }

    public void OnDisable () {
      DestroyImmediate(materialEditor);
    }

    public override void OnInspectorGUI () {
      EditorGUI.BeginChangeCheck();
      serializedObject.Update();
      GUILayout.Button("Regenerate");
      DrawDefaultInspector();
      if (EditorGUI.EndChangeCheck())
        world.dirty = true;
      if (materialEditor) {
        GUILayout.Space(4);
        materialEditor.DrawHeader();
        var isDefaultMaterial = !AssetDatabase.GetAssetPath(world.material).StartsWith("Assets");
        using (new EditorGUI.DisabledGroupScope(isDefaultMaterial)) {
          materialEditor.OnInspectorGUI();
        }
      }
    }
  }

}
