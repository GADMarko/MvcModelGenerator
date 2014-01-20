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

        private const string EditModeHidden = @"
        [Rhetos.Mvc.RenderMode(Rhetos.Mvc.RenderMode.DisplayModeOnly)]";

        private const string DisplayModeHidden = @"
        [Rhetos.Mvc.RenderMode(Rhetos.Mvc.RenderMode.EditModeOnly)]";

        private const string DefaultAutocode = @"
        [DefaultValue(""+"")]";

        private const string DefaultActive = @"
        [DefaultValue(true)]";

        private static string ImplementationCodeSnippet(IDslModel dslModel, PropertyInfo info, string type, string nameSuffix, string additionalTag)
        {
            string captionName = CaptionHelper.GetCaptionConstant(info);

            additionalTag = CreateAdditionalAttributes(dslModel, info, additionalTag);

            return string.Format(@"
        " + AttributeTag.Evaluate(info) + @"
        [Display(Name = ""{3}"", ResourceType = typeof(Captions), AutoGenerateFilter = true)]{4}
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

        public static string CreateAdditionalAttributes(IDslModel dslModel, PropertyInfo info, string dodatniAtributi)
        {
            HashSet<string> additionalMvcAttributes = new HashSet<string>();
            additionalMvcAttributes.Add(HideLookupFields(dslModel, info));
            additionalMvcAttributes.Add(HideDetailFields(dslModel, info));
            additionalMvcAttributes.Add(CreateDefaultes(dslModel, info));
            additionalMvcAttributes.Add(HideIdFields(dslModel, info));

            additionalMvcAttributes.Remove("");


            return dodatniAtributi + string.Join("", additionalMvcAttributes);
        }

        private static string CreateDefaultes(IDslModel dslModel, PropertyInfo info)
        {
            bool isAutocode = dslModel.Concepts.OfType<AutoCodePropertyInfo>().Any(
                p => p.Property.Name == info.Name && p.Property.DataStructure.Module.Name == info.DataStructure.Module.Name && p.Property.DataStructure.Name == info.DataStructure.Name);

            if (isAutocode) return DefaultAutocode;

            if (info.Name.ToLower() == "active" && info is BoolPropertyInfo) return DefaultActive;
            return "";
        }

        private static string HideDetailFields(IDslModel dslModel, PropertyInfo info)
        {
            string entityName = CaptionHelper.RemoveBrowseSufix(info.DataStructure.Name);

            bool isDetail = dslModel.Concepts.OfType<ReferenceDetailInfo>().Any(
                d => d.Reference.Name == info.Name
                     && d.Reference.DataStructure.Name == entityName
                     && d.Reference.DataStructure.Module.Name == info.DataStructure.Module.Name);

            if (isDetail) return EditModeHidden;
            return "";
        }

        private static string HideLookupFields(IDslModel dslModel, PropertyInfo info)
        {
            bool isBrowse = info.DataStructure.Name.EndsWith("Browse");
            if (isBrowse)
            {
                string dataStructureName = CaptionHelper.RemoveBrowseSufix(info.DataStructure.Name);

                bool existsInBaseEntitety = dslModel.Concepts.OfType<PropertyInfo>().Any(
                    p => p.DataStructure.Name == dataStructureName && p.Name == info.Name && p.DataStructure.Module.Name == info.DataStructure.Module.Name);

                if (!existsInBaseEntitety) return EditModeHidden;
            }
            return "";
        }

        private static string HideIdFields(IDslModel dslModel, PropertyInfo info)
        {
            return info.Name.EndsWith("ID") ? DisplayModeHidden : "";
        }
    }
}
