using UnityEngine;
using System.IO;

public static class LoadConfig
{

    public static LoadConfig.MaterialsJSON data;

    public static void Init()
    {
        // Charger le contenu du fichier JSON
        string jsonContent = LoadResourceTextfile("resourcesPath.json");

        // Désérialiser le JSON en objets C#
        data = JsonUtility.FromJson<LoadConfig.MaterialsJSON>(jsonContent);
    }

    public static string LoadResourceTextfile(string path)
    {

        string filePath = path.Replace(".json", "");

        TextAsset targetFile = Resources.Load<TextAsset>(filePath);

        return targetFile.text;
    }

    [System.Serializable]
    public class MaterialsJSON
    {
        public BrandMaterialsJSON[] Wall;
        public BrandMaterialsJSON[] Floor;
        public BrandMaterialsJSON[] Prefab;
    }

    [System.Serializable]
    public class BrandMaterialsJSON
    {
        public string Name;
        public string Path;
    }

    //TYPE -> 0 -> Wall, 1 -> Floor, 2 -> Prefab
    public static string FindPath(int id, string name)
    {
        if (id == 0)
        {
            for (int i = 0; i < data.Wall.Length; i++)
            {
                if (data.Wall[i].Name == name) return data.Wall[i].Path;
            }
        }
        else if (id == 1)
        {
            for (int i = 0; i < data.Floor.Length; i++)
            {
                if (data.Floor[i].Name == name) return data.Floor[i].Path;
            }
        }
        else if (id == 2)
        {
            for (int i = 0; i < data.Prefab.Length; i++)
            {
                if (data.Prefab[i].Name == name) return data.Prefab[i].Path;
            }
        }
        return "";
    }

    public static string[] GetPrefabPathsByName(string nameToFind)
    {
        System.Collections.Generic.List<string> prefabPaths = new System.Collections.Generic.List<string>();

        foreach (var prefabMaterial in data.Prefab)
        {
            if (Utility.IsSubstringInString(prefabMaterial.Name, nameToFind))
            {
                prefabPaths.Add(prefabMaterial.Path);
            }
        }

        return prefabPaths.ToArray();
    }

    public static string[] GetPrefabPathsByPath(string pathToFind)
    {
        System.Collections.Generic.List<string> prefabPaths = new System.Collections.Generic.List<string>();

        foreach (var prefabMaterial in data.Prefab)
        {
            if (Utility.IsSubstringInString(prefabMaterial.Path, pathToFind))
            {
                prefabPaths.Add(prefabMaterial.Path);
            }
        }

        return prefabPaths.ToArray();
    }
}
