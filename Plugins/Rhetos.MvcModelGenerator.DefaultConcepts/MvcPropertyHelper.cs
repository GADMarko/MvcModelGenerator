using Rhetos.Compiler;
using Rhetos.Dsl;
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
        
        private static string EditFormHidden = @"
        [AdditionalKendoMetadata(EditFormHidden = true)]";

        private static string ImplementationCodeSnippet(IDslModel dslModel, PropertyInfo info, string type, string nameSuffix, string additionalTag)
        {
            string entityName = CaptionHelper.RemoveBrowseSufix(info.DataStructure.Name);
            string captionName = entityName + "_" + info.Name;

            additionalTag = SakrijPolja(dslModel, info, additionalTag);

            return string.Format(@"
        " + AttributeTag.Evaluate(info) + @"
        [Display(Name = ""{3}"", ResourceType = typeof(Captions))]{4}
        public new virtual {1} {0}{2} {{ get; set; }}
        public const string Property{0}{2} = ""{0}{2}"";
        ", info.Name, type, nameSuffix, captionName, additionalTag);
        }

        public static void GenerateCodeForType(IDslModel dslModel, PropertyInfo info, ICodeBuilder codeBuilder, string type, string nameSuffix = "", string additionalTag = "")
        {
            codeBuilder.InsertCode(ImplementationCodeSnippet(dslModel, info, type, nameSuffix, additionalTag), DataStructureCodeGenerator.ClonePropertiesTag, info.DataStructure);
        }

        public static string GetPropertyType(PropertyInfo conceptInfo)
        {
            if (conceptInfo is IntegerPropertyInfo) return "int?";
            if (conceptInfo is BinaryPropertyInfo) return "byte[]";
            if (conceptInfo is BoolPropertyInfo) return "bool?";
            if (conceptInfo is GuidPropertyInfo || conceptInfo is ReferencePropertyInfo) return "Guid?";
            if (conceptInfo is ShortStringPropertyInfo) return "string";
            if (conceptInfo is LongStringPropertyInfo) return "string";
            if (conceptInfo is MoneyPropertyInfo || conceptInfo is DecimalPropertyInfo) return "decimal?";
            if (conceptInfo is DatePropertyInfo || conceptInfo is DateTimePropertyInfo) return "DateTime?";
            return null;
        }

        public static string SakrijPolja(IDslModel dslModel, PropertyInfo info, string dodatniAtributi)
        {
            dodatniAtributi = SakrijPoljaDohvacenaPrekoLookupa(dslModel, info, dodatniAtributi);
            dodatniAtributi = SakrijDetailPolja(dslModel, info, dodatniAtributi);
            return dodatniAtributi;
        }

        private static string SakrijDetailPolja(IDslModel dslModel, PropertyInfo info, string dodatniAtributi)
        {
            bool jeDetail = dslModel.Concepts.OfType<ReferenceDetailInfo>().Any(
                d => d.Reference.Name == info.Name
                     && d.Reference.DataStructure.Name == info.DataStructure.Name
                     && d.Reference.DataStructure.Module == info.DataStructure.Module);

            if (jeDetail && !dodatniAtributi.Contains(EditFormHidden)) dodatniAtributi += EditFormHidden;
            return dodatniAtributi;
        }

        private static string SakrijPoljaDohvacenaPrekoLookupa(IDslModel dslModel, PropertyInfo info, string dodatniAtributi)
        {
            bool jeBrowse = info.DataStructure.Name.EndsWith("Browse");
            if (jeBrowse)
            {
                string dataStructureName = CaptionHelper.RemoveBrowseSufix(info.DataStructure.Name);

                bool postojiUBaznomEntitetu = dslModel.Concepts.OfType<PropertyInfo>().Any(
                    p => p.DataStructure.Name == dataStructureName && p.Name == info.Name && p.DataStructure.Module.Name == info.DataStructure.Module.Name);

                if (!postojiUBaznomEntitetu) dodatniAtributi += EditFormHidden;
            }
            return dodatniAtributi;
        }
    }
}
