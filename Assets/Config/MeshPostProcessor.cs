using UnityEditor;
using UnityEngine;

public class MeshPostProcessor : AssetPostprocessor
{
    private void OnPreprocessModel()
    {
        ModelImporter modelImporter = assetImporter as ModelImporter;
        if (modelImporter != null)
        {
            modelImporter.globalScale = 1;
        }
    }
    
}
