using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConfigurationManager))]
public class ConfiguratorManagerEditor: Editor
{
    SerializedProperty _configurationName;
    SerializedProperty _configurationGroups;
    SerializedProperty _currentConfigurationName;
    SerializedProperty _configurationEvent;

    SerializedProperty _costCoeff;

    SerializedProperty _currentConfigurationTotalCost;
    SerializedProperty _costDetails;

    private void OnEnable()
    {
        _configurationName = serializedObject.FindProperty("_configurationName");
        _configurationGroups = serializedObject.FindProperty("_configurationGroups");
        _currentConfigurationName = serializedObject.FindProperty("_currentConfigurationName");
        _configurationEvent = serializedObject.FindProperty("_configurationEvent");
        _costCoeff = serializedObject.FindProperty("_costCoeff");
        _currentConfigurationTotalCost = serializedObject.FindProperty("_currentConfigurationTotalCost");
        _costDetails = serializedObject.FindProperty("_costDetails");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_configurationName);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_costCoeff);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            (target as ConfigurationManager).ComputeTotalCost();
        }
        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.PropertyField(_configurationGroups);
        EditorGUILayout.PropertyField(_configurationEvent);
        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.PropertyField(_currentConfigurationTotalCost);
        EditorGUILayout.PropertyField(_costDetails);

        serializedObject.ApplyModifiedProperties();
    }

    public void BakeConfigurations()
    {
        EditorCoroutineUtility.StartCoroutine(BakeConfigurationsRoutine(), this);
    }

    IEnumerator BakeConfigurationsRoutine()
    {
        if (_configurationGroups != null && _configurationGroups.arraySize >= 4)
        {
            for (int aGroup = 0; aGroup < (_configurationGroups.GetArrayElementAtIndex(0).objectReferenceValue as ConfigurationGroup).NbOptions; aGroup++)
            {
                (_configurationGroups.GetArrayElementAtIndex(0).objectReferenceValue as ConfigurationGroup).SetOption(aGroup, true);
                (target as ConfigurationManager).UpdateConfigurationName();
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                Debug.Log("CurrentConfiguration : " + _currentConfigurationName.stringValue);
                yield return new WaitForSecondsRealtime(5f);
                BakeConfiguration();
                AssetDatabase.Refresh();
                yield return new WaitForSeconds(0.1f);
                
                /*for (int bGroup = 0; bGroup < _configurationGroups[1].NbOptions; bGroup++)
                {
                    _configurationGroups[1].SetOption(bGroup);
                    for (int cGroup = 0; cGroup < _configurationGroups[2].NbOptions; cGroup++)
                    {
                        _configurationGroups[2].SetOption(cGroup);
                        for (int dGroup = 0; dGroup < _configurationGroups[3].NbOptions; dGroup++)
                        {
                            _currentConfigurationName = aGroup.ToString() + "_" + bGroup.ToString() +"_" + cGroup.ToString() + "_" + dGroup.ToString();
                            _configurationGroups[3].SetOption(dGroup);
                            BakeConfiguration();
                        }
                    }
                }*/
            }
        }
    }

    void BakeConfiguration()
    {
        ConfigurationManager l_manager = (target as ConfigurationManager);
        string newFolder = "Assets/LightMaps/" + _currentConfigurationName.stringValue;
        if (!AssetDatabase.IsValidFolder(newFolder))
        {
            AssetDatabase.CreateFolder("Assets/LightMaps", _currentConfigurationName.stringValue);
        }

        if (Lightmapping.Bake())
        {
            Debug.Log("Lightmapping " + _currentConfigurationName.stringValue + " baked !");

            if (AssetDatabase.IsValidFolder("Assets/Scenes/Main"))
            {
                Debug.Log("Scenes/Main is valid");
                string[] guidsToCopy = AssetDatabase.FindAssets("", new[] { "Assets/Scenes/Main" });
                foreach (string guidToCopy in guidsToCopy)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guidToCopy);
                    Debug.Log($"{guidToCopy} -> {assetPath}");
                    string fileName = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
                    Debug.Log("Filename: " + fileName + " -> " + newFolder + fileName);
                    AssetDatabase.CopyAsset(assetPath, newFolder + "/" + fileName);
                }
            }
        }
        else
        {
            Debug.LogWarning("Lightmapping " + _currentConfigurationName.stringValue + " failed to bake !");
        }
    }

    /*void BakeConfigurationZero()
    {
        if (_configurationGroups != null && _configurationGroups.Length >= 4)
        {
            _currentConfigurationName = "0_0_0_0";
            _configurationGroups[0].SetOption(0, true);
            _configurationGroups[1].SetOption(0, true);
            _configurationGroups[2].SetOption(0, true);
            _configurationGroups[3].SetOption(0, true);

            BakeConfiguration();
        }
    }*/
}
