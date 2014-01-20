using System.Text;
using Rhetos.Dsl.DefaultConcepts;

namespace Rhetos.MvcModelGenerator.DefaultConcepts
{
    public class CaptionHelper
    {
        public static string RemoveBrowseSufix(string entityName)
        {
            if (entityName.EndsWith("Browse")) entityName = entityName.Replace("Browse", "");
            return entityName;
        }

        public static string GetCaption(string name)
        {
            StringBuilder sb = new StringBuilder();

            var field = name.ToCharArray();
            sb.Append(field[0]);
            for (int i = 1; i < field.Length; i++)
            {
                if (char.IsUpper(field[i]) && char.IsLower(field[i - 1])) sb.Append(" "); 
                sb.Append(char.ToLower(field[i]));
            }

            return GetCaptionAlias(sb.ToString());
        }

        private static string GetCaptionAlias(string caption)
        {
            caption = caption.Replace("Sifra", "Šifra").Replace("sifra", "šifra");
            caption = caption.Replace("dj", "đ").Replace("Dj", "Đ");
            caption = caption.Replace("Active", "Aktivan");
            caption = caption.Replace("since", "od");
            caption = caption.Replace("Kolicina", "Količina").Replace("kolicina", "količina");
            caption = caption.Replace("Dobavljac", "Dobavljač").Replace("dobavljac", "dobavljač");
            caption = caption.Replace("Drzav", "Držav").Replace("drzav", "držav");
            caption = caption.Replace("Racun", "Račun").Replace("racun", "račun");
            caption = caption.Replace("Narudzb", "Narudžb").Replace("narudzb", "narudžb");
            caption = caption.Replace("Katalosk", "Katalošk").Replace("katalosk", "katalošk");
            caption = caption.Replace("Oib", "OIB");
            caption = caption.Replace("Ean", "EAN");
            caption = caption.Replace("Jmbg", "JMBG");
            caption = caption.Replace("Skladis", "Skladiš").Replace("skladis", "skladiš");

            return caption;
        }

        public static string GetCaptionConstant(PropertyInfo info)
        {
            string entityName = CaptionHelper.RemoveBrowseSufix(info.DataStructure.Name);
            return info.DataStructure.Module.Name + "_" + entityName + "_" + info.Name;
        }

        public static string GetCaptionConstant(DataStructureInfo info)
        {
            string entityName = CaptionHelper.RemoveBrowseSufix(info.Name);
            return info.Module.Name + "_" + entityName;
        }
    }
}
