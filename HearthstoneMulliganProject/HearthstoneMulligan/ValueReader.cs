﻿using System;
using System.Collections.Generic;
using System.Linq;
using SmartBot.Database;

/// <summary>
/// Class which reads the user config values
/// </summary> 
public class ValueReader
{
    private static string ConfigPath = Environment.CurrentDirectory +
            @"\MulliganProfiles\MulliganCore.config";

    private class Lines
    {
        public const int
        MaxManaCost = 3,
        MaxManaCostWarlockAndHunter = 4,


        AttendMinionValueBool = 8,
        MinManaCostToAttendValue = 9,
        MinNeutralMinionValue = 10,
        IncreaseMinMinionValueIfMaxCostBool = 15,
        IgnoreValueIfCardIsX_DropEtcBool = 17,
            X_Drop = 18,
        IgnoreValueIf_2234_AndCoin = 19,
        IgnoreValueIf_244_AndCoin = 20,
        OnlyIgnoreValueIfNoBadEffect = 21,

        ComboCase1Priority = 24,
        ComboCase2Priority = 25,
        ComboCase3Priority = 26,

        AddMillhouseToBlackList = 30,
        AddSoulFireToBlackList = 31,


        MaxManaToInstantAddNeutralMinion = 35,
        MinCardQualityToInstantAddMinion = 37,


        AllowTwinsBool = 46,
        DontAllowTwinsIfManaCostAtLeast = 47;
    }

    public static SmartBot.Plugins.API.Card.CQuality MinCardQualityToInstantAddMinion
    {
        get
        {
            switch (GetStringReadedValue(Lines.MinCardQualityToInstantAddMinion).ToLower())
            {
                case "free":
                    return SmartBot.Plugins.API.Card.CQuality.Free;
                case "common":
                    return SmartBot.Plugins.API.Card.CQuality.Common;
                case "rare":
                    return SmartBot.Plugins.API.Card.CQuality.Rare;
                case "epic":
                    return SmartBot.Plugins.API.Card.CQuality.Epic;
                case "legendary":
                    return SmartBot.Plugins.API.Card.CQuality.Legendary;
                default:
                    return SmartBot.Plugins.API.Card.CQuality.Epic;
            }
        }
    }

    public class ValueIgnorer
    {
        public static bool OnlyAddIfNoBadEffect
        {
            get { return GetStringReadedValue(Lines.OnlyIgnoreValueIfNoBadEffect).Equals("true"); }
        }

        public static bool IgnoreValueIfCardIsX_DropEtc
        {
            get { return GetStringReadedValue(Lines.IgnoreValueIfCardIsX_DropEtcBool).Equals("true"); }
        }

        public static bool IgnoreValueIf_2234_AndCoin
        {
            get { return GetStringReadedValue(Lines.IgnoreValueIf_2234_AndCoin).Equals("true"); }
        }

        public static bool IgnoreValueIf_244_AndCoin
        {
            get { return GetStringReadedValue(Lines.IgnoreValueIf_244_AndCoin).Equals("true"); }
        }

        public static int GetXDrop
        {
            get { return Convert.ToInt32(GetStringReadedValue(Lines.X_Drop)); }
        }

        public static bool HandContains2234(List<CardTemplate> HandCards)
        {
            if (!HandCards.Any(x => x.Name.Equals("GAME_005"))) //Coin
                return false;

            return
                HandCards.Count(x => new NeutralMinion(x).BoardCard != null &&
                    x.Cost == 2) >= 2
                &&
                HandCards.Any(x => new NeutralMinion(x).BoardCard != null &&
                    x.Cost == 3)
                &&
                HandCards.Any(x => new NeutralMinion(x).BoardCard != null &&
                    x.Cost == 4);
        }

        public static bool HandContains224(List<CardTemplate> HandCards)
        {
            if (!HandCards.Any(x => x.Name.Equals("GAME_005"))) //Coin
                return false;

            return
                HandCards.Count(x => new NeutralMinion(x).BoardCard != null &&
                     x.Cost == 2) >= 2
                &&
                HandCards.Any(x => new NeutralMinion(x).BoardCard != null &&
                     x.Cost == 4);
        }

        public enum ComboPriorities : int
        {
            Low = 0,
            Medium = 1,
            High = 2
        }

        public static ComboPriorities ComboCase1Priority
        {
            get
            {
                switch (GetStringReadedValue(Lines.ComboCase1Priority))
                {
                    case "low":
                        return ComboPriorities.Low;
                    case "medium":
                        return ComboPriorities.Medium;
                    case "high":
                        return ComboPriorities.High;
                    default:
                        return ComboPriorities.Medium;

                }
            }
        }
        public static ComboPriorities ComboCase2Priority
        {
            get
            {
                switch (GetStringReadedValue(Lines.ComboCase2Priority))
                {
                    case "low":
                        return ComboPriorities.Low;
                    case "medium":
                        return ComboPriorities.Medium;
                    case "high":
                        return ComboPriorities.High;
                    default:
                        return ComboPriorities.High;

                }
            }
        }
        public static ComboPriorities ComboCase3Priority
        {
            get
            {
                switch (GetStringReadedValue(Lines.ComboCase3Priority))
                {
                    case "low":
                        return ComboPriorities.Low;
                    case "medium":
                        return ComboPriorities.Medium;
                    case "high":
                        return ComboPriorities.High;
                    default:
                        return ComboPriorities.Low;

                }
            }
        }

        private static Dictionary<ComboPriorities, int> AllComboCases = new Dictionary<ComboPriorities, int>
                {
                    { ComboCase1Priority, (int)ComboCase1Priority },
                    { ComboCase2Priority,(int)ComboCase2Priority },
                    { ComboCase3Priority, (int)ComboCase3Priority }
                };

        public static IEnumerable<KeyValuePair<ComboPriorities, int>> AllComboCasesSortedByPriority
        {
            get
            {
                return AllComboCases.OrderBy(x => -x.Value);
            }
        }
    }

    public static bool IncreaseMinMinionValueIfMaxCost
    {
        get { return GetStringReadedValue(Lines.IncreaseMinMinionValueIfMaxCostBool).Equals("true"); }
    }

    public static bool AllowTwins
    {
        get { return GetStringReadedValue(Lines.AllowTwinsBool).Equals("true"); }
    }

    public static int DontAllowTwinsIfManaCostAtLeast
    {
        get { return Convert.ToInt32(GetStringReadedValue(Lines.DontAllowTwinsIfManaCostAtLeast)); }
    }

    public static int MaxManaToInstantAddNeutralMinion
    {
        get { return Convert.ToInt32(GetStringReadedValue(Lines.MaxManaToInstantAddNeutralMinion)); }
    }

    public static int MinManaCostToAttendValue
    {
        get { return Convert.ToInt32(GetStringReadedValue(Lines.MinManaCostToAttendValue)); }
    }

    public static bool AttendMinionValue
    {
        get { return GetStringReadedValue(Lines.AttendMinionValueBool).Equals("true"); }
    }

    public static int MaxManaCost
    {
        get { return Convert.ToInt32(GetStringReadedValue(Lines.MaxManaCost)); }
    }

    public static int MaxManaCostWarlockAndHunter
    {
        get { return Convert.ToInt32(GetStringReadedValue(Lines.MaxManaCostWarlockAndHunter)); }
    }

    public static NeutralMinion.Value MinNeutralMinionValue
    {
        get
        {
            switch (GetStringReadedValue(Lines.MinNeutralMinionValue).ToLower())
            {
                case "bad":
                    return NeutralMinion.Value.Bad;
                case "medium":
                    return NeutralMinion.Value.Medium;
                case "good":
                    return NeutralMinion.Value.Good;
                case "excellent":
                    return NeutralMinion.Value.Excellent;
                default:
                    return NeutralMinion.Value.Medium;
            }
        }
    }

    public static NeutralMinion.Value IncreasedMinNeutralMinionValue
    {
        get
        {
            switch (MinNeutralMinionValue)
            {
                case NeutralMinion.Value.Bad:
                    return NeutralMinion.Value.Medium;
                case NeutralMinion.Value.Medium:
                    return NeutralMinion.Value.Good;
                case NeutralMinion.Value.Good:
                    return NeutralMinion.Value.Excellent;
                default:
                    return NeutralMinion.Value.Excellent;
            }
        }
    }

    public static class BlackList
    {
        public static bool AddSoulFire
        {
            get { return GetStringReadedValue(Lines.AddSoulFireToBlackList).Equals("true"); }
        }
        public static bool AddMillhouseManastorm
        {
            get { return GetStringReadedValue(Lines.AddMillhouseToBlackList).Equals("true"); }
        }
    }

    public static IEnumerable<string> GetOwnBlackListEntries()
    {
        List<string> Entries = new List<string>();
        bool reachedOwnBlackList = false;

        using (System.IO.StreamReader sr = new System.IO.StreamReader(ConfigPath))
        {
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();

                if (line.Contains("Own Black List End"))
                    reachedOwnBlackList = false;

                if (reachedOwnBlackList && line.Length > 2)
                {
                    int startPos = line.Contains(")") ? 1 : 0;
                    int endPos = startPos == 1 ? line.LastIndexOf(")")
                        : line.Length;

                    Entries.Add(line.Substring(startPos, endPos - startPos));
                }

                if (line.Contains("Own Black List"))
                    reachedOwnBlackList = true;
            }
            sr.Close();
        }

        return Entries;
    }

    private static string GetStringReadedValue(int line)
    {
        string searchedLine = System.IO.File.ReadAllLines(ConfigPath)[line - 1];


        int startPos = searchedLine.LastIndexOf("=") + 1;
        int endPos = searchedLine.Length;
        string filteredValue = searchedLine.Substring(startPos, endPos - startPos);
        filteredValue = filteredValue.Replace(" ", "");

        return filteredValue;
    }
}