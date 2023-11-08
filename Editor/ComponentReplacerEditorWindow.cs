using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SnUnityCommonUtils.Utils
{
    /// <summary>
    /// This utility allows you to replace the component with another component of the same class hierarchy.
    /// All properties that exist in both components are copied to the new component.
    /// Therefore we can easily replace a base component with any derivative, or a derived component to another
    /// derived component (or even to a base one) without having to connect all properties links again. 
    /// </summary>
    public class ComponentReplacerEditorWindow : EditorWindow
    {
        private ComponentReplacerSettings _settings;
        private Vector2 _scrollPosition;
        private Component _sourceComponent;
        private Type _sourceComponentType;
        private GameObject _currentObject;
        private ICollection<Type> _derivedTypes;
        private ObjectsCopier _objectsCopier;

        #region Unity Events
    
        [MenuItem("CONTEXT/Component/Replace to Another Component..")]
        private static void ShowWindow(MenuCommand command)
        {
            var window = GetWindow<ComponentReplacerEditorWindow>();
            window.titleContent = new GUIContent("Component Replacer");
            window.Init(command.context);
            window.Show();
        }
    
        private void OnGUI()
        {
            // Close the window if something goes wrong.
            if (_currentObject == null || _sourceComponent == null || _derivedTypes == null)
            {
                Debug.LogWarning($"{titleContent} was closed because context changed");
                Close();
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            EditorGUILayout.BeginVertical();
            foreach (var type in _derivedTypes)
            {
                if (type == null)
                    continue;

                // If it's the same type, then just disable the button, since there is no point to replace the component. 
                var sameType = type == _sourceComponentType;
                EditorGUI.BeginDisabledGroup(sameType);
                if (GUILayout.Button(type.Name))
                    ReplaceComponent(type);
                EditorGUI.EndDisabledGroup();
            }
        
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    
        #endregion

        private void Init(Object context)
        {
            _objectsCopier = new ObjectsCopier();
            _settings = ComponentReplacerSettings.GetOrCreateSettings();

            SetSourceComponent((Component)context);
            _currentObject = _sourceComponent.gameObject;

            _derivedTypes = _sourceComponent == null
                ? Array.Empty<Type>() 
                : new AssembliesSearcher().GetAllDerivedTypes(_sourceComponentType, _settings._searchInAllAssemblies, 
                    _settings._ignoredAssemblies, _settings._ignoredBaseClasses);
        }

        private void SetSourceComponent(Component component)
        {
            _sourceComponent = component;
            _sourceComponentType = _sourceComponent.GetType();
        }

        private void ReplaceComponent(Type toType)
        {
            var newComponent = Undo.AddComponent(_currentObject, toType);
            _objectsCopier.Copy(_sourceComponent, newComponent);
            Undo.DestroyObjectImmediate(_sourceComponent);
            SetSourceComponent(newComponent);
        }
    }
}