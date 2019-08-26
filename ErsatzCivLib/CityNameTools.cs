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
        private static readonly Dictionary<int, int> LENGTH_DISTRIBUTION = new Dictionary<int, int>
        {
            { 1, 1 }, { 2, 14 }, { 3, 137 }, { 4, 636 }, { 5, 1816 }, { 6, 3045 }, { 7, 3585 }, { 8, 3665 }, { 9, 3090 }, { 10, 2488 },
            { 11, 1720 }, { 12, 1132 }, { 13, 905 }, { 14, 956 }, { 15, 1193 }, { 16, 1326 }, { 17, 1421 }, { 18, 1500 }, { 19, 1394 },
            { 20, 1132 }, { 21, 881 }, { 22, 593 }, { 23, 363 }, { 24, 204 }, { 25, 120 }, { 26, 86 }, { 27, 51 }, { 28, 17 }, { 29, 21 },
            { 30, 7 }, { 31, 7 }, { 32, 8 }, { 33, 2 }, { 34, 3 }, { 35, 4 }, { 36, 2 }, { 37, 1 }, { 38, 1 }
        };
        private const char END_OF_DATAS = '#';
        private static Dictionary<CivilizationPivot, Dictionary<char, Tuple<int, Dictionary<char, int>>>> CHARS_STATS =
            new Dictionary<CivilizationPivot, Dictionary<char, Tuple<int, Dictionary<char, int>>>>();
        private static Dictionary<CivilizationPivot, Dictionary<char, int>> FIRST_CHAR_STATS =
            new Dictionary<CivilizationPivot, Dictionary<char, int>>();

        private static void GenerateCharStats(CivilizationPivot civ)
        {
            if (CHARS_STATS.ContainsKey(civ) && FIRST_CHAR_STATS.ContainsKey(civ))
            {
                return;
            }

            var tempCharStats = new Dictionary<char, Tuple<int, Dictionary<char, int>>>();
            var tempFirstCharStats = new Dictionary<char, int>();

            // TODO : files required for every civilizations !
            // string rName = $"{civ.Name.ToLowerInvariant()}_city_name";
            string rName = "french_city_name";

            var rows = Properties.Resources.ResourceManager.GetString(rName).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var row in rows)
            {
                int i = 0;
                foreach (var ch in row)
                {
                    if (i == 0)
                    {
                        if (!tempFirstCharStats.ContainsKey(ch))
                        {
                            tempFirstCharStats.Add(ch, 0);
                        }
                        tempFirstCharStats[ch]++;
                    }
                    else
                    {
                        if (!tempCharStats.ContainsKey(ch))
                        {
                            tempCharStats.Add(ch, new Tuple<int, Dictionary<char, int>>(1, new Dictionary<char, int>()));
                        }
                        else
                        {
                            tempCharStats[ch] = new Tuple<int, Dictionary<char, int>>(tempCharStats[ch].Item1 + 1, tempCharStats[ch].Item2);
                        }
                        var nextCh = i == row.Length - 1 ? END_OF_DATAS : row[i + 1];
                        if (!tempCharStats[ch].Item2.ContainsKey(nextCh))
                        {
                            tempCharStats[ch].Item2.Add(nextCh, 0);
                        }
                        tempCharStats[ch].Item2[nextCh]++;
                    }
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
            var nextI = Tools.Randomizer.Next(0, LENGTH_DISTRIBUTION.Sum(kvp => kvp.Value));

            int i = 0;
            while (nextI >= 0)
            {
                nextI -= LENGTH_DISTRIBUTION.ElementAt(i).Value;
                if (nextI <= 0)
                {
                    return LENGTH_DISTRIBUTION.ElementAt(i).Key;
                }
                i++;
            }

            throw new NotImplementedException("Should not occurs !");
        }

        private static char GetRandomFirstChar(CivilizationPivot civ)
        {
            var datas = FIRST_CHAR_STATS[civ];
            
            var rdm = Tools.Randomizer.Next(0, datas.Sum(kvp => kvp.Value));
            int i = 0;
            do
            {
                rdm -= datas.ElementAt(i).Value;
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
            while (charTmp.Value == END_OF_DATAS || (alreadyTwice && charTmp.Value == previousChar) || (forbidSpace && charTmp.Value == ' '));

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
                if (i == 0 || nameChars[i - 1] == ' ')
                {
                    nameChars[i] = GetRandomFirstChar(civilization);
                }
                else
                {
                    nameChars[i] = GetRandomNextChar(civilization, nameChars[i - 1],
                        i - 2 >= 0 && nameChars[i - 2] == nameChars[i - 1],
                        (i - 2 >= 0 && nameChars[i - 2] == ' ') || (i == 1) || (i == countChars - 2));
                }
            }

            return new string(nameChars);
        }
    }
}
