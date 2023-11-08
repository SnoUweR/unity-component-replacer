using UnityEditor;
using UnityEngine;

namespace SnUnityCommonUtils.Utils
{
    internal class ObjectsCopier
    {
        public void Copy(Object source, Object dest)
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
    }
}