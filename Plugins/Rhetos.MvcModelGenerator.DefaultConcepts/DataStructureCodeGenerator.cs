/*
    Copyright (C) 2013 Omega software d.o.o.

    This file is part of Rhetos.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Xml;
using Rhetos.Compiler;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensibility;
using Rhetos.MvcModelGenerator;

namespace Rhetos.MvcModelGenerator.DefaultConcepts
{
    [Export(typeof(IMvcModelGeneratorPlugin))]
    [ExportMetadata(MefProvider.Implements, typeof(DataStructureInfo))]
    public class DataStructureCodeGenerator : IMvcModelGeneratorPlugin
    {
        public static readonly CsTag<DataStructureInfo> ClonePropertiesTag = "CloneProperties";

        private static string ImplementationCodeSnippet(DataStructureInfo info)
        {
            string dataStructureBezBrowsea = info.Name;
            if (dataStructureBezBrowsea.ToLower().EndsWith("browse")) dataStructureBezBrowsea = dataStructureBezBrowsea.Substring(0, dataStructureBezBrowsea.Length - 6);

            return string.Format(@"
namespace Omega.MvcModel.{0} 
{{ 
    [Rhetos.Mvc.LocalizedDisplayName(""{3}"", {4})]
    public partial class {1} : Rhetos.Mvc.BaseMvcModel
    {{
        public const string Entity{1} = ""{1}"";

        {2}
    }}
}}

    ",
                info.Module.Name, 
                info.Name, 
                ClonePropertiesTag.Evaluate(info),
                CaptionHelper.GetCaptionConstant(info),
                "typeof(Captions)"
                );
           
          
        }

        private static bool _isInitialCallMade;

        public static bool IsTypeSupported(DataStructureInfo conceptInfo)
        {
            return conceptInfo is EntityInfo
                || conceptInfo is BrowseDataStructureInfo
                || conceptInfo is LegacyEntityInfo
                || conceptInfo is LegacyEntityWithAutoCreatedViewInfo
                || conceptInfo is SqlQueryableInfo
                || conceptInfo is QueryableExtensionInfo
                || conceptInfo is ComputedInfo;
        }

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            DataStructureInfo info = (DataStructureInfo)conceptInfo;

            if (IsTypeSupported(info))
            {
                GenerateInitialCode(codeBuilder);

                codeBuilder.InsertCode(ImplementationCodeSnippet(info), MvcModelGeneratorTags.ModuleMembers);
            }
        }

        private static void GenerateInitialCode(ICodeBuilder codeBuilder)
        {
            if (_isInitialCallMade)
                return;
            _isInitialCallMade = true;
        }
    }
}