using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartBotUI;
using SmartBotAPI.Plugins.API;
using SmartBotUI.Settings;
using CClass = SmartBot.Plugins.API.Card.CClass;
using SmartBot.Database;
using CType = SmartBot.Plugins.API.Card.CType;

namespace SmartBotUI.Mulligan
{
    [Serializable]
    public class bMulliganProfile : MulliganProfile
    {
        public class Druid
        {
            public static bool rampStructure { get; set; }
        }

        /// <summary>
        /// Converts mulligan card class to a board card class
        /// </summary> 
        public class BoardCard
        {
            /// <summary>
            /// The resulting Card
            /// </summary> 
            public CardTemplate ResultingBoardCard { get; private set; }
            public bool HasEffect { get; private set; }
            public int EffectCount { get; private set; }
            public bool IsMaxManaCard { get; private set; }

            public BoardCard(Card MulliganCard)
            {
                ResultingBoardCard = ConvertToBoardCard(MulliganCard);

                HasEffect = ResultingBoardCard.Mechanics.Count > 0;
                EffectCount = ResultingBoardCard.Mechanics.Count;
                IsMaxManaCard = MulliganCard.Cost == ValueReader.MaxManaCost;
            }

            private CardTemplate ConvertToBoardCard(Card MulliganCard)
            {
                return CardTemplate.LoadFromId(MulliganCard.Name);
            }
        }

        /// <summary>
        /// Class which (only) indicates if the card is a character card
        /// </summary> 
        public class ClassCards
        {
            #region classCards
            private static List<string> rougeMinions = new List<string>
            {
                "CS2_072",//Backstab
                "NEW1_004",//Vanish
                "EX1_131",//Defias Ringleader
                "CS2_073",//Cold Blood
                "EX1_126",//Betrayal
                "NEW1_005",//Kidnapper
                "EX1_613",//Edwin VanCleef
                "CS2_076",//Assassinate
                "EX1_144",//Shadowstep
                "EX1_133",//Perdition's Blade
                "CS2_080",//Assassin's Blade
                "EX1_581",//Sap
                "CS2_074",//Deadly Poison
                "EX1_129",//Fan of Knives
                "EX1_137",//Headcrack
                "CS2_075",//Sinister Strike
                "NEW1_014",//Master of Disguise
                "EX1_128",//Conceal
                "CS2_233",//Blade Flurry
                "EX1_134",//SI:7 Agent
                "EX1_522",//Patient Assassin
                "EX1_145",//Preparation
                "EX1_131t",//Defias Bandit
                "EX1_278",//Shiv
                "CS2_077",//Sprint
                "EX1_124",//Eviscerate
            };
            private static List<string> druidMinions = new List<string>
            {
                "CS2_232",//Ironbark Protector
                "CS2_012",//Swipe
                "NEW1_007",//Starfall
                "CS2_009",//Mark of the Wild
                "EX1_169",//Innervate
                "EX1_161",//Naturalize
                "EX1_tk9",//Treant
                "CS2_008",//Moonfire
                "EX1_578",//Savagery
                "EX1_571",//Force of Nature
                "EX1_570",//Bite
                "CS2_011",//Savage Roar
                "EX1_173",//Starfire
                "NEW1_007b",//Starfall
                "CS2_005",//Claw
                "CS2_013",//Wild Growth
                "NEW1_007a",//Starfall
                "EX1_165t1",//Druid of the Claw
                "EX1_165t2",//Druid of the Claw
                "CS2_013t",//Excess Mana
 
                "EX1_154",//Wrath
                "EX1_154a",//Wrath 3 Damage)
                "EX1_154b",//Wrath 1 Damage Draw)
 
                "EX1_165",//Druid of the Claw
                "EX1_165a",//Cat Form Charge)
                "EX1_165b",//Bear Form +2 Hp & Taunt)
 
                "EX1_178",//Ancient of War
                "EX1_178a",//Rooted +5 Hp & Taunt)
                "EX1_178b",//Uproot +5 Attack)
 
                "NEW1_008",//Ancient of Lore
                "NEW1_008a",//Ancient Teachings
                "NEW1_008b",//Ancient Secrets
 
                "EX1_160",//Power of the Wild
                "EX1_160a",//Summon a Panther 3/2 Panther)
                "EX1_160t",//Panther Token
                "EX1_160b",//Leader of the Pack +1/+1 to Your Minions)
 
                "EX1_164",//Nourish
                "EX1_164a",//Nourish Gain 2 Mana Crystals)
                "EX1_164b",//Nourish Draw 3 Cards)
 
                "EX1_573",//Cenarius
                "EX1_573a",//Demigod's Favor +2/+2)
                "EX1_573b",//Shan'do's Lesson 2 2/2 Treant's with Taunt)
                "EX1_573t",//Treant
 
                "EX1_166",//Keeper of the Grove
                "EX1_166a",//Moonfire
                "EX1_166b",//Dispel
 
                "EX1_158",//Soul of the Forest
                "EX1_158t",//Treant
 
                "EX1_155",//Mark of Nature
                "EX1_155a",//Mark of Nature
                "EX1_155b",//Mark of Nature
            };
            private static List<string> hunterMinions = new List<string>
            {
                "CS2_084",//Hunter's Mark
                "DS1_183",//Multi-Shot
                "EX1_539",//Kill Command
                "DS1_188",//Gladiator's Longbow
                "DS1_175",//Timber Wolf
                "DS1_185",//Arcane Shot
                "EX1_544",//Flare
                "EX1_549",//Bestial Wrath
                "DS1_070",//Houndmaster
                "DS1_184",//Tracking
                "EX1_617",//Deadly Shot
                "CS2_237",//Starving Buzzard
                "EX1_538",//Unleash the Hounds
                "EX1_534",//Savannah Highmane
                "EX1_531",//Scavenging Hyena
                "EX1_536",//Eaglehorn Bow
                "EX1_543",//King Krush
                "EX1_537",//Explosive Shot
                "DS1_178",//Tundra Rhino
                "EX1_538t",//Hound
                "NEW1_031",//Animal Companion
                "NEW1_034",//Huffer
                "NEW1_033",//Leokk
                "NEW1_032",//Misha
                "EX1_534t",//Hyena
                "EX1_554",//Snake Trap
                "EX1_554t",//Snake
                "EX1_610",//Explosive Trap
                "EX1_533",//Misdirection
                "EX1_609",//Snipe
                "EX1_611",//Freezing Trap
            };
            private static List<string> mageMinions = new List<string>
            {
                "CS2_022",//Polymorph
                "EX1_287",//Counterspell
                "CS2_031",//Ice Lance
                "EX1_295",//Ice Block
                "EX1_294",//Mirror Entity
                "EX1_594",//Vaporize
                "CS2_029",//Fireball
                "tt_010",//Spellbender
                "tt_010a",//Spellbender
                "NEW1_012",//Mana Wyrm
                "EX1_275",//Cone of Cold
                "CS2_028",//Blizzard
                "CS2_023",//Arcane Intellect
                "EX1_277",//Arcane Missiles
                "CS2_026",//Frost Nova
                "EX1_608",//Sorcerer's Apprentice
                "CS2_024",//Frostbolt
                "CS2_033",//Water Elemental
                "CS2_025",//Arcane Explosion
                "EX1_289",//Ice Barrier
                "EX1_612",//Kirin Tor Mage
                "CS2_tk1",//Sheep
                "CS2_mirror",//Mirror Image
                "CS2_032",//Flamestrike
                "EX1_559",//Archmage Antonidas
                "CS2_027",//Mirror Image
                "EX1_279",//Pyroblast
                "EX1_274",//Ethereal Arcanist
            };
            private static List<string> paladinMinions = new List<string>
            {
                "CS2_087",//Blessing of Might
                "EX1_136",//Redemption
                "EX1_379",//Repentance
                "EX1_365",//Holy Wrath
                "CS2_094",//Hammer of Wrath
                "CS2_089",//Holy Light
                "CS2_091",//Light's Justice
                "EX1_132",//Eye for an Eye
                "CS2_093",//Consecration
                "EX1_354",//Lay on Hands
                "EX1_366",//Sword of Justice
                "EX1_349",//Divine Favor
                "EX1_371",//Hand of Protection
                "EX1_619",//Equality
                "CS2_088",//Guardian of Kings
                "CS2_097",//Truesilver Champion
                "EX1_360",//Humility
                "EX1_383",//Tirion Fordring
                "CS2_092",//Blessing of Kings
                "EX1_382",//Aldor Peacekeeper
                "EX1_384",//Avenging Wrath
                "EX1_363",//Blessing of Wisdom
                "EX1_355",//Blessed Champion
                "EX1_383t",//Ashbringer
                "EX1_130",//Noble Sacrifice
                "EX1_130a",//Defender
            };
            private static List<string> priestMinions = new List<string>
            {
                "CS1_113",//Mind Control
                "EX1_350",//Prophet Velen
                "EX1_339",//Thoughtsteal
                "EX1_345",//Mindgames
                "EX1_334",//Shadow Madness
                "EX1_591",//Auchenai Soulpriest
                "EX1_091",//Cabal Shadow Priest
                "CS1_130",//Holy Smite
                "CS1_129",//Inner Fire
                "DS1_233",//Mind Blast
                "CS2_004",//Power Word: Shield
                "EX1_341",//Lightwell
                "CS1_112",//Holy Nova
                "EX1_335",//Lightspawn
                "EX1_332",//Silence
                "CS2_236",//Divine Spirit
                "EX1_621",//Circle of Healing
                "EX1_622",//Shadow Word: Death
                "EX1_623",//Temple Enforcer
                "EX1_624",//Holy Fire
                "EX1_626",//Mass Dispel
                "CS2_234",//Shadow Word: Pain
                "EX1_625",//Shadowform
                "EX1_625t",//Mind Spike
                "EX1_625t2",//Mind Shatter
                "CS2_235",//Northshire Cleric
                "EX1_345t",//Shadow of Nothing
                "CS2_003",//Mind Vision
            };
            private static List<string> shamanMinions = new List<string>
            {
                "NEW1_010",//Al'Akir the Windlord
                "CS2_039",//Windfury
                "CS2_041",//Ancestral Healing
                "EX1_587",//Windspeaker
                "CS2_042",//Fire Elemental
                "CS2_045",//Rockbiter Weapon
                "EX1_248",//Feral Spirit
                "EX1_251",//Forked Lightning
                "EX1_567",//Doomhammer
                "CS2_038",//Ancestral Spirit
                "EX1_238",//Lightning Bolt
                "EX1_575",//Mana Tide Totem
                "EX1_tk11",//Spirit Wolf
                "hexfrog",//Frog
                "EX1_243",//Dust Devil
                "EX1_259",//Lightning Storm
                "EX1_246",//Hex
                "EX1_245",//Earth Shock
                "EX1_258",//Unbound Elemental
                "CS2_053",//Far Sight
                "EX1_244",//Totemic Might
                "EX1_241",//Lava Burst
                "EX1_247",//Stormforged Axe
                "CS2_037",//Frost Shock
                "EX1_565",//Flametongue Totem
                "EX1_250",//Earth Elemental
                "CS2_046",//Bloodlust
            };
            private static List<string> warlockMinions = new List<string>
            {
                "EX1_320",//Bane of Doom
                "CS2_065",//Voidwalker
                "EX1_303",//Shadowflame
                "NEW1_003",//Sacrificial Pact
                "EX1_310",//Doomguard
                "CS2_059",//Blood Imp
                "EX1_301",//Felguard
                "EX1_306",//Succubus
                "EX1_313",//Pit Lord
                "EX1_316",//Power Overwhelming
                "EX1_312",//Twisting Nether
                "EX1_317",//Sense Demons
                "CS2_057",//Shadow Bolt
                "CS2_061",//Drain Life
                "CS2_062",//Hellfire
                "EX1_315",//Summoning Portal
                "EX1_308",//Soulfire
                "CS2_063",//Corruption
                "CS2_064",//Dread Infernal
                "EX1_309",//Siphon Soul
                "EX1_596",//Demonfire
                "EX1_304",//Void Terror
                "EX1_317t",//Worthless Imp
                "EX1_319",//Flame Imp
                "EX1_302",//Mortal Coil
                "EX1_tk33",//INFERNO!
 
                "EX1_323",//Lord Jaraxxus
                "EX1_323h",//Lord Jaraxxus Hero Power Summon a 6/6 Infernal Token Demon))
                "EX1_tk34",//Infernal
                "EX1_323w",//Blood Fury
            };
            private static List<string> warriorMinions = new List<string>
            {
                "CS2_105",//Heroic Strike
                "EX1_607",//Inner Rage
                "NEW1_011",//Kor'kron Elite
                "EX1_407",//Brawl
                "EX1_603",//Cruel Taskmaster
                "CS2_112",//Arcanite Reaper
                "EX1_414",//Grommash Hellscream
                "CS2_103",//Charge
                "EX1_392",//Battle Rage
                "CS2_106",//Fiery War Axe
                "EX1_409",//Upgrade!
                "EX1_410",//Shield Slam
                "EX1_402",//Armorsmith
                "CS2_108",//Execute
                "EX1_408",//Mortal Strike
                "EX1_411",//Gorehowl
                "CS2_114",//Cleave
                "EX1_084",//Warsong Commander
                "EX1_606",//Shield Block
                "NEW1_036",//Commanding Shout
                "EX1_391",//Slam
                "CS2_104",//Rampage
                "EX1_409t",//Heavy Axe
                "EX1_398t",//Battle Axe
                "EX1_400",//Whirlwind
                "EX1_604",//Frothing Berserker
            };
            #endregion classCards

            public static List<List<string>> classCardCollectionList = new List<List<string>>
            {
                rougeMinions,
                druidMinions,
                hunterMinions,
                mageMinions,
                paladinMinions,
                priestMinions,
                shamanMinions,
                warlockMinions,
                warriorMinions
            };

            public static bool IsClassCard(Card card)
            {
                return classCardCollectionList.Any(x => x.Contains(card.Name));
            }
        }

        /// <summary>
        /// Class which (only) indicates if the mulligan card has a particular property
        /// </summary> 
        public class CardProperties
        {
            public static bool HasEffect(Card card, string PropertyName)
            {
                string property_STR = new PropertyEnumClass(PropertyName).Property;
                var boardCard = new BoardCard(card).ResultingBoardCard;

                return
                    boardCard.Mechanics.Any(x => String.Equals(x,
                        property_STR, StringComparison.OrdinalIgnoreCase));
            }
            /// <summary>
            /// For board cards
            /// </summary> 
            public static bool HasEffect(CardTemplate card, string PropertyName)
            {
                string property_STR = new PropertyEnumClass(PropertyName).Property;
                var boardCard = card;

                return
                    boardCard.Mechanics.Any(x => String.Equals(x,
                        property_STR, StringComparison.OrdinalIgnoreCase));
            }

            public static bool HasBadEffect(CardTemplate card)
            {
                /// <summary>
                /// Some cards will be added to hand anyway because of other aspects
                /// </summary>
                List<string> CardsWithBadProperties = new List<string>
                {
                    "EX1_577",//The Beast
                    "CS2_227",//Venture Co. Mercenary
                    "EX1_045",//Ancient Watcher
                    "NEW1_030",//Deathwing
                    "FP1_001",//Zombie Chow
                    "TU4c_001",//King Mukla
                };

                return HasEffect(card, "Damage All") ||
                    HasEffect(card, "Overload") ||
                    CardsWithBadProperties.Contains(card.Name);
            }

            public static bool HasBadEffect(Card card)
            {
                /// <summary>
                /// Some cards will be added to hand anyway because of other aspects
                /// </summary>
                List<string> CardsWithBadProperties = new List<string>
                {
                    "EX1_577",//The Beast
                    "CS2_227",//Venture Co. Mercenary
                    "EX1_045",//Ancient Watcher
                    "NEW1_030",//Deathwing
                    "FP1_001",//Zombie Chow
                    "TU4c_001",//King Mukla
                };

                return HasEffect(card, "Damage All") ||
                    HasEffect(card, "Overload") ||
                    CardsWithBadProperties.Contains(card.Name);
            }
        }

        class PropertyEnumClass
        {
            public string Property { get; private set; }

            public PropertyEnumClass(string EffectName)
            {
                Property = effects.Any(x => x.Equals(EffectName)) ?
                    effects.FirstOrDefault(x => x.Equals(EffectName)) :
                    effects[0];
            }

            private List<string> effects = new List<string>
            {
                "Error_PropertyNotFoundException",
                "Battle Cry",
                "Buffs",
                "Charge",
                "Choose",
                "Combo",
                "Damage All",
                "Damage Enemies",
                "Deal Damage",
                "Deathrattle",
                "Divine Shield",
                "Draw Card",
                "Enrage",
                "Freeze",
                "Gain Armor",
                "Overload",
                "Random",
                "Restore Health",
                "Secret",
                "Silence",
                "Spare Part",
                "Spell Damage",
                "Stealth",
                "Taunt",
                "Windfury"
            };
        }

        /// <summary>
        /// Class which includes the quality of a neutral minion
        /// </summary> 
        public class NeutralMinion
        {
            public BoardCard BoardCard { get; private set; }
            public Value CardValue { get; private set; }

            public enum Value : int
            {
                Bad = 0,
                Medium = 1,
                Good = 2,
                Excellent = 3
            }

            public NeutralMinion(Card MulliganNeutralMinionCard)
            {
                BoardCard = !ClassCards.IsClassCard(MulliganNeutralMinionCard) ?
                    new BoardCard(MulliganNeutralMinionCard) : null;

                if (BoardCard != null)
                    SetCardValue();
                else
                    CardValue = Value.Bad;
            }

            private void SetCardValue()
            {
                float rawValue = (BoardCard.ResultingBoardCard.Atk + BoardCard.ResultingBoardCard.Health) / 2;
                float resultingValue = rawValue - BoardCard.ResultingBoardCard.Cost;

                CardValue = resultingValue > 0 ? Value.Good : Value.Medium;

                if (CardProperties.HasBadEffect(BoardCard.ResultingBoardCard))
                {
                    switch (CardValue)
                    {
                        case Value.Medium: CardValue = Value.Bad; break;
                        case Value.Good: CardValue = Value.Medium; break;
                    }
                }

                CardValue = BoardCard.EffectCount > 0 && CardValue == Value.Good ? Value.Excellent :
                    CardValue;
            }
        }

        /// <summary>
        /// Class which reads the user config values
        /// </summary> 
        public class ValueReader
        {
            private static string ConfigPath = Environment.CurrentDirectory +
                    @"\MulliganProfiles\MulliganCore.config";

            public class Lines
            {
                public static int
                MaxManaCost = 3,
                MaxManaCostWarlockAndHunter = 4,


                AttendMinionValueBool = 8,
                MinManaCostToAttendValue = 9,
                MinNeutralMinionValue = 10,
                IncreaseMinMinionValueIfMaxCostBool = 15,
                IgnoreValueIfCardIsX_DropEtcBool = 17,
                    X_Drop = 18,
                MakeIgnoringOnlyIfNotBadPropertyBool = 19,


                AddMillhouseToBlackList = 23,
                AddSoulFireToBlackList = 24,


                MaxManaToInstantAddNeutralMinion = 28,
                OnlyAddMinionIfHasEffect = 29,
                MinCardQualityToInstantAddMinion = 30,


                AllowTwinsBool = 41,
                DontAllowTwinsIfManaCostAtLeast = 42;
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
                public static bool IgnoreValueIfCardIsX_DropEtc
                {
                    get { return GetStringReadedValue(Lines.IgnoreValueIfCardIsX_DropEtcBool).Equals("true"); }
                }

                public static int GetXDrop
                {
                    get { return Convert.ToInt32(GetStringReadedValue(Lines.X_Drop)); }
                }

                public static bool MakeIgnoringOnlyIfNotBadEffect
                {
                    get { return GetStringReadedValue(Lines.MakeIgnoringOnlyIfNotBadPropertyBool).Equals("true"); }
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

            public static bool OnlyAddMinionIfHasEffect
            {
                get { return GetStringReadedValue(Lines.OnlyAddMinionIfHasEffect).Equals("true"); }
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

            public static List<string> GetOwnBlackListEntries()
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
                            int endPos = startPos == 1 ? line.LastIndexOf(")") : line.Length;

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
            {                string searchedLine = System.IO.File.ReadAllLines(ConfigPath)[line - 1];


                int startPos = searchedLine.LastIndexOf("=") + 1;
                int endPos = searchedLine.Length;
                string filteredValue = searchedLine.Substring(startPos, endPos - startPos);
                filteredValue = filteredValue.Replace(" ", "");

                return filteredValue;
            }
        }

        List<string> whiteList = new List<string>() { "GAME_005" /*Coin*/ };
        List<Card> chosenCards = new List<Card>();
        List<string> blackList = new List<string>();

        public bMulliganProfile()
            : base()
        {
            Druid.rampStructure = false;
        }

        public override List<Card> HandleMulligan(List<Card> Choices, CClass opponentClass, CClass ownClass)
        {
            int MaxManaCost = ValueReader.MaxManaCost;

            if ((ownClass == CClass.HUNTER || ownClass == CClass.WARLOCK) &&
            SettingsManager.BotMode != SettingsManager.Mode.Arena &&
            SettingsManager.BotMode != SettingsManager.Mode.ArenaAuto)
            {
                MaxManaCost = ValueReader.MaxManaCostWarlockAndHunter;
            }

            LoadOldBlackListEntries(opponentClass, ownClass);
            foreach (var ownBlackListEntry in ValueReader.GetOwnBlackListEntries())
            {
                blackList.Add(ownBlackListEntry);
            }
            #region ListManaging
            if (ValueReader.BlackList.AddMillhouseManastorm)
                blackList.Add("NEW1_029");//Millhouse
            blackList.Add("CS2_118");//Magma Rager
            blackList.Add("EX1_132"); //Eye for an Eye
            blackList.Add("CS2_231"); //Wisp

            switch (ownClass)
            {
                case CClass.DRUID:
                    blackList.Add("CS2_012");//Swipe
                    if (Choices.Count(x => new BoardCard(x).ResultingBoardCard.Type == CType.MINION) >= 2)
                        blackList.Add("CS2_011");//Savage Roar
                    whiteList.Add("EX1_169");//Innervate
                    whiteList.Add("EX1_154");//Wrath
                    if (Choices.Any(x => x.Name == "EX1_169" || x.Name == "CS2_013"))
                    {
                        whiteList.Add("FP1_005");//Shade of Naxxramas
                        Druid.rampStructure = true;
                    }
                    if (Druid.rampStructure)
                    {
                        whiteList.Add("EX1_085");//Mind Control Tech
                        whiteList.Add("GVG_096");//Piloted Shredder
                    }
                    whiteList.Add("CS2_013");//Wild Growth
                    if (Choices.Any(x => CardProperties.HasEffect(x, "Taunt")
                        && x.Name != "FP1_028" && x.Cost <= 3))
                    {
                        whiteList.Add("FP1_028");//Undertaker
                        whiteList.Add(Choices.FirstOrDefault(x => CardProperties.HasEffect(x, "Deathrattle")
                            && x.Name != "FP1_028" && x.Cost <= 3).Name);
                    }
                    if (Choices.Count(x => CardProperties.HasEffect(x, "Deathrattle")) < 
                        Choices.Count(x => x.Name != whiteList[0]))
                        whiteList.Add("CS2_009");//Mark of the Wild
                    whiteList.Add("CS2_005");//Claw
                    if (Choices.Any(x => new BoardCard(x).ResultingBoardCard.Type == CType.WEAPON 
                        || x.Name == "CS2_005"))
                        whiteList.Add("EX1_578");//Savagery
                    else
                        blackList.Add("EX1_578");//Savagery
                    break;
                case CClass.HUNTER:
                    whiteList.Add("NEW1_031");//Animal Companion
                    whiteList.Add("EX1_617");//Deadly Shot
                    if (Choices.Count(x => new BoardCard(x).ResultingBoardCard.Type == CType.MINION
                        && !blackList.Contains(x.Name)) >= 2)
                        whiteList.Add("EX1_611");//Freezing Trap
                    else
                        blackList.Add("EX1_611");//Freezing Trap
                    if (!Choices.Any(x => CardProperties.HasEffect(x, "Stealth")))
                        whiteList.Add("EX1_544");//Flare
                    else
                        blackList.Add("EX1_544");//Flare

                    if (Choices.Count(x => CardProperties.HasEffect(x, "Deathrattle")) >= 2)
                        whiteList.Add("GVG_026");//Feign Death
                    else
                        blackList.Add("GVG_026");//Feign Death

                    if (Choices.Count(x => new BoardCard(x).ResultingBoardCard.Type == CType.MINION
                        && new BoardCard(x).ResultingBoardCard.Race == SmartBot.Plugins.API.Card.CRace.BEAST &&
                        !blackList.Contains(x.Name)) >= 3)
                        whiteList.Add("DS1_175");//Timber Wolf
                    else
                        blackList.Add("DS1_175");//Timber Wolf
                    whiteList.Add("GVG_043");//Glaivezooka
                    whiteList.Add("GVG_087");//Steamwheedle Sniper
                    whiteList.Add("BRM_013");//Quick Shot
                    whiteList.Add("FP1_011");//Webspinner
                    whiteList.Add("DS1_184");//Tracking
                    whiteList.Add("DS1_185");//Arcane Shot
                    if (Choices.Count > 3)
                        whiteList.Add("EX1_014");//Mukla
                    break;
                case CClass.MAGE:
                    whiteList.Add("EX1_608");//Sorcerer's Apprentice
                    whiteList.Add("GVG_002");//Snowchugger
                    whiteList.Add("CS2_024");//Frostbolt
                    whiteList.Add("GVG_001");//Flamecannon
                    whiteList.Add("CS2_mirror");//Mirror Image
                    whiteList.Add("NEW1_012");//Mana Wyrm
                    whiteList.Add("EX1_277");//Arcane Missiles
                    break;
                case CClass.PALADIN:
                    whiteList.Add("EX1_366");//Sword of Justice
                    if (opponentClass == CClass.DRUID || opponentClass == CClass.WARLOCK)
                        whiteList.Add("GVG_101");//Scarlet Purifier
                    whiteList.Add("GVG_061");//Muster for Battle
                    whiteList.Add("EX1_382");//Aldor Peacekeeper
                    whiteList.Add("GVG_058");//Shielded Minibot
                    whiteList.Add("EX1_362");//Argent Protector
                    whiteList.Add("EX1_130");//Noble Sacrifice
                    whiteList.Add("CS2_091");//Light's Justice
                    whiteList.Add("EX1_363");//Blessing of Wisdom
                    whiteList.Add("CS2_087");//Blessing of Might
                    whiteList.Add("FP1_020");//Avenges
                    break;
                case CClass.PRIEST:
                    whiteList.Add("EX1_339");//Thoughtsteal
                    whiteList.Add("GVG_072");//Shadowboxer
                    whiteList.Add("CS2_234");//Shadow Word: Pain
                    if (Choices.Any(x => new BoardCard(x).ResultingBoardCard.Type == CType.MINION 
                        && x.Cost <= 3 && !blackList.Contains(x.Name)))
                        whiteList.Add("CS2_236");//Divine Spirit
                    else
                        blackList.Add("CS2_236");//Divine Spirit
                    whiteList.Add("GVG_009");//Shadowbomber
                    whiteList.Add("CS2_004");//Power Word: Shield
                    whiteList.Add("CS2_235");//Northshire Cleric
                    whiteList.Add("EX1_332");//Silence
                    whiteList.Add("CS1_130");//Holy Smite
                    whiteList.Add("CS1_129");//Inner Fire

                    whiteList.Add("CS2_181");//Injured Blademaster
                    if (Choices.Any(x => x.Name == "CS2_181"))
                        whiteList.Add("EX1_621"); // Circle of Healing
				    else
					    blackList.Add("EX1_621");

                    if (Choices.Count > 3)
                    {
                        if (Choices.Any(x => x.Name == "CS2_235"))//Northshire Cleric
                            whiteList.Add("CS2_004");//Power Word: Shield
                        if (Choices.Any(x => x.Name == "CS2_181"))//Injured Blademaster	
                            whiteList.Add("EX1_621");//Circle of Healing
                    }

                    if (opponentClass == CClass.WARRIOR || opponentClass == CClass.PALADIN)
                        whiteList.Add("EX1_588");       
                    else
                        blackList.Add("EX1_588");

                    if (Choices.Count(c => c.Name == "FP1_001" || c.Name == "CS2_235" || c.Name == "GVG_081") > 1)
                    {
                        whiteList.Add("CS2_004");//Power Word: Shield
        	            whiteList.Add("GVG_010");//Velen's Chosen    
                	    whiteList.Add("FP1_009");//Deathlord  
                    }
                    break;
                case CClass.ROGUE:
                    if (Choices.Any(x => x.Cost <= 1 && (new BoardCard(x).ResultingBoardCard.Type == CType.SPELL 
                        || new BoardCard(x).ResultingBoardCard.Type == CType.WEAPON)) &&
                        !Choices.Any(x => x.Name == "EX1_131"))
                        whiteList.Add("EX1_134"); //SI:7-Agent
                    whiteList.Add("EX1_129"); //Dolchfächer
                    whiteList.Add("EX1_126"); //Verrat
                    if (Choices.Any(x => x.Cost <= 1 && (new BoardCard(x).ResultingBoardCard.Type == CType.SPELL
                        || new BoardCard(x).ResultingBoardCard.Type == CType.WEAPON)))
                    whiteList.Add("EX1_131"); //Rädelsführer der Defias
                    whiteList.Add("GVG_023"); //Goblinbarbier-o-Mat
                    whiteList.Add("EX1_522"); //Geduldiger Attentäter
                    whiteList.Add("EX1_124"); //Ausweiden
                    whiteList.Add("CS2_074"); //Tödliches Gift
                    whiteList.Add("CS2_073"); //Kaltblütigkeit
                    whiteList.Add("CS2_072"); //Meucheln
                    whiteList.Add("CS2_075"); //Finsterer Stoß
                    whiteList.Add("EX1_145"); //Vorbereitung
                    break;
                case CClass.SHAMAN:
                    whiteList.Add("EX1_248"); //Wildgeist
                    whiteList.Add("EX1_575"); //Manafluttotem
                    whiteList.Add("EX1_259"); //Gewittersturm
                    whiteList.Add("EX1_258"); //Entfesselter Elementar
                    whiteList.Add("GVG_037"); //Wirbelnder Zapp-o-Mat
                    whiteList.Add("CS2_039"); //Windzorn
                    whiteList.Add("EX1_247");//Stormforged Axe
                    whiteList.Add("FP1_025");//Reincarnate
                    whiteList.Add("EX1_565");//Flametongue Totem
                    whiteList.Add("GVG_038");//Crackle
                    whiteList.Add("CS2_045");//Rockbiter Weapon
                    whiteList.Add("EX1_238");//Lightning Bolt
                    whiteList.Add("CS2_037");//Frost Shock
                    whiteList.Add("EX1_251");//Forked Lightning
                    whiteList.Add("EX1_245");//Earth Shock
                    whiteList.Add("EX1_243");//Dust Devil
                    whiteList.Add("EX1_244");//Totemic Might
                    whiteList.Add("CS2_041");//Ancestral Healing
                    break;
                case CClass.WARLOCK:
                    whiteList.Add("BRM_005");//Demonwrath
                    whiteList.Add("CS2_065");//Voidwalker
                    whiteList.Add("EX1_306");//Succubus
                    whiteList.Add("EX1_596");//Demonfire
                    whiteList.Add("GVG_015");//Darkbomb
                    if (ValueReader.BlackList.AddSoulFire)
                        blackList.Add("EX1_308");//Soulfire
                    else
                        whiteList.Add("EX1_308");//Soulfire
                    whiteList.Add("EX1_302");//Mortal Coil
                    whiteList.Add("EX1_319");//Flame Imp
                    whiteList.Add("CS2_059");//Blood Imp
                    break;
                case CClass.WARRIOR:
                    if (Choices.Any(x => new BoardCard(x).ResultingBoardCard.Type == CType.MINION))
                        whiteList.Add("EX1_402");//Armorsmith
                    if (Choices.Any(x => new BoardCard(x).ResultingBoardCard.Type == CType.MINION && 
                        new BoardCard(x).ResultingBoardCard.Health > 1))
                        whiteList.Add("CS2_104");//Rampage
                    if (Choices.Any(x => x.Name == "EX1_007" || x.Name == "EX1_393"))
                        whiteList.Add("EX1_607"); //Inner Rage
                    else
                        blackList.Add("EX1_607");
                    if (Choices.Any(x => x.Name == "EX1_607"))
                        whiteList.Add("EX1_007"); //Acolyte
                    whiteList.Add("EX1_604");//Frothing Berserker
                    {
                        float averageMana = 0;
                        foreach (var card in Choices)
                            averageMana += card.Cost;
                        averageMana = averageMana / Choices.Count;
                        if (averageMana < 4f)
                            whiteList.Add("GVG_050");//Bouncing Blade
                    }

                    switch (opponentClass)
                    {
                        case CClass.WARLOCK:
                            whiteList.Add("EX1_402");//Armorsmith
                            whiteList.Add("FP1_021");//Death's Bite
                            break;
                        case CClass.SHAMAN:
                            whiteList.Add("EX1_402");//Armorsmith
                            whiteList.Add("FP1_021");//Death's Bite
                            break;
                        case CClass.PRIEST:
                            whiteList.Add("EX1_402");//Armorsmith
                            whiteList.Add("EX1_606");//Shield Block
                            whiteList.Add("EX1_410");//Shield Slam
                            whiteList.Add("EX1_007");//Acolyte of Pain
                            whiteList.Add("FP1_021");//Death's Bite
                            break;
                        case CClass.ROGUE:
                            whiteList.Add("EX1_606");//Shield Block
                            whiteList.Add("EX1_410");//Shield Slam	
                            whiteList.Add("FP1_021");//Death's Bite
                            break;
                    }

                    whiteList.Add("EX1_391");//Slam
                    whiteList.Add("CS2_105");//Heroic Strike
                    whiteList.Add("CS2_106");//Fiery War Axe
                    whiteList.Add("EX1_603");//Cruel Taskmaster
                    whiteList.Add("NEW1_036");//Commanding Shout
                    whiteList.Add("CS2_114");//Cleave
                    whiteList.Add("GVG_051");//Warbot
                    if (Choices.Any(x => new BoardCard(x).ResultingBoardCard.Type == CType.WEAPON))
                        whiteList.Add("EX1_409");//Upgrade!
                    whiteList.Add("EX1_410");//Shield Slam
                    whiteList.Add("CS2_108");//Execute
                    break;

            }

            switch (opponentClass)
            {
                case CClass.DRUID:
                    whiteList.Add("EX1_591");//Auchenai Priest
                    whiteList.Add("EX1_339");//Thoughtsteal
                    whiteList.Add("EX1_621");//Circle of Healing
                    break;
                case CClass.WARRIOR:
                    whiteList.Add("EX1_591");//Auchenai Priest
                    whiteList.Add("EX1_339");//Thoughtsteal
                    whiteList.Add("EX1_621");//Circle of Healing
                    whiteList.Add("CS2_179");//Sen'jin Shieldmasta
                    break;
                case CClass.ROGUE:
                    whiteList.Add("EX1_591");//Auchenai Priest
                    whiteList.Add("EX1_621");//Circle of Healing
                    whiteList.Add("EX1_339");//Thoughtsteal
                    whiteList.Add("FP1_030");//Loatheb
                    whiteList.Add("CS2_179");//Sen'jin Shieldmasta
                    break;
                case CClass.PRIEST:
                    whiteList.Add("EX1_591");//Auchenai Priest
                    whiteList.Add("EX1_339");//Thoughtsteal
                    whiteList.Add("EX1_621");//Circle of Healing
                    whiteList.Add("FP1_030");//Loatheb
                    break;
            }
            #endregion ListManaging

            CalculateMulligan(Choices, MaxManaCost, ownClass);

            return chosenCards;
        }

        private void CalculateMulligan(List<Card> Choices, int MaxManaCost, CClass ownClass)
        {
            #region TwinManaging
            for (int i = 0; i < Choices.Count; i++)
            {
                for (int j = 0; j < Choices.Count; j++)
                {
                    if (i != j && Choices[i].Name.Equals(Choices[j].Name))
                    {
                        if (Choices[i].Cost < ValueReader.DontAllowTwinsIfManaCostAtLeast &&
                            ValueReader.AllowTwins)
                            continue;
                        else if (!ValueReader.AllowTwins || Choices[i].Cost >= ValueReader.DontAllowTwinsIfManaCostAtLeast)
                        {
                            chosenCards.Add(Choices[i]);
                            blackList.Add(Choices[i].Name);
                        }
                    }
                }
            }
            #endregion TwinManaging

            foreach (var card in Choices.Where(x => !blackList.Contains(x.Name)))
            {
                if (card.Name == "GAME_005") //Coin
                {
                    chosenCards.Add(card);
                    continue;
                }


                if (whiteList.Contains(card.Name))
                    chosenCards.Add(card);
                else if (new NeutralMinion(card).BoardCard != null &&
                    card.Cost <= MaxManaCost && !blackList.Contains(card.Name))
                        ManageNeutralMinion(card, MaxManaCost, Choices);
            }
        }

        private void ManageNeutralMinion(Card card, int maxMana, List<Card> HandCards)
        {
            //<= max mana
            var boardCard = new BoardCard(card);

            if (boardCard.ResultingBoardCard.Quality >= ValueReader.MinCardQualityToInstantAddMinion) //epic by default
                chosenCards.Add(card);
            else if (boardCard.ResultingBoardCard.Cost <= ValueReader.MaxManaToInstantAddNeutralMinion) // min insta add cost
                chosenCards.Add(card);
            else
            { //card quality not hight enough and mana to high too
                if (!ValueReader.AttendMinionValue)
                    chosenCards.Add(card);
                else if (boardCard.ResultingBoardCard.Cost >= ValueReader.MinManaCostToAttendValue)
                {
                    if ((ValueReader.OnlyAddMinionIfHasEffect && boardCard.HasEffect) ||
                        !ValueReader.OnlyAddMinionIfHasEffect)
                    {
                        var minionCard = new NeutralMinion(card);
                        NeutralMinion.Value resultingMinNeutralMinionValue =
                            minionCard.BoardCard.IsMaxManaCard && ValueReader.IncreaseMinMinionValueIfMaxCost
                            ?
                            ValueReader.IncreasedMinNeutralMinionValue
                            :
                            ValueReader.MinNeutralMinionValue;

                        int X_Config_Drop = ValueReader.ValueIgnorer.GetXDrop;


                        if (minionCard.CardValue >= resultingMinNeutralMinionValue)
                            chosenCards.Add(card);
                        else if (card.Cost == X_Config_Drop &&
                            HandCards.Count(x => new NeutralMinion(x).BoardCard != null && x.Cost == X_Config_Drop - 1) > 0 &&
                            HandCards.Count(x => new NeutralMinion(x).BoardCard != null && x.Cost == X_Config_Drop - 2) > 0 &&
                            ValueReader.ValueIgnorer.IgnoreValueIfCardIsX_DropEtc)
                        {
                            //card is X drop and hand contain "x - 1 drop" and "x - 2 drop"
                            if (
                                !ValueReader.ValueIgnorer.MakeIgnoringOnlyIfNotBadEffect
                                ||
                                (ValueReader.ValueIgnorer.MakeIgnoringOnlyIfNotBadEffect &&
                                !CardProperties.HasBadEffect(card))
                                )
                            {
                                //add x drop
                                chosenCards.Add(card);

                                //add BEST x - 1 drop and x - 2 drop
                                var bestX_1Drop =
                                    HandCards.Where(x => new NeutralMinion(x).BoardCard != null && x.Cost == X_Config_Drop - 1).
                                    OrderBy(x => new NeutralMinion(x).CardValue).First();
                                var bestX_2Drop =
                                    HandCards.Where(x => new NeutralMinion(x).BoardCard != null && x.Cost == X_Config_Drop - 2).
                                    OrderBy(x => new NeutralMinion(x).CardValue).First();

                                if (!chosenCards.Contains(bestX_1Drop))
                                {
                                    chosenCards.Add(bestX_1Drop);
                                    blackList.Add(bestX_1Drop.Name);
                                }

                                if (!chosenCards.Contains(bestX_2Drop))
                                {
                                    chosenCards.Add(bestX_2Drop);
                                    blackList.Add(bestX_2Drop.Name);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LoadOldBlackListEntries(CClass opponentClass, CClass myClass)
        {
            #region rawBlacklist
            if (opponentClass != CClass.PALADIN && opponentClass != CClass.HUNTER)
            {
                blackList.Add("EX1_007");//Acolyte of Pain
            }
            blackList.Add("FP1_025");//Reincarnate
            blackList.Add("CS2_038");//Ancestral Spirit

            blackList.Add("EX1_349");//Divine Favor
            blackList.Add("CS2_023");//Arcane Intellect
            blackList.Add("CS2_011");//Savage roar
            blackList.Add("EX1_622");//Shadow Word Death
            blackList.Add("EX1_625");//Shadow Form
            blackList.Add("DS1_233");//Mind Blast
            blackList.Add("CS2_108");//Execute
            blackList.Add("EX1_391");//Slam
            blackList.Add("EX1_005");//BGH
            blackList.Add("CS2_007");//Healing Touch
            blackList.Add("EX1_246");//Hex 
            blackList.Add("EX1_575");//Mana Tide Totem
            blackList.Add("EX1_539");//Kill Command
            blackList.Add("CS2_203");//Ironbeak Owl

            blackList.Add("EX1_294");//Mirror entity

            if (opponentClass != CClass.WARLOCK)
                blackList.Add("EX1_238");//Lightning Bolt

            blackList.Add("EX1_565");//Flametongue Totem
            #endregion rawBlacklist
        }

    }
}
