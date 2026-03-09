using System.Collections;
using UnityEngine;

public static class Utility
{
    public static Vector3 Multiply(Vector3 v1, Vector3 v2)
    {
        Vector3 result = new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        return result;
    }

    public static bool CheckZero(float[] values)
    {
        foreach (float value in values)
        {
            if (value != 0) return false;
        }
        return true;
    }

    public static bool CheckZeroVec(Vector3[] vecs)
    {
        foreach (Vector3 v in vecs)
        {
            if (v.Equals(Vector3.zero)) return true;
        }
        return false;
    }

    public static bool IsSubstringInString(string mainString, string subString)
    {
        // Utilisez la m�thode Contains pour v�rifier si la sous-cha�ne est pr�sente dans la cha�ne
        return mainString.Contains(subString);
    }

    public static int FindStringIndex(string[] array, string target)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(target))
            {
                return i; // Renvoie la position d�s que la cha�ne est trouv�e
            }
        }
        return -1;
    }

    public static string[] ExtractGameObjectNames(GameObject[] gameObjects)
    {
        string[] names = new string[gameObjects.Length];

        for (int i = 0; i < gameObjects.Length; i++)
        {
            names[i] = gameObjects[i].name;
        }

        return names;
    }

    public static bool RayPlaneIntersection(Ray p_ray, Vector3 p_planePos, Vector3 p_planeN, out float p_intersection)
    {
        //Debug.Log("RayPlaneI: " + p_ray + " - " + p_planeN + " - " + p_planePos);
        float denom = Vector3.Dot(p_planeN.normalized, p_ray.direction.normalized);
        //Debug.Log("Denom: " + denom.ToString("F6"));
        if (Mathf.Abs(denom) > 1e-6)
        {
            Vector3 p0l0 = (p_planePos - p_ray.origin);
            p_intersection = Vector3.Dot(p0l0, p_planeN) / denom;
            //Debug.Log("Inters: " + p_intersection.ToString("F3"));
            return p_intersection >= 0;
        }
        //Debug.Log("No inters denom");
        p_intersection = 0f;
        return false;
    }

    public static Bounds CalculateLocalBounds(GameObject p_object)
    {
        Quaternion currentRotation = p_object.transform.rotation;
        p_object.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Bounds bounds = new Bounds(p_object.transform.position, Vector3.zero);
        foreach (Renderer renderer in p_object.GetComponentsInChildren<Renderer>())
        {
            if (renderer.gameObject.layer != LayerMask.NameToLayer("PMRZone") && renderer.gameObject.layer != LayerMask.NameToLayer("ManipulationHelper"))
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        Vector3 localCenter = bounds.center - p_object.transform.position;
        bounds.center = localCenter;
        p_object.transform.rotation = currentRotation;

        return bounds;
    }

    public static Bounds GetLocalBounds(GameObject p_object, BoxCollider p_collider)
    {
        Bounds bounds = new Bounds(p_object.transform.position, Vector3.zero);
        if (p_collider == null)
            return bounds;
        Vector3 localCenter = p_collider.center - p_object.transform.position;
        bounds.center = localCenter;
        bounds.size = new Vector3(p_collider.size.x * p_collider.gameObject.transform.localScale.x, p_collider.size.y * p_collider.gameObject.transform.localScale.y, p_collider.size.z * p_collider.gameObject.transform.localScale.z);
        bounds.extents = bounds.size * 0.5f;
        return bounds;
    }

    public static void SetMaterialTransparent(Material p_material)
    {
        p_material.SetOverrideTag("RenderType", "Transparent");
        p_material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        p_material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        p_material.SetInt("_ZWrite", 0);
        p_material.DisableKeyword("_ALPHATEST_ON");
        p_material.EnableKeyword("_ALPHABLEND_ON");
        p_material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        p_material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    public static void SetMaterialOpaque(Material p_material)
    {
        p_material.SetOverrideTag("RenderType", "Opaque");
        p_material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        p_material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        p_material.SetInt("_ZWrite", 1);
        p_material.DisableKeyword("_ALPHATEST_ON");
        p_material.EnableKeyword("_ALPHABLEND_ON");
        p_material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        p_material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
    }
} 