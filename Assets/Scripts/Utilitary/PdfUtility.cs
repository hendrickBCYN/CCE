using UnityEngine;

public static class PdfUtility
{
    public enum ScaleInCm
    {
        _1_50,
        _1_100,
        _1_250
    };

    public static string ScaleInCmToText(ScaleInCm scaleInCm)
    {
        string result = "1 : 1";

        switch (scaleInCm)
        {
            case ScaleInCm._1_50:
                result = "1 : 50";
                break;
            case ScaleInCm._1_100:
                result = "1 : 100";
                break;
            case ScaleInCm._1_250:
                result = "1 : 250";
                break;
            default:
                result = "1 : 1";
                break;
        }
        return result;
    }

    public static float ScaleEnumToFloat(ScaleInCm scale)
    {
        switch(scale)
        {
            case ScaleInCm._1_50:
                return 1 / 50f;
            case ScaleInCm._1_100:
                return 1 / 100f;
            case ScaleInCm._1_250:
                return 1 / 250f;
            default:
                Debug.Log($"Unsupported scale value");
                return 1 / 50f; // default scale value
        }
    }

    public static float ConvertCmToPoints(float p_imageSizeInCm)
    {
        float l_imageSizeInPoints = p_imageSizeInCm * 28.34644f;   
        return l_imageSizeInPoints;
    }

     public static Sprite ConvertTexture2DToSprite(Texture2D p_texture)
    {
        Sprite sprite = Sprite.Create(
            p_texture, 
            new Rect(0, 0, p_texture.width, p_texture.height), // area of ​​the texture to use
            new Vector2(0.5f, 0.5f) // pivot point of the Sprite (center here)
        );

        return sprite;
    }

    public static RenderTexture ConvertTexture2DToRenderTexture(Texture2D p_texture2D, int p_depth = 24)
    {
        RenderTexture renderTexture = new RenderTexture(p_texture2D.width, p_texture2D.height, p_depth);
        RenderTexture.active = renderTexture;

        Graphics.Blit(p_texture2D, renderTexture); // Copy data from Texture2D to RenderTexture

        RenderTexture.active = null;

        return renderTexture;
    }

    public static Texture2D ConvertRenderTextureToTexture2D(RenderTexture p_renderTexture)
    {
        RenderTexture currentRenderTexture = RenderTexture.active;
        RenderTexture.active = p_renderTexture;

        Texture2D texture2D = new Texture2D(p_renderTexture.width, p_renderTexture.height, TextureFormat.RGB24, false);
        texture2D.ReadPixels(new Rect(0, 0, p_renderTexture.width, p_renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRenderTexture;
        return texture2D;
    }

    public static byte[] ConvertTexture2DToByteArray(Texture2D p_texture)
    {
        byte[] l_bytes = p_texture.EncodeToPNG();
        return l_bytes;
    }

    public static byte[][] ConvertTexture2DArrayToByteArray(Texture2D[] p_textures)
    {
        byte[][] l_bytes = new byte[p_textures.Length][];

        for (int i = 0; i < p_textures.Length; i++)
        {
            if (p_textures[i] != null)
            {
                l_bytes[i] = p_textures[i].EncodeToPNG(); 
            }
            else
            {
                Debug.Log($"Utility - ConvertTexture2DArrayToByteArray - {p_textures[i]} is null");
                l_bytes[i] = null;
            }
        }
        return l_bytes;
    }
}
