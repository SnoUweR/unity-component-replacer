using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SnUnityCommonUtils.Utils
{
    internal class ComponentReplacerSettingsProvider : SettingsProvider
    {
        private const string SettingsPath = "Project/ComponentReplacerSettings";
        
        private SerializedProperty _searchInAllAssembliesProperty;
        private SerializedProperty _ignoredAssembliesProperty;
        private SerializedProperty _ignoredBaseClassesProperty;
        private SerializedObject _settings;

        private ComponentReplacerSettingsProvider() : base(SettingsPath, SettingsScope.Project)
        {
            label = "Component Replacer Settings";
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            
            _settings = ComponentReplacerSettings.GetSerializedSettings();
            
            _searchInAllAssembliesProperty = _settings.FindProperty(nameof(ComponentReplacerSettings._searchInAllAssemblies));
            _ignoredAssembliesProperty = _settings.FindProperty(nameof(ComponentReplacerSettings._ignoredAssemblies));
            _ignoredBaseClassesProperty = _settings.FindProperty(nameof(ComponentReplacerSettings._ignoredBaseClasses));
        }

        public override void OnDeactivate()
        {
            _settings = null;
            _searchInAllAssembliesProperty = null;
            _ignoredAssembliesProperty = null;
            _ignoredBaseClassesProperty = null;
            
            base.OnDeactivate();
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.PropertyField(_searchInAllAssembliesProperty, new GUIContent("Search in all assemblies"));
            
            if (_searchInAllAssembliesProperty.boolValue)
            {
                EditorGUILayout.PropertyField(_ignoredAssembliesProperty, new GUIContent("Ignored Assemblies (wildcard * supported)"));
            }

            EditorGUILayout.PropertyField(_ignoredBaseClassesProperty, new GUIContent("Ignored Base Classes (wildcard * supported)"));
            
            _settings.ApplyModifiedPropertiesWithoutUndo();
        }
        
        [SettingsProvider]
        public static SettingsProvider CreateComponentReplacerSettingsProvider()
        {
            return new ComponentReplacerSettingsProvider();
        }
    }
}