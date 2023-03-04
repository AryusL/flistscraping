using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace flistscraping
{
    public class CharacterInfo
    {
        public string Name { get; set; }

        public enum KinkPosition
        {
            NO, FAVE, YES, MAYBE
        }

        private Dictionary<string, string> sideBarInfos;
        public static HashSet<string> SideBarColumns = new HashSet<string>() { "Gender", "Orientation", "Language Preference", "Species", "Furry Preference", "Dom/Sub role", "Timezone" };

        private Dictionary<string, string> rpInfos;
        public static HashSet<string> RpColumns = new HashSet<string>() { "Currently looking for", "Desired post length" };

        private Dictionary<string, KinkPosition> kinksPositions;
        public static HashSet<string> KinkFilters = new HashSet<string>() { "Humans",
            "Vore (Being Predator)", "Vore (Being Prey)", "Soft Vore", "Cock Vore", "Belching / Burping", "Food Play", "Digestion",
            "Size Differences (Micro / Macro)", "Microphilia", "Macrophilia", "Shrinking (Micro)",
            "Consensual", "Dub-Consensual", "Nonconsensual"};
        private Dictionary<string, KinkPosition> customKinksPositions;
        public static HashSet<string> CustomKinkFilters = new HashSet<string>() { "macro", "micro", "vore" };


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

        public string CleanString(string str)
        {
            if (String.IsNullOrEmpty(str))
                return str;
            return Regex.Replace(str.Trim(), @"[\p{C}|\n|\r|\t]+", "");
        }

        public void AddSideBarInfo(string variableName, string variableValue)
        {
            if (SideBarColumns.Contains(variableName))
            {
                var value = CleanString(variableValue);
                sideBarInfos.Add(variableName, value);

            }
        }

        public void AddRpInfo(string variableName, string variableValue)
        {
            if (RpColumns.Contains(variableName))
            {
                var value = CleanString(variableValue);
                rpInfos.Add(variableName, value);

            }
        }

        public void AddKink(string variableName, KinkPosition kp)
        {
            var kinkName = CleanString(variableName);
            if (KinkFilters.Contains(kinkName))
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
                foreach (var ck in CustomKinkFilters)
                {
                    var key = "Custom-" + ck;
                    if (kinkName.Contains(ck) && !customKinksPositions.ContainsKey(key))
                        customKinksPositions.Add("Custom-" + ck, kp);
                }
            }
        }

        public void DisplayInfos()
        {
            Console.WriteLine($"#Name: {Name}");
            Console.WriteLine($"##SideBar");

            foreach (var key in SideBarColumns)
            {
                if (sideBarInfos.ContainsKey(key))
                    Console.WriteLine($"{key}: {sideBarInfos[key]}");
                else
                    Console.WriteLine($"{key}: N/A");

            }

            Console.WriteLine($"##RpInfo");

            foreach (var key in RpColumns)
            {
                if (rpInfos.ContainsKey(key))
                    Console.WriteLine($"{key}: {rpInfos[key]}");
                else
                    Console.WriteLine($"{key}: N/A");

            }

            Console.WriteLine($"##KINKS");

            foreach (var key in KinkFilters)
            {
                if (kinksPositions.ContainsKey(key))
                    Console.WriteLine($"{key}: {kinksPositions[key].ToString()}");
                else
                    Console.WriteLine($"{key}: NO");

            }

            Console.WriteLine($"##CUSTOM KINKS");

            foreach (var key in CustomKinkFilters)
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
