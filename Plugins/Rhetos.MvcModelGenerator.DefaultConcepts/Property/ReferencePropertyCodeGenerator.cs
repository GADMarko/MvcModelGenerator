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
using System.Collections.Generic;
using System.Linq;
using OmegaCommonConcepts;

namespace Rhetos.MvcModelGenerator.DefaultConcepts
{
    [Export(typeof(IMvcModelGeneratorPlugin))]
    [ExportMetadata(MefProvider.Implements, typeof(ReferencePropertyInfo))]
    public class ReferencePropertyCodeGenerator : IMvcModelGeneratorPlugin
    {
        private readonly IDslModel _dslModel;

        public ReferencePropertyCodeGenerator(IDslModel dslModel)
        {
            _dslModel = dslModel;
        }

        private const string ReferenceFormat = @"
        [Rhetos.Mvc.Lookup(LookupTextField = ""{0}"", LookupEntity = ""{1}"", LookupColumns = new string[] {{{2}}}, LookupType = Rhetos.Mvc.LookupType.DropDown)]";

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            ReferencePropertyInfo info = (ReferencePropertyInfo)conceptInfo;
            if (DataStructureCodeGenerator.IsTypeSupported(info.DataStructure))
            {


                var properties = GetReferenceProperties(_dslModel.Concepts, info);

                string lookupField = "";
                var lookupColumns = new List<string>();

                foreach (var prop in properties)
                {

                    lookupField = prop.Name;
                    lookupColumns.Add("\"" + prop.Name + "\"");   
                }


                string lookupEntity = info.Referenced.Name;

                //string dodatniAtribut = string.Format(ReferenceFormat, _dslModel.Concepts.Count(), lookupEntity, String.Join(", ", lookupColumns));
                string dodatniAtribut = string.Format(ReferenceFormat, lookupField, lookupEntity, String.Join(", " , lookupColumns));
                
               

                MvcPropertyHelper.GenerateCodeForType(_dslModel, info, codeBuilder, "Guid?", "ID", dodatniAtribut);
            }
        }

        private static IEnumerable<PropertyInfo> GetReferenceProperties(IEnumerable<IConceptInfo> existingConcepts, ReferencePropertyInfo property)
        {

            return existingConcepts.OfType<LookupVisibleInfo>()
                   .Where(r => r.Property.DataStructure == property.Referenced).ToList()
                   .Select(r => r.Property);

        }

    }
}