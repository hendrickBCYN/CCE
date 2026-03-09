using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(ConfigurationOptionObject))]
public class ConfigurationOptionObjectEditor : ConfigurationOptionEditor
{
    SerializedProperty _object;
    SerializedProperty _position;
    SerializedProperty _orientation;
    SerializedProperty _scale;

    protected override void OnEnable()
    {
        base.OnEnable();

        _object = serializedObject.FindProperty("_object");
        _position = serializedObject.FindProperty("_position");

        _orientation = serializedObject.FindProperty("_orientation"); ;

        _scale = serializedObject.FindProperty("_scale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawOptionEditor();
        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.PropertyField(_object);
        EditorGUILayout.PropertyField(_position);
        EditorGUILayout.PropertyField(_orientation);
        EditorGUILayout.PropertyField(_scale);
        serializedObject.ApplyModifiedProperties();
    }
}
