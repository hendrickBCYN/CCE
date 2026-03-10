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

#if UNITY_WEBGL && !UNITY_EDITOR
        // En WebGL : sérialise en mémoire (pas d'écriture fichier)
        try
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConfigurationData));
            using (StringWriter stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, l_data);
                string xmlContent = stringWriter.ToString();
                // On retourne un nom fictif pour indiquer le succès
                // Les données transitent via NetworkManager/React
                return "webgl_config.xml";
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
            return "";
        }
#else
        // En Standalone/Editor : comportement fichier original
        XmlSerializer xmlSerializerLocal = new XmlSerializer(typeof(ConfigurationData));
        string l_folderPath = FileDialogManager.OpenFolder();
        if (l_folderPath != null)
            Debug.Log("Folder: " + l_folderPath);

        string fileName = p_configManager.ConfigurationName + ".xml";
        Debug.Log("Filename = " + fileName);
        try
        {
            StreamWriter streamWriter = new StreamWriter(l_folderPath + "/" + fileName);
            xmlSerializerLocal.Serialize(streamWriter, l_data);
            streamWriter.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
            fileName = "";
        }
        return fileName;
#endif
    }

    public ConfigurationData LoadSerializedConfiguration(ConfigurationManager p_configManager)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // En WebGL : le chargement passe par React/NetworkManager
        Debug.LogWarning("ConfigurationSerialization Chargement WebGL non supporté via fichier.");
        return null;
#else
        string l_folderPath = FileDialogManager.OpenFolder();
        string fileName = p_configManager.ConfigurationName + ".xml";
        if (File.Exists(l_folderPath + "/" + fileName))
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConfigurationData));
            StreamReader streamReader = new StreamReader(l_folderPath + "/" + fileName);
            ConfigurationData l_data = xmlSerializer.Deserialize(streamReader) as ConfigurationData;
            streamReader.Close();
            return l_data;
        }
        return null;
#endif
    }
}
