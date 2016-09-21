using UnityEngine;
using System;
using System.IO;


public class Utilities : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    //public static Texture2D Texture2dFromPath(string filePath)
    //{
    //    string textureName = System.IO.Path.GetFileName(filePath);
    //    string texturePath = Application.dataPath + "/Resources/" + textureName;
    //    byte[] fileData;
    //    Texture2D mainTex;
    //    if (File.Exists(texturePath))
    //    {
    //        fileData = File.ReadAllBytes(texturePath);
    //        mainTex = new Texture2D(2, 2);
    //        mainTex.LoadImage(fileData);
    //        return mainTex;
    //    }
    //    else
    //    {
    //        throw new Exception("File doesn't exist at path.");
    //    }
    //}

    public static Texture2D CreateNormalMap(Texture2D source, float strength)
    {
        strength = Mathf.Clamp(strength, 0.0F, 1.0F);

        Texture2D normalTexture;
        float xLeft;
        float xRight;
        float yUp;
        float yDown;
        float yDelta;
        float xDelta;

        normalTexture = new Texture2D(source.width, source.height, TextureFormat.ARGB32, true);

        for (int y = 0; y < normalTexture.height; y++)
        {
            for (int x = 0; x < normalTexture.width; x++)
            {
                xLeft = source.GetPixel(x - 1, y).grayscale * strength;
                xRight = source.GetPixel(x + 1, y).grayscale * strength;
                yUp = source.GetPixel(x, y - 1).grayscale * strength;
                yDown = source.GetPixel(x, y + 1).grayscale * strength;
                xDelta = ((xLeft - xRight) + 1) * 0.5f;
                yDelta = ((yUp - yDown) + 1) * 0.5f;
                normalTexture.SetPixel(x, y, new Color(xDelta, yDelta, 1.0f, yDelta));
            }
        }
        normalTexture.Apply();
        return normalTexture;
    }

    public static Color ConvertToUnityColor(string colorString, double valueA = 1)
    {
        string[] colorArr = colorString.Split(',');
        float r = Utilities.ConvertRange(0, 255, 0, 1, Convert.ToSingle(colorArr[0]));
        float g = Utilities.ConvertRange(0, 255, 0, 1, Convert.ToSingle(colorArr[1]));
        float b = Utilities.ConvertRange(0, 255, 0, 1, Convert.ToSingle(colorArr[2]));
        float a = Convert.ToSingle(valueA);

        return new Color(r, g, b, a);
    }

    public static Vector3 TranslateFromRhinoCS(string location)
    {
        // extract coordinates from string
        string[] arr = location.Split(',');
        float x = Convert.ToSingle(arr[0]);
        float y = Convert.ToSingle(arr[1]);
        float z = Convert.ToSingle(arr[2]);

        // correct for left handed coordinate system of Unity
        float unityX = x * -1;
        float unityY = z;
        float unityZ = y * -1;

        return new Vector3(unityX, unityY, unityZ);
    }

    public static float ConvertRange(
        float originalStart,
        float originalEnd,
        float newStart,
        float newEnd,
        float value)
    {
        float scale = (float)(newEnd - newStart) / (originalEnd - originalStart);
        return (float)(newStart + ((value - originalStart) * scale));
    }

    public static float RadiansToDegrees(double angle)
    {
        return Convert.ToSingle(angle * (180.0 / Math.PI));
    }
}
