using System;
using System.Collections.Generic;
using System.Linq;
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
        private Vector2 _scrollPosition;
        private Component _sourceComponent;
        private Type _sourceComponentType;
        private GameObject _currentObject;
        private ICollection<Type> _derivedTypes;

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
            SetSourceComponent((Component)context);
            _currentObject = _sourceComponent.gameObject;
            _derivedTypes = GetAllDerivedTypes();
        }

        private void SetSourceComponent(Component component)
        {
            _sourceComponent = component;
            _sourceComponentType = _sourceComponent.GetType();
        }

        private ICollection<Type> GetAllDerivedTypes(bool orderByName = true)
        {
            if (_sourceComponent == null)
                return Array.Empty<Type>();
        
            // Firstly, we should find the most base type for the current component. It will have MonoBehaviour as parent type.
            var objType = _sourceComponent.GetType();
            var baseType = objType;
            var monoBehaviourType = typeof(MonoBehaviour);
            while (true)
            {
                var baseTypeTmp = baseType.BaseType;
                if (baseTypeTmp == null || baseTypeTmp == monoBehaviourType)
                    break;

                baseType = baseTypeTmp;
            }
        
            // Secondly, we should find all types that inherit from our base type in some way. 
            var list = new List<Type>();
            var assembly = objType.Assembly;
            var allTypes = assembly.GetTypes();
            foreach (var type in allTypes)
            {
                // Ignore the type if it can't be added as a component.
                if (!type.IsClass || type.IsAbstract)
                    continue;

                // If the base type can be assigned from the current type, then we have found an inheritor.
                if (baseType.IsAssignableFrom(type))
                    list.Add(type);
            }

            return orderByName ? list.OrderBy(item => item.Name).ToList() : list;
        }

        private void Copy(Object source, Object dest)
        {
            if (source == null)
            {
                Debug.LogError($"{nameof(source)} is null");
                return;
            }
        
            if (dest == null)
            {
                Debug.LogError($"{nameof(dest)} is null");
                return;
            }
        
            // If the types are the same, then we can use the built-in Unity method. 
            if (source == dest)
            {
                EditorUtility.CopySerialized(source, dest);
                return;
            }
        
            var sourceSo = new SerializedObject(source);
            var destSo = new SerializedObject(dest);

            var sourceIterator = sourceSo.GetIterator();
            // We should skip the first iteration, since it contains a type of the script that we do not need to overwrite. 
            if (sourceIterator.NextVisible(true))
            {
                while (sourceIterator.NextVisible(true))
                {
                    // Trying to find the same property in the destination component. 
                    var destProperty = destSo.FindProperty(sourceIterator.propertyPath);
                    if (destProperty == null)
                        continue;

                    // If the destination property has different type, then we should ignore it. 
                    if (destProperty.propertyType != sourceIterator.propertyType)
                        continue;

                    // Finally, copying the property value from the original component.
                    destSo.CopyFromSerializedProperty(sourceIterator);
                }
            
                destSo.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private void ReplaceComponent(Type toType)
        {
            var newComponent = Undo.AddComponent(_currentObject, toType);
            Copy(_sourceComponent, newComponent);
            Undo.DestroyObjectImmediate(_sourceComponent);
            SetSourceComponent(newComponent);
        }
    }
}