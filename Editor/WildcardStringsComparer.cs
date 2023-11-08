using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SnUnityCommonUtils.Utils
{
	internal static class WildcardStringsCompareUtils
	{
		public static bool IsStringMatchWildcardTemplate(string template, string inputString)
		{
			if (template == null || inputString == null)
				return false;
			
			return Regex.IsMatch(inputString, WildCardToRegular(template));
		}

		public static bool IsStringMatchAnyOfWildcardTemplates(IEnumerable<string> templates, string inputString)
		{
			if (templates == null || inputString == null)
				return false;
			
			return templates.Any(template => IsStringMatchWildcardTemplate(template, inputString));
		}

		private static string WildCardToRegular(string template)
		{
			return "^" + Regex.Escape(template).Replace("\\*", ".*") + "$";
		}
	}
}