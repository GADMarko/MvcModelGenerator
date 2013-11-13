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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rhetos.Compiler;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensibility;
using Rhetos.MvcModelGenerator.Captions;

namespace Rhetos.MvcModelGenerator.DefaultConcepts.Captions
{
    [Export(typeof(ICaptionsGeneratorPlugin))]
    [ExportMetadata(MefProvider.Implements, typeof(PropertyInfo))]
    public class PropertyCaptionGenerator : ICaptionsGeneratorPlugin
    {
        private static readonly HashSet<string> GeneriraneKonstante = new HashSet<string>();

        private static string GetPropertyType(PropertyInfo conceptInfo)
        {
            return MvcPropertyHelper.GetPropertyType(conceptInfo);
        }

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            PropertyInfo info = (PropertyInfo)conceptInfo;
            string imePropertyja = CaptionHelper.GetCaptionConstant(info);
            string caption = CaptionHelper.GetCaption(info.Name);

            string generiraniKod =
            @"  
  <data name=""" + imePropertyja + @""" xml:space=""preserve"">
    <value>" + caption + @"</value>
  </data>";

            if (!GeneriraneKonstante.Contains(imePropertyja))
            {
                GeneriraneKonstante.Add(imePropertyja);

                string propertyType = GetPropertyType(info);
                if (!String.IsNullOrEmpty(propertyType) && DataStructureCodeGenerator.IsTypeSupported(info.DataStructure))
                {
                    codeBuilder.InsertCode(generiraniKod, MvcModelGeneratorTags.ModuleMembers);
                }
            }
        }
    }
}