using System.Text;

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

            var polje = name.ToCharArray();
            sb.Append(polje[0]);
            for (int i = 1; i < polje.Length; i++)
            {
                if (char.IsUpper(polje[i]) && char.IsLower(polje[i - 1])) sb.Append(" "); //provjera slova na i-1 zbog riječi tipa "OIB".
                sb.Append(char.ToLower(polje[i]));
            }

            return GetCaptionAlias(sb.ToString());
        }

        private static string GetCaptionAlias(string caption)
        {
            caption = caption.Replace("Sifra", "Šifra").Replace("sifra", "šifra");
            caption = caption.Replace("dj", "đ").Replace("Dj", "Đ");
            caption = caption.Replace("Active", "Aktivan");
            caption = caption.Replace("Kolicina", "Količina").Replace("kolicina", "količina");
            caption = caption.Replace("Dobavljac", "Dobavljač").Replace("dobavljac", "dobavljač");
            caption = caption.Replace("Drzav", "Držav").Replace("drzav", "držav");
            caption = caption.Replace("Racun", "Račun").Replace("racun", "račun");
            caption = caption.Replace("Narudzb", "Narudžb").Replace("narudzb", "narudžb");

            return caption;
        }
    }
}
