using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(ConfigurationOption))]
public class ConfigurationOptionEditor : Editor
{
    SerializedProperty _optionName;
    SerializedProperty _optionType;

    SerializedProperty _optionReference;

    SerializedProperty _hasDimensions;
    SerializedProperty _optionDimensions;

    SerializedProperty _hasCost;
    SerializedProperty _optionCostType;
    SerializedProperty _optionCost;
    SerializedProperty _optionCostTotal;

    SerializedProperty _optionImage;

    SerializedProperty _configurationEvent;

    virtual protected void OnEnable()
    {
        _optionName = serializedObject.FindProperty("_optionName");
        _optionType = serializedObject.FindProperty("_optionType");

        _optionReference = serializedObject.FindProperty("_optionReference"); ;

        _hasDimensions = serializedObject.FindProperty("_hasDimensions");
        _optionDimensions = serializedObject.FindProperty("_optionDimensions");

        _hasCost = serializedObject.FindProperty("_hasCost");
        _optionCostType = serializedObject.FindProperty("_optionCostType");
        _optionCost = serializedObject.FindProperty("_optionCost");
        _optionCostTotal = serializedObject.FindProperty("_optionCostTotal");

        _optionImage = serializedObject.FindProperty("_optionImage");

        _configurationEvent = serializedObject.FindProperty("_configurationEvent");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawOptionEditor();
        serializedObject.ApplyModifiedProperties();
    }

    protected void DrawOptionEditor()
    {
        EditorGUILayout.PropertyField(_optionName);
        EditorGUILayout.PropertyField(_optionType);
        EditorGUILayout.PropertyField(_optionReference);

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.PropertyField(_hasDimensions);
        if (_hasDimensions.boolValue)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(_optionDimensions);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.PropertyField(_hasCost);
        if (_hasCost.boolValue)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(_optionCostType);
            EditorGUILayout.PropertyField(_optionCost);
            EditorGUILayout.PropertyField(_optionCostTotal);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.PropertyField(_optionImage);
        EditorGUILayout.PropertyField(_configurationEvent);
    }
}
