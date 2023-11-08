using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Compilation;
using UnityEngine;
using Assembly = System.Reflection.Assembly;

namespace SnUnityCommonUtils.Utils
{
	internal class AssembliesSearcher
	{
		public ICollection<Type> GetAllDerivedTypes(Type targetObjectType, bool searchInAllAssemblies, IReadOnlyCollection<string> ignoredAssemblies = null,
			IReadOnlyCollection<string> ignoredBaseClasses = null, bool orderByName = true)
		{
			// Firstly, we should find the most base type for the current component. It will have MonoBehaviour as parent type.
			var baseType = targetObjectType;
			var monoBehaviourType = typeof(MonoBehaviour);
			while (true)
			{
				var baseTypeTmp = baseType.BaseType;
				if (baseTypeTmp == null || baseTypeTmp == monoBehaviourType)
					break;

				if (WildcardStringsCompareUtils.IsStringMatchAnyOfWildcardTemplates(ignoredBaseClasses, baseTypeTmp.Name))
					break;

				baseType = baseTypeTmp;
			}
        
			// Secondly, we should find all types that inherit from our base type in some way.
			var list = new List<Type>();
			foreach (var assembly in GetAssembliesToSearch(baseType, searchInAllAssemblies, ignoredAssemblies))
			{
				list.AddRange(GetAllInheritedClasses(assembly, baseType));
			}

			return orderByName ? list.OrderBy(item => item.Name).ToList() : list;
		}
		
		private IEnumerable<Assembly> GetAssembliesToSearch(Type baseType, bool searchInAllAssemblies, IReadOnlyCollection<string> ignoredAssemblies)
		{
			if (searchInAllAssemblies)
			{
				var unityKnownAssemblies = CompilationPipeline.GetAssemblies();
				var assemblies = AppDomain.CurrentDomain.GetAssemblies();
				
				foreach (var assembly in assemblies)
				{
					var assemblyName = assembly.GetName().Name;
					
					if (unityKnownAssemblies.All(unityAssembly => unityAssembly.name != assemblyName))
						continue;
					
					if (WildcardStringsCompareUtils.IsStringMatchAnyOfWildcardTemplates(ignoredAssemblies, assemblyName))
						continue;
					
					yield return assembly;
				}
			}
			else
			{
				yield return baseType.Assembly;
			}
		}
		
		private IEnumerable<Type> GetAllInheritedClasses(Assembly assembly, Type baseType)
		{
			var allTypes = assembly.GetTypes();
			foreach (var type in allTypes)
			{
				// Ignore the type if it can't be added as a component.
				if (!type.IsClass || type.IsAbstract)
					continue;

				// If the base type can be assigned from the current type, then we have found an inheritor.
				if (baseType.IsAssignableFrom(type))
					yield return type;
			}
		}
	}
}