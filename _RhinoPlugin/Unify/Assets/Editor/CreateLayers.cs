using UnityEngine;
using UnityEditor;

public class LayerUtils : MonoBehaviour
{
    private static bool resourcesLoaded;

    [MenuItem("Unify/1.ImportAssets")]
    static void LayerUtilsCall()
    {
        CreateLayer();
        ImportAssets();
    }

    static void ImportAssets()
    {
        // only load resources once
        if (!resourcesLoaded)
        {
            resourcesLoaded = !resourcesLoaded;
            string path = Application.dataPath + "/Resources/UnifyPackage.unitypackage";
            AssetDatabase.ImportPackage(path, false);
            AssetDatabase.Refresh();
        }

        // print debug message
        Debug.Log("Unify Assets were loaded.");
    }

    static void CreateLayer()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty layers = tagManager.FindProperty("layers");
        if (layers == null || !layers.isArray)
        {
            Debug.LogWarning("Can't set up the layers.  It's possible the format of the layers and tags data has changed in this version of Unity.");
            Debug.LogWarning("Layers is null: " + (layers == null));
            return;
        }

        string[] layersToCreate = new string[]{ "UnifyShow", "UnifyHide"};
        int index = 8;

        foreach (string layer in layersToCreate)
        {
            SerializedProperty layerSP = layers.GetArrayElementAtIndex(index);
            index += 1;
            if (layerSP.stringValue != layer)
            {
                Debug.Log("Setting up layers.  Layer " + index + " is now called " + layer);
                layerSP.stringValue = layer;
            }
            tagManager.ApplyModifiedProperties();
        }
    }
}

