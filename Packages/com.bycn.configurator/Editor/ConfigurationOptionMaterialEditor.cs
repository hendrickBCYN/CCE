using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(ConfigurationOptionMaterial))]
public class ConfigurationOptionMaterialEditor : ConfigurationOptionEditor
{
    SerializedProperty _material;

    protected override void OnEnable()
    {
        base.OnEnable();

        _material = serializedObject.FindProperty("_material");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawOptionEditor();
        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.PropertyField(_material);
        serializedObject.ApplyModifiedProperties();
    }
}
