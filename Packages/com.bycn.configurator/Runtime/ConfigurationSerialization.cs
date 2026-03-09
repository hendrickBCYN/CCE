using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class ConfigurationSerialization
{
    public string SerializeConfiguration(ConfigurationManager p_configManager)
    {
        Debug.Log("SerializeConfiguration");
        ConfigurationData l_data = new ConfigurationData();
        l_data.Init(p_configManager);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConfigurationData));

        string l_folderPath = FileDialogManager.OpenFolder();
        if (l_folderPath != null)
            Debug.Log("Folder: " + l_folderPath);
        //Debug.Log("Check StreamingAssets path : " + Application.streamingAssetsPath);
        //
        //else Debug.Log("Folder StreamingAssets already exists");
        string fileName = p_configManager.ConfigurationName + ".xml";
        Debug.Log("Filename = " + fileName);
        try
        {
            StreamWriter streamWriter = new StreamWriter(l_folderPath + "/" + fileName);
            xmlSerializer.Serialize(streamWriter, l_data);
            streamWriter.Close();
        } catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
            fileName = "";
        }
        
        return fileName;
    }

    public ConfigurationData LoadSerializedConfiguration(ConfigurationManager p_configManager)
    {
        string l_folderPath = FileDialogManager.OpenFolder();
        string fileName = p_configManager.ConfigurationName + ".xml";
        if (File.Exists(l_folderPath + "/" + fileName))
        {
            XmlSerializer xmlSerializer = new XmlSerializer (typeof(ConfigurationData));
            StreamReader streamReader = new StreamReader(l_folderPath + "/" + fileName);
            ConfigurationData l_data = xmlSerializer.Deserialize(streamReader) as ConfigurationData;
            streamReader.Close();
            return l_data;
        }

        return null;
    }
}
