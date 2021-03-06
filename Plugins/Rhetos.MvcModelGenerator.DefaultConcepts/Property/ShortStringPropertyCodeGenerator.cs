﻿/*
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
using System.Linq;
using System.Xml;
using Rhetos.Compiler;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensibility;
using Rhetos.MvcModelGenerator;

namespace Rhetos.MvcModelGenerator.DefaultConcepts
{
    [Export(typeof(IMvcModelGeneratorPlugin))]
    [ExportMetadata(MefProvider.Implements, typeof(PropertyInfo))]
    public class ShortStringPropertyCodeGenerator : IMvcModelGeneratorPlugin
    {
        private readonly IDslModel _dslModel;

        public ShortStringPropertyCodeGenerator(IDslModel dslModel)
        {
            _dslModel = dslModel;
        }

        private const string ShortStringFormat = @"
        [MaxLength(256)]";


        private static string GetPropertyType(PropertyInfo conceptInfo)
        {
            if (conceptInfo is ShortStringPropertyInfo) return "string";
            return null;
        }

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            PropertyInfo info = (PropertyInfo)conceptInfo;
            string propertyType = GetPropertyType(info);

            var hasMaxLength = _dslModel.Concepts.OfType<MaxLengthInfo>().Any(
                maxLength => maxLength.Property.Name == info.Name 
                    && maxLength.Property.DataStructure.Name == info.DataStructure.Name
                    && maxLength.Property.DataStructure.Module.Name == info.DataStructure.Module.Name
                    );

            string additionalTag = hasMaxLength ? "" : ShortStringFormat;

            if (!String.IsNullOrEmpty(propertyType) && DataStructureCodeGenerator.IsTypeSupported(info.DataStructure))
            {
                MvcPropertyHelper.GenerateCodeForType(_dslModel, info, codeBuilder, propertyType, "", additionalTag);
            }
        }
    }
}