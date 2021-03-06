﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HearthstoneMulligan.HeroInstances;
using HearthstoneMulligan.Specific_Mulligans;
using SmartBot.Database;
using SmartBot.Plugins.API;

namespace HearthstoneMulligan
{
    class DllMain
    {
        private static Card.Cards BoardToMulliganCard(CardTemplate boardCard)
        {
            return CardTemplate.StringToCard(boardCard.Id.ToString());
        }

        public static List<Card.Cards> HandleMulligan(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            InitMulligan(choices, opponentClass, ownClass);

            Task t_LoadGeneralBlackListEntries = Task.Run(() =>
            {
                LoadGeneralBlackListEntries(MainLists.OpponentClass, MainLists.OwnClass);
            });

            Task t_LoadOwnBlackListEntries = Task.Run(() =>
            {
                LoadOwnBlackListEntries();
            });

            t_LoadGeneralBlackListEntries.Wait();
            t_LoadOwnBlackListEntries.Wait();

            var detectedDeckType = DeckTypeDetector.SearchForDeckType();
            MainLists.CurrentDeckType = detectedDeckType;
            if (detectedDeckType == DeckTypeDetector.DeckType.UNKNOWN)
            {
                CreateDefaultLists();
            }
            else //particular deck type
            {
                MainLists.MaxManaCost = 3;
                DeckTypeDetector.GenerateWhiteAndBlackListForSpecialDeck(detectedDeckType);
            }

            CalculateMulligan(MainLists.MaxManaCost);
            foreach (Task task in neutralMinionTaskList)
            {
                task.Wait();
            }

            //DeckCalc
            CalculateDeckProbabilities();

            if (ValueReader.IsCoachModeEnabled &&
                File.Exists(Environment.CurrentDirectory + @"\De.TorstenMandelkow.MetroChart.dll"))
            {
                Thread t = new Thread(delegate ()
                {
                    var cWind = new USER_GUI.CoachMode.CoachWindow
                    {
                        Title = $"Mulligan Coach for {MainLists.CurrentDeckType.ToString()}"
                    };
                    cWind.ShowDialog();
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }

            return MainLists.chosenCards;
        }

        private static void InitMulligan(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            List<CardTemplate> choices_BoardCards =
                            choices.Select(x => new BoardCard(x).ResultingBoardCard).ToList();
            MainLists.HandCards_BoardCards = choices_BoardCards;
            MainLists.HandCards_native = choices;

            try
            {
                MainLists.currentDeck = Bot.CurrentDeck();
            }
            catch //MulliganTester.exe doesnt recognize deck :(
            {
                MainLists.currentDeck = new Deck();
            }
            AutoUpdateInit.CheckUpdate();

            MainLists.chosenCards = new List<Card.Cards>();
            MainLists.whiteList = new List<string>();
            MainLists.blackList = new List<string>();
            MainLists.OwnClass = ownClass;
            MainLists.OpponentClass = opponentClass;
            MainLists.CurrentDeckType = DeckTypeDetector.DeckType.UNKNOWN;
            //BlackListEntries could get whitelisted if good combo => prefer whitelist > blacklist

            //3+ mana neutral minions are origanizated by the value cuz not listed anywhere
            int MaxManaCost = ValueReader.MaxManaCost;
            if ((ownClass == Card.CClass.HUNTER || ownClass == Card.CClass.WARLOCK) &&
            Bot.CurrentMode() != Bot.Mode.Arena &&
            Bot.CurrentMode() != Bot.Mode.ArenaAuto)
            {
                MaxManaCost = ValueReader.MaxManaCostWarlockAndHunter;
            }
            MainLists.MaxManaCost = MaxManaCost;
        }

        /// <summary>
        /// If no particular deck type is detected then continue creating the white/blacklists default style
        /// </summary>
        private static void CreateDefaultLists()
        {
            Task t_LoadGeneralWhiteListEntries = Task.Run(() =>
            {
                LoadGeneralWhiteListEntriesForNeutralMinions();
            });

            Task t_Load_TGT_ListEntries = Task.Run(() =>
            {
                TGT.SetTGT_WhiteAndBlackList();
            });

            /*load obvious whitelist first*/
            t_LoadGeneralWhiteListEntries.Wait();
            t_Load_TGT_ListEntries.Wait();

            #region ListManaging
            switch (MainLists.OpponentClass)
            {
                case Card.CClass.DRUID:
                    MainLists.whiteList.Add("EX1_591");//Auchenai Priest
                    MainLists.whiteList.Add("EX1_339");//Thoughtsteal
                    MainLists.whiteList.Add("EX1_621");//Circle of Healing
                    break;
                case Card.CClass.WARRIOR:
                    MainLists.whiteList.Add("EX1_591");//Auchenai Priest
                    MainLists.whiteList.Add("EX1_339");//Thoughtsteal
                    MainLists.whiteList.Add("EX1_621");//Circle of Healing
                    MainLists.whiteList.Add("CS2_179");//Sen'jin Shieldmasta
                    break;
                case Card.CClass.ROGUE:
                    MainLists.whiteList.Add("EX1_591");//Auchenai Priest
                    MainLists.whiteList.Add("EX1_621");//Circle of Healing
                    MainLists.whiteList.Add("EX1_339");//Thoughtsteal
                    MainLists.whiteList.Add("FP1_030");//Loatheb
                    MainLists.whiteList.Add("CS2_179");//Sen'jin Shieldmasta
                    break;
                case Card.CClass.PRIEST:
                    MainLists.whiteList.Add("EX1_591");//Auchenai Priest
                    MainLists.whiteList.Add("EX1_339");//Thoughtsteal
                    MainLists.whiteList.Add("EX1_621");//Circle of Healing
                    MainLists.whiteList.Add("FP1_030");//Loatheb
                    break;
            }

            switch (MainLists.OwnClass)
            {
                case Card.CClass.DRUID:
                    MainLists.blackList.Add("CS2_012");//Swipe
                    if (MainLists.HandCards_BoardCards.Count(x => x.Type == Card.CType.MINION) >= 2)
                        MainLists.blackList.Add("CS2_011");//Savage Roar
                    MainLists.whiteList.Add("EX1_169");//Innervate
                    MainLists.whiteList.Add("EX1_154");//Wrath
                    MainLists.whiteList.Add("CS2_005");//Claw

                    if (MainLists.HandCards_BoardCards.Any(x => x.Id.ToString() == "EX1_169" /*already in whitelist*/
                        || x.Id.ToString() == "CS2_013"))
                    {
                        MainLists.whiteList.Add("FP1_005");//Shade of Naxxramas
                        MainLists.whiteList.Add("CS2_013");//Wild Growth
                        Druid.rampStructure = true;
                    }
                    if (Druid.rampStructure)
                    {
                        MainLists.whiteList.Add("EX1_085");//Mind Control Tech
                        MainLists.whiteList.Add("GVG_096");//Piloted Shredder
                    }

                    if (MainLists.HandCards_BoardCards.Any(x => x.Id.ToString() == "CS2_005" /*already in whitelist*/))//Claw
                    {
                        MainLists.whiteList.Add("EX1_578"); //Savagery
                    }
                    else
                        MainLists.blackList.Add("EX1_578"); //Savagery
                    break;
                case Card.CClass.HUNTER:
                    MainLists.whiteList.Add("NEW1_031");//Animal Companion
                    MainLists.whiteList.Add("EX1_617");//Deadly Shot
                    MainLists.whiteList.Add("GVG_043");//Glaivezooka
                    MainLists.whiteList.Add("GVG_087");//Steamwheedle Sniper
                    MainLists.whiteList.Add("BRM_013");//Quick Shot
                    MainLists.whiteList.Add("FP1_011");//Webspinner
                    MainLists.whiteList.Add("DS1_184");//Tracking
                    MainLists.whiteList.Add("DS1_185");//Arcane Shot
                    MainLists.blackList.Add("GVG_026");//Feign Death
                    MainLists.blackList.Add("EX1_544");//Flare

                    if (MainLists.HandCards_BoardCards.Count(x => x.Type == Card.CType.MINION
                                                          && !CardEffects.HasBadEffect(x) && x.Cost <= 3) >= 2)
                    {
                        MainLists.whiteList.Add("EX1_611"); //Freezing Trap
                        MainLists.whiteList.Add(MainLists.HandCards_BoardCards.OrderBy(x => new NeutralMinion(x).thisCardValue).
                            First(x => x.Type == Card.CType.MINION
                                && !CardEffects.HasBadEffect(x) && x.Cost <= 3).Id.ToString());
                    }
                    else //better get minions
                        MainLists.blackList.Add("EX1_611"); //Freezing Trap                                        

                    if (MainLists.HandCards_BoardCards.Count(x => x.Type == Card.CType.MINION
                        && x.Race == Card.CRace.BEAST &&
                        !MainLists.blackList.Contains(x.Id.ToString())) >= 3)
                        MainLists.whiteList.Add("DS1_175");//Timber Wolf
                    else
                        MainLists.blackList.Add("DS1_175");//Timber Wolf

                    if (MainLists.HandCards_BoardCards.Count > 3)
                        MainLists.whiteList.Add("EX1_014");//Mukla
                    break;
                case Card.CClass.MAGE:
                    MainLists.whiteList.Add("EX1_608");//Sorcerer's Apprentice
                    MainLists.whiteList.Add("GVG_002");//Snowchugger
                    MainLists.whiteList.Add("CS2_024");//Frostbolt
                    MainLists.whiteList.Add("GVG_001");//Flamecannon
                    MainLists.whiteList.Add("CS2_mirror");//Mirror Image
                    MainLists.whiteList.Add("NEW1_012");//Mana Wyrm
                    MainLists.whiteList.Add("EX1_277");//Arcane Missiles

                    if (Mage.IsHoldingAllSecretsInHand(MainLists.HandCards_BoardCards) && MainLists.HandCards_BoardCards.
                        Any(x => x.Id.ToString() == "FP1_004"))//Mad Scientist
                    {
                        CardTemplate mostExpensiveHandSecret =
                            MainLists.HandCards_BoardCards.Where(x => x.IsSecret).OrderBy(x => x.Cost).Last();

                        MainLists.whiteList.Add("FP1_004");//Mad Scientist
                        MainLists.blackList.Add(mostExpensiveHandSecret.Id.ToString());
                    }
                    MainLists.whiteList.Add("FP1_004");//Mad Scientist
                    break;
                case Card.CClass.PALADIN:
                    MainLists.whiteList.Add("EX1_366");//Sword of Justice
                    MainLists.whiteList.Add("GVG_061");//Muster for Battle
                    MainLists.whiteList.Add("EX1_382");//Aldor Peacekeeper
                    MainLists.whiteList.Add("GVG_058");//Shielded Minibot
                    MainLists.whiteList.Add("EX1_362");//Argent Protector
                    MainLists.whiteList.Add("EX1_130");//Noble Sacrifice
                    MainLists.whiteList.Add("CS2_091");//Light's Justice
                    MainLists.whiteList.Add("EX1_363");//Blessing of Wisdom
                    MainLists.whiteList.Add("CS2_087");//Blessing of Might
                    MainLists.whiteList.Add("EX1_136");//Redemption
                    MainLists.whiteList.Add("FP1_020");//Avenges

                    if (MainLists.OpponentClass == Card.CClass.DRUID || MainLists.OpponentClass == Card.CClass.WARLOCK)
                        MainLists.whiteList.Add("GVG_101");//Scarlet Purifier
                    break;
                case Card.CClass.PRIEST:
                    MainLists.whiteList.Add("EX1_339");//Thoughtsteal
                    MainLists.whiteList.Add("GVG_072");//Shadowboxer
                    MainLists.whiteList.Add("CS2_234");//Shadow Word: Pain
                    MainLists.whiteList.Add("GVG_009");//Shadowbomber
                    MainLists.whiteList.Add("CS2_004");//Power Word: Shield
                    MainLists.whiteList.Add("CS2_235");//Northshire Cleric
                    MainLists.whiteList.Add("EX1_332");//Silence
                    MainLists.whiteList.Add("CS1_130");//Holy Smite
                    MainLists.whiteList.Add("CS1_129");//Inner Fire
                    MainLists.whiteList.Add("CS2_181");//Injured Blademaster

                    if (MainLists.HandCards_BoardCards.Any(x => x.Type == Card.CType.MINION
                        && x.Cost <= 3 && !MainLists.blackList.Contains(x.Id.ToString())))
                        MainLists.whiteList.Add("CS2_236");//Divine Spirit
                    else
                        MainLists.blackList.Add("CS2_236");//Divine Spirit
                    if (MainLists.HandCards_BoardCards.Any(x => x.Id.ToString() == "CS2_181"))
                        MainLists.whiteList.Add("EX1_621"); // Circle of Healing
                    else
                        MainLists.blackList.Add("EX1_621");

                    if (MainLists.HandCards_BoardCards.Count > 3)
                    {
                        if (MainLists.HandCards_BoardCards.Any(x => x.Id.ToString() == "CS2_235"))//Northshire Cleric
                            MainLists.whiteList.Add("CS2_004");//Power Word: Shield
                        if (MainLists.HandCards_BoardCards.Any(x => x.Id.ToString() == "CS2_181"))//Injured Blademaster	
                            MainLists.whiteList.Add("EX1_621");//Circle of Healing
                    }

                    if (MainLists.OpponentClass == Card.CClass.WARRIOR || MainLists.OpponentClass == Card.CClass.PALADIN)
                        MainLists.whiteList.Add("EX1_588");
                    else
                        MainLists.blackList.Add("EX1_588");

                    if (MainLists.HandCards_BoardCards.Count(c => c.Id.ToString() == "FP1_001" || c.Id.ToString() == "CS2_235" /*already in whitelist*/
                        || c.Id.ToString() == "GVG_081" /*already in whitelist*/ ) > 1)
                    {
                        MainLists.whiteList.Add("FP1_001");
                        MainLists.whiteList.Add("CS2_004");//Power Word: Shield
                        MainLists.whiteList.Add("GVG_010");//Velen's Chosen    
                        MainLists.whiteList.Add("FP1_009");//Deathlord  
                    }
                    break;
                case Card.CClass.ROGUE:
                    MainLists.whiteList.Add("GVG_023"); //Goblinbarbier-o-Mat
                    MainLists.whiteList.Add("EX1_522"); //Geduldiger Attentäter
                    MainLists.whiteList.Add("EX1_124"); //Ausweiden
                    MainLists.whiteList.Add("CS2_074"); //Tödliches Gift
                    MainLists.whiteList.Add("CS2_073"); //Kaltblütigkeit
                    MainLists.whiteList.Add("CS2_072"); //Meucheln
                    MainLists.whiteList.Add("CS2_075"); //Finsterer Stoß
                    MainLists.whiteList.Add("EX1_145"); //Vorbereitung
                    MainLists.whiteList.Add("EX1_131");//Defias Ringleader
                    MainLists.whiteList.Add("EX1_129"); //Dolchfächer
                    MainLists.whiteList.Add("EX1_126"); //Verrat

                    if (MainLists.HandCards_BoardCards.Any(x => x.Cost <= 1
                                                        &&
                                                        (x.Type == Card.CType.SPELL || x.Type == Card.CType.WEAPON)
                                                        &&
                                                        !CardEffects.HasBadEffect(x)) &&
                        MainLists.HandCards_BoardCards.All(x => x.Id.ToString() != "EX1_131")) //got no Defias Ringleader
                    {
                        MainLists.whiteList.Add("EX1_134"); //SI:7-Agent
                        var theOtherCard = MainLists.HandCards_BoardCards.First(x => x.Cost <= 1
                                                                             &&
                                                                             (x.Type == Card.CType.SPELL ||
                                                                              x.Type == Card.CType.WEAPON)
                                                                             &&
                                                                             !CardEffects.HasBadEffect(x));
                        MainLists.whiteList.Add(theOtherCard.Id.ToString());
                    }
                    //dont force adding to blacklist
                    else if (MainLists.HandCards_BoardCards.Any(x => x.Cost <= 1
                                                             &&
                                                             (x.Type == Card.CType.SPELL || x.Type == Card.CType.WEAPON)
                                                             &&
                                                             !CardEffects.HasBadEffect(x)) &&
                             MainLists.HandCards_BoardCards.Any(x => x.Id.ToString() == "EX1_131")) //got Defias Ringleader
                    {
                        var theOtherCard = MainLists.HandCards_BoardCards.First(x => x.Cost <= 1
                                                                             &&
                                                                             (x.Type == Card.CType.SPELL ||
                                                                              x.Type == Card.CType.WEAPON)
                                                                             &&
                                                                             !CardEffects.HasBadEffect(x));
                        MainLists.whiteList.Add(theOtherCard.Id.ToString());
                    }

                    if (MainLists.HandCards_BoardCards.Any(x => x.Cost <= 1 && (x.Type == Card.CType.SPELL
                        || x.Type == Card.CType.WEAPON)))
                        MainLists.whiteList.Add("EX1_131"); //Rädelsführer der Defias
                    break;
                case Card.CClass.SHAMAN:
                    MainLists.whiteList.Add("EX1_248"); //Wildgeist
                    MainLists.whiteList.Add("EX1_575"); //Manafluttotem
                    MainLists.whiteList.Add("EX1_259"); //Gewittersturm
                    MainLists.whiteList.Add("EX1_258"); //Entfesselter Elementar
                    MainLists.whiteList.Add("GVG_037"); //Wirbelnder Zapp-o-Mat
                    MainLists.whiteList.Add("CS2_039"); //Windzorn
                    MainLists.whiteList.Add("EX1_247");//Stormforged Axe
                    MainLists.whiteList.Add("FP1_025");//Reincarnate
                    MainLists.whiteList.Add("EX1_565");//Flametongue Totem
                    MainLists.whiteList.Add("GVG_038");//Crackle
                    MainLists.whiteList.Add("CS2_045");//Rockbiter Weapon
                    MainLists.whiteList.Add("EX1_238");//Lightning Bolt
                    MainLists.whiteList.Add("CS2_037");//Frost Shock
                    MainLists.whiteList.Add("EX1_251");//Forked Lightning
                    MainLists.whiteList.Add("EX1_245");//Earth Shock
                    MainLists.whiteList.Add("EX1_243");//Dust Devil
                    MainLists.whiteList.Add("EX1_244");//Totemic Might
                    MainLists.whiteList.Add("CS2_041");//Ancestral Healing
                    break;
                case Card.CClass.WARLOCK:
                    MainLists.whiteList.Add("BRM_005");//Demonwrath
                    MainLists.whiteList.Add("CS2_065");//Voidwalker
                    MainLists.whiteList.Add("EX1_306");//Succubus
                    MainLists.whiteList.Add("EX1_596");//Demonfire
                    MainLists.whiteList.Add("GVG_015");//Darkbomb
                    MainLists.whiteList.Add("EX1_302");//Mortal Coil
                    MainLists.whiteList.Add("EX1_319");//Flame Imp
                    MainLists.whiteList.Add("CS2_059");//Blood Imp

                    if (ValueReader.BlackList.AddSoulFire)
                        MainLists.blackList.Add("EX1_308");//Soulfire
                    else
                        MainLists.whiteList.Add("EX1_308");//Soulfire
                    break;
                case Card.CClass.WARRIOR:
                    MainLists.whiteList.Add("EX1_604");//Frothing Berserker

                    if (MainLists.HandCards_BoardCards.Any(x => x.Type == Card.CType.MINION && !CardEffects.HasBadEffect(x) &&
                                                        x.Cost <= 3))
                    {
                        MainLists.whiteList.Add("EX1_402"); //Armorsmith
                        MainLists.blackList.Add(MainLists.HandCards_BoardCards.OrderBy(x => new NeutralMinion(x).thisCardValue).
                            First(x => x.Type == Card.CType.MINION &&
                            !CardEffects.HasBadEffect(x) &&
                                    x.Cost <= 3).Id.ToString());
                    }
                    else
                        MainLists.blackList.Add("EX1_402"); //Armorsmith

                    //INNER RAGE
                    if (MainLists.HandCards_BoardCards.Any(x => x.Type == Card.CType.MINION &&
                        x.Health > 1 && !CardEffects.HasBadEffect(x) && x.Cost <= 3))
                        MainLists.whiteList.Add("CS2_104");//Rampage
                    if (MainLists.HandCards_BoardCards.Any(x => x.Id.ToString() == "EX1_607")) //Inner Rage
                    {
                        if (MainLists.HandCards_BoardCards.Any(x => x.Id.ToString() == "EX1_007" || x.Id.ToString() == "EX1_393" || x.Id.ToString() ==
                            "BRM_019" || x.Id.ToString() == "EX1_412"))
                        {
                            MainLists.whiteList.Add("EX1_007"); //Acolyte
                            MainLists.whiteList.Add("EX1_393"); //Amani Berserker
                            MainLists.whiteList.Add("BRM_019");//Grim Patron
                            MainLists.whiteList.Add("EX1_412");//Raging Worgen
                            MainLists.whiteList.Add("EX1_607"); //Inner Rage
                        }
                        else
                        {
                            MainLists.blackList.Add("EX1_607");
                        }
                    }

                    //WHIRLWIND
                    if (MainLists.HandCards_BoardCards.Any(x => x.Id.ToString() == "EX1_400")) //Whirlwind
                    {
                        if (MainLists.HandCards_BoardCards.Any(x => x.Id.ToString() == "EX1_007" || x.Id.ToString() == "EX1_393" || x.Id.ToString() ==
                            "BRM_019" || x.Id.ToString() == "EX1_412"))
                        {
                            MainLists.whiteList.Add("EX1_007"); //Acolyte
                            MainLists.whiteList.Add("EX1_393"); //Amani Berserker
                            MainLists.whiteList.Add("BRM_019");//Grim Patron
                            MainLists.whiteList.Add("EX1_412");//Raging Worgen
                            MainLists.whiteList.Add("EX1_400"); //Whirlwind
                        }
                        else
                        {
                            MainLists.blackList.Add("EX1_400"); //Whirlwind
                        }
                    }

                    float averageMana = MainLists.HandCards_native.Aggregate<Card.Cards, float>(0,
                        (current, card) => current + new BoardCard(card).ResultingBoardCard.Cost);
                    averageMana = averageMana / MainLists.HandCards_BoardCards.Count;
                    if (averageMana < 4f)
                        MainLists.whiteList.Add("GVG_050");//Bouncing Blade

                    switch (MainLists.OpponentClass)
                    {
                        case Card.CClass.WARLOCK:
                            MainLists.whiteList.Add("EX1_402");//Armorsmith
                            MainLists.whiteList.Add("FP1_021");//Death's Bite
                            break;
                        case Card.CClass.SHAMAN:
                            MainLists.whiteList.Add("EX1_402");//Armorsmith
                            MainLists.whiteList.Add("FP1_021");//Death's Bite
                            break;
                        case Card.CClass.PRIEST:
                            MainLists.whiteList.Add("EX1_402");//Armorsmith
                            MainLists.whiteList.Add("EX1_606");//Shield Block
                            MainLists.whiteList.Add("EX1_410");//Shield Slam
                            MainLists.whiteList.Add("EX1_007");//Acolyte of Pain
                            MainLists.whiteList.Add("FP1_021");//Death's Bite
                            break;
                        case Card.CClass.ROGUE:
                            MainLists.whiteList.Add("EX1_606");//Shield Block
                            MainLists.whiteList.Add("EX1_410");//Shield Slam	
                            MainLists.whiteList.Add("FP1_021");//Death's Bite
                            break;
                    }

                    MainLists.whiteList.Add("EX1_391");//Slam
                    MainLists.whiteList.Add("CS2_105");//Heroic Strike
                    MainLists.whiteList.Add("CS2_106");//Fiery War Axe
                    MainLists.whiteList.Add("EX1_603");//Cruel Taskmaster
                    MainLists.whiteList.Add("NEW1_036");//Commanding Shout
                    MainLists.whiteList.Add("CS2_114");//Cleave
                    MainLists.whiteList.Add("GVG_051");//Warbot
                    if (MainLists.HandCards_BoardCards.Any(x => x.Type == Card.CType.WEAPON))
                    {
                        MainLists.whiteList.Add("EX1_409"); //Upgrade!
                        MainLists.whiteList.Add(MainLists.HandCards_BoardCards.OrderBy(x => x.Cost).
                            First(x => x.Type == Card.CType.WEAPON).Id.ToString());
                    }
                    MainLists.whiteList.Add("EX1_410");//Shield Slam
                    MainLists.whiteList.Add("CS2_108");//Execute
                    break;

            }
            #endregion ListManaging
        }

        private static void CalculateDeckProbabilities()
        {
            try
            {
                int normalMinionCount = MainLists.HandCards_BoardCards.Count(x => new NeutralMinion(x).minionBoardCard != null &&
                    NeutralMinion.WouldTakeMinion(x) &&  !MainLists.whiteList.Contains(x.Id.ToString()));

                if (normalMinionCount == 0)
                    return;

                float minProbabilityToReplace = ValueReader.MinProbabilityToReplace;

                ProbabilityCalculation probabilityCalculation = new ProbabilityCalculation();
                Tuple<int, float> treeResult = probabilityCalculation.GenerateTreeWhiteListDraw();

                //less medium cards than actually having possible
                if (treeResult.Item1 /*prospective refused*/< normalMinionCount)
                {
                    int deltaReplaceCount = normalMinionCount - treeResult.Item1;

                    int i = 1;
                    foreach (CardTemplate badCard in MainLists.HandCards_BoardCards.
                        Where(x => new NeutralMinion(x).minionBoardCard != null && NeutralMinion.WouldTakeMinion(x) &&
                                   !MainLists.whiteList.Contains(x.Id.ToString())).
                        OrderBy(x => new NeutralMinion(x).thisCardValue).TakeWhile(x => i <= deltaReplaceCount))
                    {
                        MainLists.chosenCards.Remove(BoardToMulliganCard(badCard));
                        ++i;
                    }
                }
            }
            catch
            {
                //Ignored
            }
        }

        /// <summary>
        /// Additionally organizates neutral minions with the whitelist instead of only the value
        /// 2 Mana or below
        /// </summary>
        private static void LoadGeneralWhiteListEntriesForNeutralMinions()
        {
            List<CardTemplate> HandCards_BoardCards = MainLists.HandCards_BoardCards;

            //Chromatic Dragonkin
            //Lance Bearer
            //Chromatic Prototype
            //Deathcharger

            MainLists.whiteList.Add("EX1_066");//Acidic Swamp Ooze
            MainLists.whiteList.Add("EX1_393");//Amani Berserker
            MainLists.whiteList.Add("GVG_085");//Annoy-o-Tron
            MainLists.whiteList.Add("AT_109");//Argent Watchman
            MainLists.whiteList.Add("CS2_172");//Bloodfen Raptor
            MainLists.whiteList.Add("EX1_012");//Bloodmage Thalnos
            MainLists.whiteList.Add("NEW1_018");//Bloodsail Raider

            if (Murloc.IsMurlocDeck)
            {
                MainLists.blackList.Add("CS2_173"); //Bluegill Warrior
            }
            else
            {
                MainLists.whiteList.Add("CS2_173"); //Bluegill Warrior
            }

            if (HandCards_BoardCards.Count(x => x.Type == Card.CType.MINION && x.Id.ToString() != "EX1_162" && x.Cost <= 2) >= 2)
            {
                MainLists.whiteList.Add("EX1_162"); //Dire Wolf Alpha
                foreach (var minion in HandCards_BoardCards.Where(x => x.Type == Card.CType.MINION && x.Cost <= 2 &&
                    !CardEffects.HasBadEffect(x)))
                {
                    MainLists.whiteList.Add(minion.Id.ToString());
                }
            }
            else
                MainLists.blackList.Add("EX1_162"); //Dire Wolf Alpha

            if (HandCards_BoardCards.Any(x => x.Id.ToString() == "CS2_026")) //Frost Nova
            {
                MainLists.whiteList.Add("NEW1_021"); //Doomsayer
                MainLists.whiteList.Add("CS2_026");//Frost Nova
            }
            else
                MainLists.blackList.Add("NEW1_021"); //Doomsayer


            MainLists.whiteList.Add("NEW1_016");//Captain's Parrot
            MainLists.whiteList.Add("NEW1_023");//Faerie Dragon
            MainLists.whiteList.Add("CS2_121");//Frostwolf Grunt
            MainLists.whiteList.Add("GVG_081");//Gilblin Stalker
            MainLists.whiteList.Add("FP1_002");//Haunted Creeper
            MainLists.whiteList.Add("NEW1_019");//Knife Juggler
            MainLists.whiteList.Add("CS2_142");//Kobold Geomancer
            MainLists.whiteList.Add("EX1_096");//Loot Hoarder
            MainLists.whiteList.Add("EX1_100");//Lorewalker Cho
            MainLists.whiteList.Add("EX1_082");//Mad Bomber

            if (HandCards_BoardCards.Any(x => x.Type == Card.CType.SPELL && x.Cost <= 2 && !CardEffects.HasBadEffect(x)))
            {
                MainLists.whiteList.Add("EX1_055"); //Mana Addict
                foreach (var spell in HandCards_BoardCards.Where(x => x.Type == Card.CType.SPELL && x.Cost <= 2 
                    && !CardEffects.HasBadEffect(x)))
                {
                    MainLists.whiteList.Add(spell.Id.ToString());
                }
            }
            else
                MainLists.blackList.Add("EX1_055"); //Mana Addict

            if (HandCards_BoardCards.Any(x => x.Type == Card.CType.MINION && x.Cost <= 3 && !CardEffects.HasBadEffect(x)))
            {
                MainLists.whiteList.Add("NEW1_037"); //Master Swordsmith
                MainLists.whiteList.Add(HandCards_BoardCards.OrderBy(x => new NeutralMinion(x).thisCardValue).First(x =>
                    x.Type == Card.CType.MINION && x.Cost <= 3 && !CardEffects.HasBadEffect(x)).Id.ToString());
            }
            else
                MainLists.blackList.Add("NEW1_037"); //Master Swordsmith

            MainLists.whiteList.Add("GVG_006");//Mechwarper
            MainLists.whiteList.Add("EX1_506");//Murloc Tidehunter
            MainLists.whiteList.Add("EX1_557");//Nat Pagle

            if (HandCards_BoardCards.Any(x => x.Id.ToString() == "EX1_093" || x.Id.ToString() == "EX1_058" 
                || x.Id.ToString() == "GVG_076"))
                //Defender of Argus 
                //Sunfury Protector 
                //Explosive Sheep
            {
                MainLists.whiteList.Add("FP1_007"); //Nerubian Egg
                bool containsArgus = HandCards_BoardCards.Any(x => x.Id.ToString() == "EX1_093");
                bool containsProtector = HandCards_BoardCards.Any(x => x.Id.ToString() == "EX1_058");

                if (containsArgus)
                    MainLists.whiteList.Add(HandCards_BoardCards.First(x => x.Id.ToString() == "EX1_093").
                        Id.ToString());
                else if (containsProtector)
                    MainLists.whiteList.Add(HandCards_BoardCards.First(x => x.Id.ToString() == "EX1_058").
                        Id.ToString());
                else //must contain the sheep
                    MainLists.whiteList.Add(HandCards_BoardCards.First(x => x.Id.ToString() == "GVG_076").
                        Id.ToString());
            }
            else
                MainLists.blackList.Add("FP1_007"); //Nerubian Egg

            MainLists.whiteList.Add("EX1_015");//Novice Engineer
            MainLists.whiteList.Add("EX1_076");//Pint-Sized Summoner
            MainLists.whiteList.Add("GVG_064");//Puddlestomper
            MainLists.whiteList.Add("CS2_120");//River Crocolisk
            MainLists.whiteList.Add("GVG_075");//Ship's Cannon
            MainLists.whiteList.Add("GVG_067");//Stonesplinter Trogg
            MainLists.whiteList.Add("EX1_058");//Sunfury Protector
            MainLists.whiteList.Add("FP1_024");//Unstable Ghoul

            MainLists.whiteList.Add("NEW1_020");//Wild Pyromancer

            if (HandCards_BoardCards.Any(x => x.Type == Card.CType.MINION && x.Cost <= 3 && !CardEffects.HasBadEffect(x)))
            {
                MainLists.whiteList.Add("CS2_188"); //Abusive Sergeant
                MainLists.whiteList.Add(HandCards_BoardCards.OrderBy(x => new NeutralMinion(x).thisCardValue).First(x => 
                    x.Type == Card.CType.MINION && x.Cost <= 3 && !CardEffects.HasBadEffect(x)).Id.ToString());               
            }
            //dont force adding to blacklist => calculate minion value later

            MainLists.whiteList.Add("EX1_008");//Argent Squire
            MainLists.whiteList.Add("GVG_082");//Clockwork Gnome

            if (HandCards_BoardCards.Any(x => x.Race == Card.CRace.MECH && x.Cost <= 3 && !CardEffects.HasBadEffect(x)))
            {
                MainLists.whiteList.Add("GVG_013"); //Cogmaster
                MainLists.whiteList.Add(HandCards_BoardCards.OrderBy(x => new NeutralMinion(x).thisCardValue).First(
                    x => x.Race == Card.CRace.MECH && 
                x.Cost <= 3 && !CardEffects.HasBadEffect(x)).Id.ToString());
            }
            else
                MainLists.blackList.Add("GVG_013"); //Cogmaster

            MainLists.whiteList.Add("CS2_189");//Elven Archer

            if (Murloc.IsMurlocDeck)
                MainLists.whiteList.Add("EX1_508");//Grimscale Oracle
            //dont force blacklist
            MainLists.whiteList.Add("EX1_508");//Grimscale Oracle

            MainLists.whiteList.Add("NEW1_017");//Hungry Crab
            MainLists.whiteList.Add("EX1_029");//Leper Gnome
            MainLists.whiteList.Add("EX1_001");//Lightwarden
            MainLists.whiteList.Add("AT_082");//Lowly Squire
            MainLists.whiteList.Add("EX1_080");//Secretkeeper

            if (!Murloc.IsMurlocDeck)
            {
                MainLists.whiteList.Add("CS2_168"); //Murloc Raider
                MainLists.blackList.Add("EX1_509");//Murloc Tidecaller
            }
            else
            {
                MainLists.blackList.Add("CS2_168"); //Murloc Raider
                MainLists.whiteList.Add("EX1_509");//Murloc Tidecaller
            }
            MainLists.whiteList.Add("CS2_168"); //Murloc Raider
            MainLists.whiteList.Add("EX1_509");//Murloc Tidecaller

            MainLists.whiteList.Add("EX1_405");//Shieldbearer
            MainLists.whiteList.Add("CS2_146");//Southsea Deckhand
            MainLists.whiteList.Add("CS2_171");//Stonetusk Boar
            MainLists.whiteList.Add("FP1_028");//Undertaker
            MainLists.whiteList.Add("EX1_011");//Voodoo Doctor
            MainLists.whiteList.Add("EX1_010");//Worgen Infiltrator
            MainLists.whiteList.Add("EX1_004");//Young Priestess
            MainLists.whiteList.Add("FP1_001");//Zombie Chow
            MainLists.whiteList.Add("GVG_093");//Target Dummy
            MainLists.whiteList.Add("EX1_301");//Felguard
        }

        private static void LoadOwnBlackListEntries()
        {
            foreach (var ownBlackListEntry in ValueReader.GetOwnBlackListEntries())
            {
                MainLists.blackList.Add(ownBlackListEntry);
            }
            if (ValueReader.BlackList.AddMillhouseManastorm)
                MainLists.blackList.Add("NEW1_029");//Millhouse
        }

        static List<Task> neutralMinionTaskList = new List<Task>();
        private static void CalculateMulligan(int MaxManaCost)
        {
            List<CardTemplate> HandCards_BoardCards = MainLists.HandCards_BoardCards;

            ManageTwins();

            ManageCombos();

            /*====================================main comparison===========================================*/
            /*prefers whitelist over blacklist*/
            foreach (CardTemplate card in HandCards_BoardCards.Where(x => !MainLists.chosenCards.Contains(x.Id)))
            {
                if (MainLists.whiteList.Contains(card.Id.ToString()))
                    MainLists.chosenCards.Add(BoardToMulliganCard(card));
                else if (MainLists.blackList.Contains(card.Id.ToString()))
                    // ReSharper disable once RedundantJumpStatement
                    continue;
                else if (new NeutralMinion(card).minionBoardCard != null &&
                         card.Cost <= MaxManaCost)
                {
                    Task newT = new Task(() => NeutralMinion.ManageNeutralMinion(BoardToMulliganCard(card)));
                    neutralMinionTaskList.Add(newT);
                    newT.Start();
                }
            }
        }

        private static void ManageTwins()
        {
            var HandCards_BoardCards = MainLists.HandCards_BoardCards;

            for (int i = 0; i < HandCards_BoardCards.Count; i++)
            {
                for (int j = 0; j < HandCards_BoardCards.Count; j++)
                {
                    if (i != j && HandCards_BoardCards[i].Id.Equals(HandCards_BoardCards[j].Id))
                    {                
                        if (!ValueReader.AllowTwins ||
                            HandCards_BoardCards[i].Cost >= ValueReader.DontAllowTwinsIfManaCostAtLeast)
                        {
                            var toMulliganCard = BoardToMulliganCard(HandCards_BoardCards[i]);
                            if (!MainLists.chosenCards.Contains(toMulliganCard))
                            {
                                MainLists.chosenCards.Add(toMulliganCard);
                            }
                            MainLists.blackList.Add(HandCards_BoardCards[i].Id.ToString());
                        }
                    }
                }
            }
        }

        

        private static void ManageCombos()
        {
            #region comboChecks

            foreach (var priorityCase in ValueReader.ValueIgnorer.AllComboCasesSortedByPriority)
            {
                if (priorityCase.Key == ValueReader.ValueIgnorer.ComboCase1Priority)
                {
                    Combos.CaseOne();
                }
                else if (priorityCase.Key == ValueReader.ValueIgnorer.ComboCase2Priority)
                {
                    Combos.CaseTwo();
                }
                else if (priorityCase.Key == ValueReader.ValueIgnorer.ComboCase3Priority)
                {
                    Combos.CaseThree();
                }
            }

            #endregion comboChecks
        }

        private static void LoadGeneralBlackListEntries(Card.CClass opponentClass, Card.CClass myClass)
        {
            if (opponentClass != Card.CClass.WARRIOR && opponentClass != Card.CClass.HUNTER &&
                opponentClass != Card.CClass.ROGUE)
            {
                MainLists.blackList.Add("EX1_007");//Acolyte of Pain
            }
            if (opponentClass != Card.CClass.HUNTER)
                MainLists.blackList.Add("TU4d_001");//Hemet Nesingwary

            if (opponentClass != Card.CClass.WARLOCK)
                MainLists.blackList.Add("AT_106");//Light's Champion

            MainLists.blackList.Add("AT_115");//Fencing Coach
            MainLists.blackList.Add("EX1_304");//Void Terror
            MainLists.blackList.Add("FP1_025");//Reincarnate
            MainLists.blackList.Add("CS2_038");//Ancestral Spirit

            MainLists.blackList.Add("EX1_349");//Divine Favor
            MainLists.blackList.Add("CS2_023");//Arcane Intellect
            MainLists.blackList.Add("CS2_011");//Savage roar
            MainLists.blackList.Add("EX1_622");//Shadow Word Death
            MainLists.blackList.Add("EX1_625");//Shadow Form
            MainLists.blackList.Add("DS1_233");//Mind Blast
            MainLists.blackList.Add("CS2_108");//Execute
            MainLists.blackList.Add("EX1_391");//Slam
            MainLists.blackList.Add("EX1_005");//BGH
            MainLists.blackList.Add("CS2_007");//Healing Touch
            MainLists.blackList.Add("EX1_246");//Hex 
            MainLists.blackList.Add("EX1_575");//Mana Tide Totem
            MainLists.blackList.Add("EX1_539");//Kill Command
            MainLists.blackList.Add("CS2_203");//Ironbeak Owl

            MainLists.blackList.Add("CS2_118");//Magma Rager
            MainLists.blackList.Add("EX1_132"); //Eye for an Eye
            MainLists.blackList.Add("CS2_231"); //Wisp

            MainLists.blackList.Add("EX1_294");//Mirror entity

            if (opponentClass != Card.CClass.WARLOCK)
                MainLists.blackList.Add("EX1_238");//Lightning Bolt

            MainLists.blackList.Add("EX1_565");//Flametongue Totem
            MainLists.blackList.Add("EX1_059");//Crazed Alchemist
            MainLists.blackList.Add("FP1_003");//Echoing Ooze
            MainLists.blackList.Add("GVG_108");//Recombobulator
            MainLists.blackList.Add("EX1_049");//Youthful Brewmaster
            MainLists.blackList.Add("NEW1_025");//Bloodsail Corsair
            MainLists.blackList.Add("CS2_169");//Young Dragonhawk
            MainLists.blackList.Add("EX1_408");//Mortal Strike
        }
    }
}
