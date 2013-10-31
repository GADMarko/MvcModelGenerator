using Rhetos.Compiler;
using Rhetos.Dsl.DefaultConcepts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Rhetos.MvcModelGenerator.DefaultConcepts
{

    public static class MvcPropertyHelper
    {
        public static readonly CsTag<PropertyInfo> AttributeTag = "Attribute";

        private static string ImplementationCodeSnippet(PropertyInfo info, string type, string nameSuffix, string additionalTag)
        {
            string captionName = info.Name;
            if (captionName.EndsWith("Browse")) captionName = captionName.Replace("Browse", "");

            return string.Format(@"
        " + AttributeTag.Evaluate(info) + @"
        [Display(Name = ""{3}"", ResourceType = typeof(Captions))]{4}
        public new virtual {1} {0}{2} {{ get; set; }}
        public const string Property{0}{2} = ""{0}{2}"";
        ", info.Name, type, nameSuffix, captionName, additionalTag);
        }

        public static void GenerateCodeForType(PropertyInfo info, ICodeBuilder codeBuilder, string type, string nameSuffix = "", string additionalTag = "")
        {
            codeBuilder.InsertCode(ImplementationCodeSnippet(info, type, nameSuffix, additionalTag), DataStructureCodeGenerator.ClonePropertiesTag, info.DataStructure);
        }
    }
}
