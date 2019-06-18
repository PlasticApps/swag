using UnityEditor;

namespace SW
{
    public static class BuildAllAssets
    {
        [MenuItem("Assets/Bundles Build/Android")]
        static void BuildAndroidAssets()
        {
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/Android/", BuildAssetBundleOptions.None,
                BuildTarget.Android);
        }
    }
}