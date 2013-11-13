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

        private const string EditFormHidden = @"
        [AdditionalKendoMetadata(EditFormHidden = true)]";

        private const string DefaultAutocode = @"
        [DefaultValue(""+"")]";

        private const string DefaultActive = @"
        [DefaultValue(true)]";

        private static string ImplementationCodeSnippet(IDslModel dslModel, PropertyInfo info, string type, string nameSuffix, string additionalTag)
        {
            string captionName = CaptionHelper.GetCaptionConstant(info);

            additionalTag = KreirajDodatneMvcAtribute(dslModel, info, additionalTag);

            return string.Format(@"
        " + AttributeTag.Evaluate(info) + @"
        [Display(Name = ""{3}"", ResourceType = typeof(Captions))]{4}
        public virtual {1} {0}{2} {{ get; set; }}
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

        public static string KreirajDodatneMvcAtribute(IDslModel dslModel, PropertyInfo info, string dodatniAtributi)
        {
            HashSet<string> dodatniMvcAtributi = new HashSet<string>();
            dodatniMvcAtributi.Add(SakrijPoljaDohvacenaPrekoLookupa(dslModel, info));
            dodatniMvcAtributi.Add(SakrijDetailPolja(dslModel, info));
            dodatniMvcAtributi.Add(KreirajDefaulte(dslModel, info));

            dodatniMvcAtributi.Remove("");


            return dodatniAtributi + string.Join("", dodatniMvcAtributi);
        }

        private static string KreirajDefaulte(IDslModel dslModel, PropertyInfo info)
        {
            bool jeAutocode = dslModel.Concepts.OfType<AutoCodePropertyInfo>().Any(
                p => p.Property.Name == info.Name && p.Property.DataStructure.Module.Name == info.DataStructure.Module.Name);

            if (jeAutocode) return DefaultAutocode;

            if (info.Name.ToLower() == "active" && info is BoolPropertyInfo) return DefaultActive;
            return "";
        }

        private static string SakrijDetailPolja(IDslModel dslModel, PropertyInfo info)
        {
            string entityName = CaptionHelper.RemoveBrowseSufix(info.DataStructure.Name);

            bool jeDetail = dslModel.Concepts.OfType<ReferenceDetailInfo>().Any(
                d => d.Reference.Name == info.Name
                     && d.Reference.DataStructure.Name == entityName
                     && d.Reference.DataStructure.Module.Name == info.DataStructure.Module.Name);

            if (jeDetail) return EditFormHidden;
            return "";
        }

        private static string SakrijPoljaDohvacenaPrekoLookupa(IDslModel dslModel, PropertyInfo info)
        {
            bool jeBrowse = info.DataStructure.Name.EndsWith("Browse");
            if (jeBrowse)
            {
                string dataStructureName = CaptionHelper.RemoveBrowseSufix(info.DataStructure.Name);

                bool postojiUBaznomEntitetu = dslModel.Concepts.OfType<PropertyInfo>().Any(
                    p => p.DataStructure.Name == dataStructureName && p.Name == info.Name && p.DataStructure.Module.Name == info.DataStructure.Module.Name);

                if (!postojiUBaznomEntitetu) return EditFormHidden;
            }
            return "";
        }
    }
}
