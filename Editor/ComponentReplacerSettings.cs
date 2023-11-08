using System.IO;
using UnityEditor;
using UnityEngine;

namespace SnUnityCommonUtils.Utils
{
    internal class ComponentReplacerSettings : ScriptableObject
    {
        private const string SettingsPath = "Assets/Editor/Settings/ComponentReplacerSettings.asset";
        
        [SerializeField] internal bool _searchInAllAssemblies;
        [SerializeField] internal string[] _ignoredAssemblies;
        [SerializeField] internal string[] _ignoredBaseClasses;

        internal static ComponentReplacerSettings GetOrCreateSettings()
        {
            return GetSettings() ?? CreateSettings();
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        private static ComponentReplacerSettings GetSettings()
        {
            return AssetDatabase.LoadAssetAtPath<ComponentReplacerSettings>(SettingsPath);
        }

        private static ComponentReplacerSettings CreateSettings()
        {
            CreateDirectoryFromAssetPath(SettingsPath);
            
            var settings = CreateInstance<ComponentReplacerSettings>();
            settings._ignoredAssemblies = new[]
            {
                "UnityEngine.*", "Unity.*", "UnityEditor.*", "UnityEditor", "UnityEngine", "System", "mscorlib", "System.*"
            };
            AssetDatabase.CreateAsset(settings, SettingsPath);
            AssetDatabase.SaveAssets();
            return settings;
        }

        private static void CreateDirectoryFromAssetPath(string assetPath)
        {
            var directoryPath = Path.GetDirectoryName(assetPath);
            if (Directory.Exists(directoryPath))
                return;
            
            Directory.CreateDirectory(directoryPath);
            AssetDatabase.Refresh();
        }
    }
}