using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;

namespace flistscraping
{
    public class CharacterList
    {
        private List<CharacterInfo> characters;
        public List<CharacterInfo> Characters => characters;
        public static HashSet<string> SideBarColumns = new HashSet<string>() { "Gender", "Orientation", "Language Preference", "Species", "Furry Preference", "Dom/Sub role", "Timezone" };
        public static HashSet<string> RpColumns = new HashSet<string>() { "Currently looking for", "Desired post length" };
        public static HashSet<string> KinkFilters = new HashSet<string>() { "Humans",
            "Vore (Being Predator)", "Vore (Being Prey)", "Soft Vore", "Cock Vore", "Belching / Burping", "Food Play", "Digestion",
            "Size Differences (1-3 Feet)", "Size Differences (Micro / Macro)", "Microphilia", "Macrophilia", "Shrinking (Micro)",
            "Consensual", "Dub-Consensual", "Nonconsensual"};
        public static HashSet<string> CustomKinkFilters = new HashSet<string>() { "macro", "micro", "size", "vore" };
        public CharacterList(List<CharacterInfo> characters)
        {
            this.characters = characters;
        }

        public List<string> GetExcelColumns()
        {
            List<string> columns = new List<string>();
            columns.Add("Name");
            columns.Add("Url");
            foreach (var cl in SideBarColumns)
                columns.Add(cl);
            foreach (var cl in RpColumns)
                columns.Add(cl);
            foreach (var cl in KinkFilters)
                columns.Add(cl);
            foreach (var cl in CustomKinkFilters)
                columns.Add("Custom-" + cl);
            return columns;
        }

        public void ExportToExcel(string fileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // create a new Excel package
            using (var package = new ExcelPackage())
            {
                // add a new worksheet to the package
                var worksheet = package.Workbook.Worksheets.Add("Profiles");

                // write the header row to the worksheet
                var columns = GetExcelColumns();
                for (int i = 0; i < columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = columns[i];
                }

                // write the values to the worksheet
                int row = 2;
                foreach (var character in characters)
                {
                    var characterValues = character.GetValues();
                    for (int i = 0; i < characterValues.Count; i++)
                    {
                        worksheet.Cells[row, i + 1].Value = characterValues[i];
                        if (i == 1) // hyperlink column
                        {
                            worksheet.Cells[row, i + 1].Hyperlink = new ExcelHyperLink(characterValues[i]);

                            worksheet.Cells[row, i + 1].Style.Font.UnderLine = true;
                            worksheet.Cells[row, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        }
                    }
                    row++;
                }

                // save the package to a file
                using (var stream = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    package.SaveAs(stream);
                }
            }
        }
    }

    public class CharacterInfo
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public enum KinkPosition
        {
            NO, FAVE, YES, MAYBE
        }

        private Dictionary<string, string> sideBarInfos;

        private Dictionary<string, string> rpInfos;

        private Dictionary<string, KinkPosition> kinksPositions;

        private Dictionary<string, KinkPosition> customKinksPositions;

        public CharacterInfo()
        {
            sideBarInfos = new Dictionary<string, string>();
            rpInfos = new Dictionary<string, string>();
            kinksPositions = new Dictionary<string, KinkPosition>();
            customKinksPositions = new Dictionary<string, KinkPosition>();
        }

        public void SetName(string name)
        {
            Name = CleanString(name);
        }

        public void SetUrl(string url)
        {
            Url = CleanString(url);
        }

        public string CleanString(string str)
        {
            if (String.IsNullOrEmpty(str))
                return str;
            return Regex.Replace(str.Trim(), @"[\p{C}|\n|\r|\t]+", "");
        }

        public void AddSideBarInfo(string variableName, string variableValue)
        {
            if (CharacterList.SideBarColumns.Contains(variableName))
            {
                var value = CleanString(variableValue);
                sideBarInfos.Add(variableName, value);

            }
        }

        public void AddRpInfo(string variableName, string variableValue)
        {
            if (CharacterList.RpColumns.Contains(variableName))
            {
                var value = CleanString(variableValue);
                rpInfos.Add(variableName, value);

            }
        }

        public void AddKink(string variableName, KinkPosition kp)
        {
            var kinkName = CleanString(variableName);
            if (CharacterList.KinkFilters.Contains(kinkName))
            {
                kinksPositions.Add(kinkName, kp);

            }
        }

        public void AddCustomKink(string variableName, KinkPosition kp)
        {
            var kinkName = CleanString(variableName);
            if (!String.IsNullOrEmpty(kinkName))
            {
                kinkName = kinkName.ToLower();
                foreach (var ck in CharacterList.CustomKinkFilters)
                {
                    var key = "Custom-" + ck;
                    if (kinkName.Contains(ck) && !customKinksPositions.ContainsKey(key))
                        customKinksPositions.Add("Custom-" + ck, kp);
                }
            }
        }

        public List<string> GetValues()
        {
            var values = new List<string>();
            values.Add(Name);
            values.Add(Url);
            foreach (var key in CharacterList.SideBarColumns)
            {
                if (sideBarInfos.ContainsKey(key))
                    values.Add(sideBarInfos[key]);
                else
                    values.Add("N/A");
            }
            foreach (var key in CharacterList.RpColumns)
            {
                if (rpInfos.ContainsKey(key))
                    values.Add(rpInfos[key]);
                else
                    values.Add("N/A");
            }
            foreach (var key in CharacterList.KinkFilters)
            {
                if (kinksPositions.ContainsKey(key))
                    values.Add(kinksPositions[key].ToString());
                else
                    values.Add("NO");

            }
            foreach (var key in CharacterList.CustomKinkFilters)
            {
                var customKey = "Custom-" + key;
                if (kinksPositions.ContainsKey(customKey))
                    values.Add(kinksPositions[customKey].ToString());
                else
                    values.Add("NO");
            }

            return values;
        }

        public void DisplayInfos()
        {
            Console.WriteLine($"#Name: {Name}");
            Console.WriteLine($"##SideBar");

            foreach (var key in CharacterList.SideBarColumns)
            {
                if (sideBarInfos.ContainsKey(key))
                    Console.WriteLine($"{key}: {sideBarInfos[key]}");
                else
                    Console.WriteLine($"{key}: N/A");

            }

            Console.WriteLine($"##RpInfo");

            foreach (var key in CharacterList.RpColumns)
            {
                if (rpInfos.ContainsKey(key))
                    Console.WriteLine($"{key}: {rpInfos[key]}");
                else
                    Console.WriteLine($"{key}: N/A");

            }

            Console.WriteLine($"##KINKS");

            foreach (var key in CharacterList.KinkFilters)
            {
                if (kinksPositions.ContainsKey(key))
                    Console.WriteLine($"{key}: {kinksPositions[key].ToString()}");
                else
                    Console.WriteLine($"{key}: NO");

            }

            Console.WriteLine($"##CUSTOM KINKS");

            foreach (var key in CharacterList.CustomKinkFilters)
            {
                var customKey = "Custom-" + key;
                if (kinksPositions.ContainsKey(customKey))
                    Console.WriteLine($"{customKey}: {kinksPositions[customKey].ToString()}");
                else
                    Console.WriteLine($"{customKey}: NO");

            }

        }
    }
}
