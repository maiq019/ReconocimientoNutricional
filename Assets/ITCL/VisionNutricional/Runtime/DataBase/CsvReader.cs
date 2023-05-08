using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ITCL.VisionNutricional.Runtime.DataBase
{
    /// <summary>
    /// Simple CSV reader based on WhateverDevs/Localization/Runtime/CsvReader
    /// Altered to parse into list of food instead of list of dictionary. 
    /// </summary>
    public class CsvReader
    {
        private const string LineSplitRe = @"\r\n|\n\r|\n|\r";
        private static readonly char[] TrimChars = { '\"' };

        public static List<DB.Food> Read(string file)
        {
            List<DB.Food> list = new();

            string[] lines = Regex.Split(file, LineSplitRe);

            if (lines.Length <= 1) return list;

            string[] header = Regex.Split(lines[0], "\t");

            for (int i = 1; i < lines.Length; ++i)
            {
                string[] values = Regex.Split(lines[i], "\t");
                if (values.Length == 0 || values[0] == "") continue;

                DB.Food food = new();

                for (int j = 0; j < header.Length && j < values.Length; ++j)
                {
                    string value = values[j];
                    value = value.TrimStart(TrimChars).TrimEnd(TrimChars).Replace("\\", "");
                    value = value.Replace("{LineBreak}", "\n");
                    
                    switch (j)
                    {
                        case 0:
                            food.foodName = value;
                            break;
                        case 1:
                            food.calories = float.Parse(value);
                            break;
                        case 2:
                            food.fat = float.Parse(value);
                            break;
                        case 3:
                            food.saturatedFat = float.Parse(value);
                            break;
                        case 4:
                            food.carbHyd = float.Parse(value);
                            break;
                        case 5:
                            food.sugar = float.Parse(value);
                            break;
                        case 6:
                            food.protein = float.Parse(value);
                            break;
                        case 7:
                            food.salt = float.Parse(value);
                            break;
                    }
                }

                list.Add(food);
            }

            return list;
        }
    }
}
