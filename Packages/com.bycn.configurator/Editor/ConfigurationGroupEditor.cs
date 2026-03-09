using System.Collections;
using System.Collections.Generic;
using UnityEditor;


[CustomEditor(typeof(ConfigurationGroup))]
public class ConfigurationGroupEditor : Editor
{
    SerializedProperty _name;
    SerializedProperty _configuredObjects;
    SerializedProperty _messagedObjects;
    //SerializedProperty _temporaryConfiguredObjects;
    SerializedProperty _configurationOptions;
    SerializedProperty _configurationOptionsUIGroup;

    SerializedProperty _configurationGroupType;
    SerializedProperty _nbPieces;
    SerializedProperty _surface;
    SerializedProperty _metres;

    SerializedProperty _contributeToTotalCost;
    SerializedProperty _totalCost;

    SerializedProperty _configurationEvent;

    virtual protected void OnEnable()
    {
        _name = serializedObject.FindProperty("_name");
        _configuredObjects = serializedObject.FindProperty("_configuredObjects");
        _messagedObjects = serializedObject.FindProperty("_messagedObjects");

        //_temporaryConfiguredObjects = serializedObject.FindProperty("_temporaryConfiguredObjects"); ;

        _configurationOptions = serializedObject.FindProperty("_configurationOptions");
        _configurationGroupType = serializedObject.FindProperty("_configurationGroupType");
        _configurationOptionsUIGroup = serializedObject.FindProperty("_uiOptionGroup");

        _nbPieces = serializedObject.FindProperty("_nbPieces");
        _surface = serializedObject.FindProperty("_surface");
        _metres = serializedObject.FindProperty("_metres");
        _contributeToTotalCost = serializedObject.FindProperty("_contributeToTotalCost");
        _totalCost = serializedObject.FindProperty("_totalCost");

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
        EditorGUILayout.PropertyField(_name);
        EditorGUILayout.PropertyField(_configuredObjects);
        EditorGUILayout.PropertyField(_messagedObjects);
        //EditorGUILayout.PropertyField(_temporaryConfiguredObjects);
        EditorGUILayout.PropertyField(_configurationGroupType);
        EditorGUILayout.PropertyField(_contributeToTotalCost);

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        switch ((ConfigurationGroup.ConfigurationGroupType)_configurationGroupType.intValue)
        {
            case ConfigurationGroup.ConfigurationGroupType.Piece:
                EditorGUILayout.PropertyField(_nbPieces);
                break;
            case ConfigurationGroup.ConfigurationGroupType.Surface:
                EditorGUILayout.PropertyField(_surface); break;
            case ConfigurationGroup.ConfigurationGroupType.MetreLineaire: 
                EditorGUILayout.PropertyField(_metres); break;
            default:
                break;
        }
        EditorGUILayout.PropertyField(_totalCost);

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.PropertyField(_configurationOptions);
        EditorGUILayout.PropertyField(_configurationOptionsUIGroup);
        EditorGUILayout.PropertyField(_configurationEvent);
    }
}
