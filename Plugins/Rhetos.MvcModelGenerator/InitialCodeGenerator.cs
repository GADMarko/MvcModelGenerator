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
using System.Configuration;
using Rhetos.Compiler;
using Rhetos.Dsl;

namespace Rhetos.MvcModelGenerator
{
    internal class InitialCodeGenerator : IMvcModelGeneratorPlugin
    {
        private string CodeSnippet =
@"
using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using " + GetNamespace() + @";
" + MvcModelGeneratorTags.Using + @"

/*
    If additional DataAnnotation attributes wants to be used for specific Model here generated, create class as following:

    [MetadataTypeAttribute(typeof(MyModel.AdditionalAttributes))]{
    public partial class MyModel
    {
        internal sealed class AdditionalAttributes
        {
            private AdditionalAttributes() { }

            ...
            List of properties along with additional DataAnnotation attributes.
            Example:
            [Display(Name = ""Last Name"", Order = 1, Prompt = ""Enter Last Name"")]
            public string LastName { get; set; }
            ...

        }
    }

*/
namespace Rhetos.Mvc
{

    public partial class BaseMvcModel
    {
         public Guid ID { get; set; }
    }

    " + MvcModelGeneratorTags.NamespaceMembers + @"

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class MinValueIntegerAttribute : ValidationAttribute
    {
        public string MinValue { get; set; }

        public override bool IsValid(object value)
        {
            return Convert.ToInt32(value) >= Convert.ToInt32(MinValue);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class MinValueDecimalAttribute : ValidationAttribute
    {
        public string MinValue { get; set; }

        public override bool IsValid(object value)
        {
            return  Convert.ToDecimal(value) >= Convert.ToDecimal(MinValue);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class MinValueDateAttribute : ValidationAttribute
    {
        public string MinValue { get; set; }

        public override bool IsValid(object value)
        {
            return Convert.ToDateTime(value) >= Convert.ToDateTime(MinValue);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class MaxValueIntegerAttribute : ValidationAttribute
    {
        public string MaxValue { get; set; }

        public override bool IsValid(object value)
        {
            return Convert.ToInt32(value) <= Convert.ToInt32(MaxValue);
        }
    }
    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class MaxValueDecimalAttribute : ValidationAttribute
    {
        public string MaxValue { get; set; }

        public override bool IsValid(object value)
        {
            return Convert.ToDecimal(value) <= Convert.ToDecimal(MaxValue);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class MaxValueDateAttribute : ValidationAttribute
    {
        public string MaxValue { get; set; }

        public override bool IsValid(object value)
        {
            return Convert.ToDateTime(value) <= Convert.ToDateTime(MaxValue);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class LookupAttribute: Attribute
    {
        public string LookupTextField { get; set;}
        public string LookupEntity { get; set; }
        public string[] LookupColumns { get; set; }
        public LookupType LookupType { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class StyleAttribute: Attribute
    {
        public int Width { get; set; }          
    }
        
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class RenderModeAttribute : Attribute
    {
        private RenderMode _renderMode = RenderMode.Any;
        public RenderMode RenderMode { 
            get{
                return _renderMode;   
            }
            set {
                _renderMode = value;
            }
        }

        public RenderModeAttribute(RenderMode renderMode)
        {
            _renderMode = renderMode;
        }
    }

    public enum RenderMode
    {
        Any,
        EditModeOnly,
        DisplayModeOnly,
        None
    }

    public enum LookupType
    {
        DropDown,
        AutoComplete,
        ComboBox
    }

    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly PropertyInfo nameProperty;

        public LocalizedDisplayNameAttribute(string displayNameKey, Type resourceType = null)
            : base(displayNameKey)
        {
            if (resourceType != null)
            {
                nameProperty = resourceType.GetProperty(base.DisplayName,
                                               BindingFlags.Static | BindingFlags.Public);
            }
        }

        public override string DisplayName
        {
            get
            {
                if (nameProperty == null)
                {
                    return base.DisplayName;
                }
                return (string)nameProperty.GetValue(nameProperty.DeclaringType, null);
            }
        }
    }

}

    " + MvcModelGeneratorTags.ModuleMembers + @"

";

        private static string GetNamespace()
        {
            string webNamespace = ConfigurationManager.AppSettings["WebNamespace"];
            if (string.IsNullOrEmpty(webNamespace)) webNamespace = "Omega.Web.App_GlobalResources";

            return webNamespace;
        }

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            codeBuilder.InsertCode(CodeSnippet);
           
            codeBuilder.AddReferencesFromDependency(typeof(Guid));
            codeBuilder.AddReferencesFromDependency(typeof(System.Linq.Enumerable));
        }

    }
}