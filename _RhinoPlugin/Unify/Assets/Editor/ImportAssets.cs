using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Unify.UnifyCommon;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

/// <summary>
///     Loads obj object every time its run for re-fresh purpose.
///     Only loads HUD and UnifyPackage once. There is no need to reload those.
/// </summary>
public class ImportAssets : MonoBehaviour
{
    private static Dictionary<string, List<object>> deserialized;
    private static bool hudLoaded;

    [MenuItem("Unify/2.ProcessAssets")]
    static void ProcessAssets()
    {
        DeserializeJson();
        ImportModel();
        ProcessLights();
        ProcessMaterials();
        ProcessMeshColliders();
        ProcessCharacter();
    }

    private static void DeserializeJson()
    {
        // load settings from assets
        TextAsset ta = Resources.Load("UnifySettings") as TextAsset;
        string json = ta.text;
        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };
        deserialized = JsonConvert.DeserializeObject<Dictionary<string, List<object>>>(json, settings);
    }

    private static void ImportModel()
    {
        // create parent object that obj will live in
        GameObject objParent;
        if (GameObject.Find("Model") == null)
        {
            objParent = new GameObject("Model");
            objParent.transform.position = new Vector3(0f, 0f, 0f);
        }
        else
        {
            objParent = GameObject.Find("Model");
        }

        // get meta data from deserialized JSON string
        Dictionary<string, string> metaData = ((UnifyMetaData)deserialized["MetaData"][0]).MetaData;

        // get obj name and load into assets
        string objName = metaData["OBJName"];

        // load obj model into parent object
        if (GameObject.Find(Path.GetFileNameWithoutExtension(objName)) == null)
        {
            var go = Instantiate(Resources.Load("Model/" + Path.GetFileNameWithoutExtension(objName)));
            ((GameObject)go).transform.parent = objParent.transform;
            go.name = go.name.Substring(0, go.name.IndexOf("("));
        }

        // delete main camera if such exists
        // it gets created by default on a new project
        if (GameObject.FindGameObjectWithTag("MainCamera") != null)
        {
            DestroyImmediate(GameObject.FindGameObjectWithTag("MainCamera"));
            AssetDatabase.Refresh();
        }

        // only instantiate HUD once
        if (!hudLoaded)
        {
            hudLoaded = !hudLoaded;
            var hud = Instantiate(Resources.Load("UnifyPrefabs"));
            ((GameObject)hud).transform.position = new Vector3(0f, 0f, 0f);
            hud.name = hud.name.Substring(0, hud.name.IndexOf("("));
        }

        // print debug message
        Debug.Log("Model was imported.");
    }

    private static void ProcessLights()
    {
        List<UnifyLight> allLights = deserialized["Lights"].Cast<UnifyLight>().ToList();

        // create parent object that all lights will live in
        GameObject lightsParent;
        if (GameObject.Find("Lights") == null)
        {
            lightsParent = new GameObject("Lights");
            lightsParent.transform.position = new Vector3(0f, 0f, 0f);
        }
        else
        {
            lightsParent = GameObject.Find("Lights");
        }

        foreach (UnifyLight obj in allLights)
        {
            if (!obj.Deleted)
            {
                CreateLight(obj, lightsParent);
            }
            else
            {
                try
                {
                    // delete asset from database
                    AssetDatabase.DeleteAsset("Assets/Resources/Lights/" + obj.LightType + "_" + obj.Guid);
                    // delete object from scene
                    GameObject lighObj = GameObject.Find(obj.LightType + "_" + obj.Guid);
                    if (lighObj != null)
                    {
                        DestroyImmediate(lighObj);
                    }
                }
                catch (Exception) { }
            }
        }
        AssetDatabase.Refresh();

        // print debug message
        Debug.Log("Lights were processed.");
    }

    private static void CreateLight(UnifyLight obj, GameObject parent)
    {
        GameObject lightGameObj;
        Light lightComp;
        if (GameObject.Find(obj.LightType + "_" + obj.Guid) == null)
        {
            lightGameObj = new GameObject(obj.LightType + "_" + obj.Guid);
            lightComp = lightGameObj.AddComponent<Light>();
        }
        else
        {
            lightGameObj = GameObject.Find(obj.LightType + "_" + obj.Guid);
            lightComp = lightGameObj.GetComponent<Light>();
        }

        // set light type
        switch (obj.LightType)
        {
            case "WorldSpot":
                {
                    lightComp.type = LightType.Spot;

                    // set range
                    lightComp.range = Convert.ToSingle(obj.Range);

                    // set angle
                    lightComp.spotAngle = Utilities.ConvertRange(0, 90, 0, 180, Utilities.RadiansToDegrees(obj.SpotAngle));
                    break;
                }
            case "WorldDirectional":
                {
                    lightComp.type = LightType.Directional;
                    break;
                }
            case "WorldPoint":
                {
                    lightComp.type = LightType.Point;

                    // set range
                    // range settings doesn't exist in Rhino
                    lightComp.range = 5.0f;
                    break;
                }
            case "WorldRectangular":
                {
                    lightComp.type = LightType.Area;

                    // set width and height
                    lightComp.areaSize = new Vector2(Convert.ToSingle(obj.Length), Convert.ToSingle(obj.Width));
                    break;
                }
            default:
                {
                    Debug.Log("No Light Type found.");
                    break;
                }
        }

        // set color
        lightComp.color = Utilities.ConvertToUnityColor(obj.Diffuse);

        // set location
        lightGameObj.transform.position = Utilities.TranslateFromRhinoCS(obj.Location);

        // set target
        GameObject tempObj = new GameObject("Temp");
        tempObj.transform.position = Utilities.TranslateFromRhinoCS(obj.Target);
        lightGameObj.transform.LookAt(tempObj.transform.position);
        DestroyImmediate(tempObj);

        // set intensity
        lightComp.intensity = Utilities.ConvertRange(0, 1, 0, 8, Convert.ToSingle(obj.Intensity));

        // set shadows
        lightComp.shadows = LightShadows.Soft;
        lightComp.shadowStrength = Convert.ToSingle(obj.ShadowIntensity);

        // create prefab
        string assetName = "Assets/Resources/Lights/" + obj.UniqueName + ".prefab";
        PrefabUtility.CreatePrefab(assetName, lightGameObj, ReplacePrefabOptions.ReplaceNameBased);

        // add light to parent
        lightGameObj.transform.parent = parent.transform;
    }

    private static void ProcessMaterials()
    {
        List<UnifyMaterial> allMaterials = deserialized["Materials"].Cast<UnifyMaterial>().ToList();

        ImportTextures(allMaterials);
        ApplyMaterials(allMaterials);

        // print debug message
        Debug.Log("Materials were processed.");
    }

    private static void ImportTextures(List<UnifyMaterial> allMaterials)
    {
        // copy textures over to unity folders
        HashSet<string> uniqueTextures = new HashSet<string>();
        foreach (UnifyMaterial obj in allMaterials)
        {
            if (obj.DiffuseTexture != null && uniqueTextures.Add(obj.DiffuseTexture))
            {
                CopyImageAsset(obj.DiffuseTexture);
            }
            if (obj.BumpTexture != null && uniqueTextures.Add(obj.BumpTexture))
            {
                CopyImageAsset(obj.BumpTexture);
            }
            if (obj.TransparencyTexture != null && uniqueTextures.Add(obj.TransparencyTexture))
            {
                CopyImageAsset(obj.TransparencyTexture);
            }
            if (obj.EnvironmentTexture != null && uniqueTextures.Add(obj.EnvironmentTexture))
            {
                CopyImageAsset(obj.EnvironmentTexture);
            }
        }
        AssetDatabase.Refresh();
    }

    private static void CopyImageAsset(string sourceFilePath)
    {
        string fileName = "Resources\\" + Path.GetFileName(sourceFilePath);
        string destFile = Path.Combine(Application.dataPath, fileName);

        try
        {
            File.Copy(sourceFilePath, destFile, true);
        }
        catch (Exception) { }
    }

    private static void ApplyMaterials(List<UnifyMaterial> allMaterials)
    {
        GameObject cube;
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MeshRenderer cubeRenderer = cube.GetComponent<MeshRenderer>();

        foreach (UnifyMaterial obj in allMaterials)
        {
            // skip layers with no materials assigned/default
            if (obj.Guid != Guid.Empty)
            {
                // obj replaces all dashes in names with underscores hence material assets will have different names than in Rhino
                // if layers had dashes in their names
                string objUniqueName = obj.UniqueName.Replace("-", "_");
                Material m = (Material)AssetDatabase.LoadAssetAtPath("Assets/Resources/Model/Materials/" + objUniqueName + "Mat.mat", typeof(UnityEngine.Object));
                if (m != null)
                {
                    OverrideMaterial(m, obj, cubeRenderer);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(m));
                }
            }
        }
        DestroyImmediate(cube);
        AssetDatabase.Refresh();
    }

    private static Material OverrideMaterial(Material m, UnifyMaterial obj, MeshRenderer renderer)
    {
        renderer.material = m;
        // set main color
        // set transparency
        if (obj.Transparency != 0)
        {
            Color newColor = Utilities.ConvertToUnityColor(obj.Diffuse, obj.Transparency);
            renderer.sharedMaterial.SetFloat("_Mode", 3);
            renderer.sharedMaterial.SetColor("_Color", newColor);
        }
        else
        {
            Color newColor = Utilities.ConvertToUnityColor(obj.Diffuse);
            renderer.sharedMaterial.SetColor("_Color", newColor);
        }

        // set main texture
        if (obj.DiffuseTexture != null)
        {
            // load texture from assets
            string textureName = System.IO.Path.GetFileName(obj.DiffuseTexture);
            string texturePath = "Assets/Resources/" + textureName;
            Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));

            renderer.sharedMaterial.mainTexture = texture;
        }

        //set bump map
        if (obj.BumpTexture != null)
        {
            // load texture from assets
            string textureName = System.IO.Path.GetFileName(obj.BumpTexture);
            string texturePath = "Assets/Resources/" + textureName;
            Texture2D bumpTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));

            // re-import with readable flag set to true
            string p = AssetDatabase.GetAssetPath(bumpTexture);
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(p);
            ti.isReadable = true;
            AssetDatabase.ImportAsset(p);

            // convert texture to normal map and save to assets
            Texture2D normalBump = Utilities.CreateNormalMap(bumpTexture, 1.0f);
            string sysTexturePath = Application.dataPath + "/Resources/" + textureName.Remove(textureName.IndexOf(".")) + "_normal.png";
            File.WriteAllBytes(sysTexturePath, normalBump.EncodeToPNG());
            AssetDatabase.Refresh();

            // re-import normal texture with normalmap flag set to true
            string relativeTexturePath = "Assets/Resources/" + textureName.Remove(textureName.IndexOf(".")) + "_normal.png";
            Texture2D newBumpTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(relativeTexturePath, typeof(Texture2D));
            string p2 = AssetDatabase.GetAssetPath(newBumpTexture);
            TextureImporter ti2 = (TextureImporter)TextureImporter.GetAtPath(p2);
            ti2.normalmap = true;
            AssetDatabase.ImportAsset(p2);

            // assign normal map to material
            renderer.sharedMaterial.SetTexture("_BumpMap", newBumpTexture);

            // set bump scale
            renderer.sharedMaterial.SetFloat("_BumpScale", 0.3f);
        }

        // set metallic
        renderer.sharedMaterial.SetFloat("_Metallic", Utilities.ConvertRange(0, 255, 0, 1, Convert.ToSingle(obj.Metallic)));

        // set emission color
        Color emissionColor = Utilities.ConvertToUnityColor(obj.EmissionColor);
        renderer.sharedMaterial.SetColor("_EmissionColor", emissionColor);
        return renderer.sharedMaterial;
    }

    private static void ProcessMeshColliders()
    {
        List<UnifyLayer> allLayers = deserialized["Layers"].Cast<UnifyLayer>().ToList();

        foreach (UnifyLayer layer in allLayers)
        {
            GameObject go = GameObject.Find(layer.Name.Replace("-", "_"));
            switch (layer.MeshCollider)
            {
                case true:
                    if (go != null && go.GetComponent<MeshCollider>() == null)
                    {
                        go.AddComponent<MeshCollider>();
                    }
                    break;
                case false:
                    if (go != null && go.GetComponent<MeshCollider>() != null)
                    {
                        DestroyImmediate(go.GetComponent<MeshCollider>());
                    }
                    break;
            }
        }

        // print debug message
        Debug.Log("Mesh Colliders were processed.");
    }

    private static void ProcessCharacter()
    {
        List<UnifyCamera> allCameras = deserialized["Cameras"].Cast<UnifyCamera>().ToList();

        // get origin camera
        UnifyCamera originCam = allCameras.Where(x => x.IsPlayerOriginCamera).FirstOrDefault();

        // get fps controller
        var character = GameObject.FindGameObjectWithTag("Player");
        if (character != null)
        {
            character.transform.position = Utilities.TranslateFromRhinoCS(originCam.CameraLocation);

            // set camera direction
            GameObject tempObj = new GameObject("Temp");
            tempObj.transform.position = Utilities.TranslateFromRhinoCS(originCam.CameraTarget);
            character.transform.LookAt(tempObj.transform.position);
            DestroyImmediate(tempObj);

            // print debug message
            Debug.Log("Character was processed.");
        }
    }
}
