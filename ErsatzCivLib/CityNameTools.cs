using System;
using System.Collections.Generic;
using System.Linq;
using ErsatzCivLib.Model.Static;

namespace ErsatzCivLib
{
    /// <summary>
    /// Helpers to generate city name.
    /// </summary>
    internal static class CityNameTools
    {
        private const double AVERAGE_CHARS = 11.5;
        private const double STANDARD_DEVIATION_CHARS = 5.5;
        private const char END_OF_DATAS = ' ';
        private static Dictionary<CivilizationPivot, Dictionary<char, Tuple<int, Dictionary<char, int>>>> CHARS_STATS =
            new Dictionary<CivilizationPivot, Dictionary<char, Tuple<int, Dictionary<char, int>>>>();
        private static Dictionary<CivilizationPivot, Dictionary<char, Tuple<int, Dictionary<char, int>>>> FIRST_CHAR_STATS =
            new Dictionary<CivilizationPivot, Dictionary<char, Tuple<int, Dictionary<char, int>>>>();

        private static void GenerateCharStats(CivilizationPivot civ)
        {
            if (CHARS_STATS.ContainsKey(civ) && FIRST_CHAR_STATS.ContainsKey(civ))
            {
                return;
            }

            var tempCharStats = new Dictionary<char, Tuple<int, Dictionary<char, int>>>();
            var tempFirstCharStats = new Dictionary<char, Tuple<int, Dictionary<char, int>>>();

            // TODO : files required for every civilizations !
            // string rName = $"{civ.Name.ToLowerInvariant()}_city_name";
            string rName = "french_city_name";

            var rows = Properties.Resources.ResourceManager.GetString(rName).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var row in rows)
            {
                int i = 0;
                foreach (var ch in row)
                {
                    var usedHere = i == 0 || row[i - 1] == END_OF_DATAS ? tempFirstCharStats : tempCharStats;

                    if (!usedHere.ContainsKey(ch))
                    {
                        usedHere.Add(ch, new Tuple<int, Dictionary<char, int>>(1, new Dictionary<char, int>()));
                    }
                    else
                    {
                        usedHere[ch] = new Tuple<int, Dictionary<char, int>>(usedHere[ch].Item1 + 1, usedHere[ch].Item2);
                    }
                    var nextCh = i == row.Length - 1 ? END_OF_DATAS : row[i + 1];
                    if (!usedHere[ch].Item2.ContainsKey(nextCh))
                    {
                        usedHere[ch].Item2.Add(nextCh, 0);
                    }
                    usedHere[ch].Item2[nextCh]++;
                    i++;
                }
            }

            for (int i = 0; i <= 9; i++)
            {
                tempCharStats.Remove(i.ToString().First());
                tempFirstCharStats.Remove(i.ToString().First());
            }

            CHARS_STATS.Add(civ, tempCharStats);
            FIRST_CHAR_STATS.Add(civ, tempFirstCharStats);
        }

        private static int GetCityNameCharactersCount()
        {
            int tmp = 0;
            do
            {
                tmp = Convert.ToInt32(Math.Round(
                    AVERAGE_CHARS + STANDARD_DEVIATION_CHARS * (
                        Math.Pow(-2 * Math.Log(Tools.Randomizer.NextDouble()), 0.5)
                        * Math.Cos(2 * Math.PI * Tools.Randomizer.NextDouble())
                    )
                ));
            }
            while (tmp <= 0);

            return tmp;
        }

        private static char GetRandomFirstChar(CivilizationPivot civ)
        {
            var datas = FIRST_CHAR_STATS[civ];
            
            var rdm = Tools.Randomizer.Next(0, datas.Sum(kvp => kvp.Value.Item1));
            int i = 0;
            do
            {
                rdm -= datas.ElementAt(i).Value.Item1;
                if (rdm <= 0)
                {
                    return datas.ElementAt(i).Key;
                }
                i++;
            }
            while (rdm > 0);

            throw new InvalidOperationException("Should never occurs !");
        }

        private static char GetRandomNextChar(CivilizationPivot civ, char previousChar, bool alreadyTwice, bool forbidSpace)
        {
            var datas = CHARS_STATS[civ][previousChar].Item2;

            char? charTmp = null;

            do
            {
                var rdm = Tools.Randomizer.Next(0, datas.Sum(kvp => kvp.Value));
                int i = 0;
                do
                {
                    rdm -= datas.ElementAt(i).Value;
                    if (rdm <= 0)
                    {
                        charTmp = datas.ElementAt(i).Key;
                        break;
                    }
                    i++;
                }
                while (rdm > 0);
            }
            while ((alreadyTwice && charTmp.Value == previousChar) || (forbidSpace && charTmp.Value == END_OF_DATAS));

            return charTmp.Value;
        }

        /// <summary>
        /// Generates a city name for the specified <see cref="CivilizationPivot"/>.
        /// </summary>
        /// <param name="civilization">The civilization</param>
        /// <returns>The city name.</returns>
        internal static string GenerateCityName(CivilizationPivot civilization)
        {
            GenerateCharStats(civilization);

            var countChars = GetCityNameCharactersCount();
            var nameChars = new char[countChars];

            for (int i = 0; i < countChars; i++)
            {
                if (i == 0 || nameChars[i - 1] == END_OF_DATAS)
                {
                    nameChars[i] = GetRandomFirstChar(civilization);
                }
                else
                {
                    nameChars[i] = GetRandomNextChar(civilization, nameChars[i - 1],
                        i - 2 >= 0 && nameChars[i - 2] == nameChars[i - 1],
                        (i - 2 >= 0 && nameChars[i - 2] == END_OF_DATAS) || (i == 1) || (i == countChars - 2));
                }
            }

            return new string(nameChars);
        }
    }
}
