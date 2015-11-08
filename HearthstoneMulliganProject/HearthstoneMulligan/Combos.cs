using System.Collections.Generic;
using System.Linq;
using SmartBot.Database;
using SmartBot.Plugins.API;

namespace HearthstoneMulligan
{
    class Combos
    {
        private static Card.Cards BoardToMulliganCard(CardTemplate boardCard)
        {
            return CardTemplate.StringToCard(boardCard.Id.ToString());
        }

        private static bool alreadyFoundOneCombo { get; set; }

        public static void CaseThree()
        {
            List<CardTemplate> HandCards_BoardCards = MainLists.HandCards_BoardCards;

            if (ValueReader.ValueIgnorer.IgnoreValueIf_244_AndCoin &&
                ValueReader.ValueIgnorer.HandContains224(HandCards_BoardCards) &&
                !alreadyFoundOneCombo)
            {
                alreadyFoundOneCombo = true;
                var best2Drop =
                        HandCards_BoardCards.Where(x => new NeutralMinion(x).minionBoardCard != null && x.Cost == 2).
                            OrderBy(x => new NeutralMinion(x).thisCardValue).Last();
                var bestOther2Drop =
                        HandCards_BoardCards.Where(x => x.Id.ToString() != best2Drop.Id.ToString() &&
                            new NeutralMinion(x).minionBoardCard != null && x.Cost == 2).
                                OrderBy(x => new NeutralMinion(x).thisCardValue).Last();
                var best4Drop =
                        HandCards_BoardCards.Where(x => new NeutralMinion(x).minionBoardCard != null && x.Cost == 4).
                            OrderBy(x => new NeutralMinion(x).thisCardValue).Last();

                var bestDrops = new[] { best2Drop, bestOther2Drop, best4Drop };

                if (ValueReader.ValueIgnorer.OnlyAddIfNoBadEffect && !bestDrops.All(CardEffects.HasBadEffect))
                {
                    foreach (var drop in bestDrops.Where(x => !MainLists.chosenCards.Contains(BoardToMulliganCard(x))))
                    {
                        string dropAsString = drop.Id.ToString();
                        MainLists.chosenCards.Add(CardTemplate.StringToCard(dropAsString));
                        MainLists.blackList.Add(drop.Id.ToString());
                    }
                }
            }
        }

        public static void CaseTwo()
        {
            List<CardTemplate> HandCards_BoardCards = MainLists.HandCards_BoardCards;

            if (ValueReader.ValueIgnorer.IgnoreValueIf_2234_AndCoin &&
                ValueReader.ValueIgnorer.HandContains2234(HandCards_BoardCards) &&
                alreadyFoundOneCombo)
            {
                alreadyFoundOneCombo = true;
                var best2Drop =
                        HandCards_BoardCards.Where(x => new NeutralMinion(x).minionBoardCard != null && x.Cost == 2).
                            OrderBy(x => new NeutralMinion(x).thisCardValue).Last();
                var bestOther2Drop =
                        HandCards_BoardCards.Where(x => x.Id.ToString() != best2Drop.Id.ToString() &&
                            new NeutralMinion(x).minionBoardCard != null && x.Cost == 2).
                                OrderBy(x => new NeutralMinion(x).thisCardValue).Last();
                var best3Drop =
                        HandCards_BoardCards.Where(x => new NeutralMinion(x).minionBoardCard != null && x.Cost == 3).
                            OrderBy(x => new NeutralMinion(x).thisCardValue).Last();
                var best4Drop =
                        HandCards_BoardCards.Where(x => new NeutralMinion(x).minionBoardCard != null && x.Cost == 4).
                            OrderBy(x => new NeutralMinion(x).thisCardValue).Last();

                var bestDrops = new[] { best2Drop, bestOther2Drop, best3Drop, best4Drop };

                foreach (var drop in bestDrops.Where(x => !MainLists.chosenCards.Contains(BoardToMulliganCard(x))))
                {
                    string dropAsString = drop.Id.ToString();
                    MainLists.chosenCards.Add(CardTemplate.StringToCard(dropAsString));
                    MainLists.blackList.Add(drop.Id.ToString());
                }
            }
        }

        public static void CaseOne()
        {
            List<CardTemplate> HandCards_BoardCards = MainLists.HandCards_BoardCards;

            int X_Config_Drop = ValueReader.ValueIgnorer.GetXDrop;

            //card is X drop and hand contains "x - 1 drop" and "x - 2 drop"
            if (ValueReader.ValueIgnorer.IgnoreValueIfCardIsX_DropEtc
                &&
                HandCards_BoardCards.Any(x => new NeutralMinion(x).minionBoardCard != null && x.Cost == X_Config_Drop)
                &&
                HandCards_BoardCards.Any(x => new NeutralMinion(x).minionBoardCard != null && x.Cost == X_Config_Drop - 1)
                &&
                HandCards_BoardCards.Any(x => new NeutralMinion(x).minionBoardCard != null && x.Cost == X_Config_Drop - 2)
                && !Combos.alreadyFoundOneCombo)
            {
                Combos.alreadyFoundOneCombo = true;
                //add BEST x - 1 drop and x - 2 drop
                var bestXDrop =
                        HandCards_BoardCards.Where(x => new NeutralMinion(x).minionBoardCard != null && x.Cost == X_Config_Drop).
                            OrderBy(x => new NeutralMinion(x).thisCardValue).Last();
                var bestX_1Drop =
                        HandCards_BoardCards.Where(x => new NeutralMinion(x).minionBoardCard != null && x.Cost == X_Config_Drop - 1).
                            OrderBy(x => new NeutralMinion(x).thisCardValue).Last();
                var bestX_2Drop =
                        HandCards_BoardCards.Where(x => new NeutralMinion(x).minionBoardCard != null && x.Cost == X_Config_Drop - 2).
                            OrderBy(x => new NeutralMinion(x).thisCardValue).Last();

                var bestDrops = new[] { bestXDrop, bestX_1Drop, bestX_2Drop };


                foreach (var drop in bestDrops.Where(x => !MainLists.chosenCards.Contains(BoardToMulliganCard(x))))
                {
                    string dropAsString = drop.Id.ToString();
                    MainLists.chosenCards.Add(CardTemplate.StringToCard(dropAsString));
                    MainLists.blackList.Add(drop.Id.ToString());
                }
            }
        }

        public static bool HasOneTwoThreeMurlocCombo()
        {
            return MainLists.HandCards_BoardCards.Any(x => x.Type == Card.CType.MINION && x.Cost == 1 && 
                    x.Id.ToString() != "CS2_188"/*Abusive Sergeant*/ && x.Id.ToString() != "NEW1_017") /*Hungry Crab*/ &&
                    MainLists.HandCards_BoardCards.Any(x => x.Type == Card.CType.MINION && x.Cost == 2) &&
                    MainLists.HandCards_BoardCards.Any(x => x.Type == Card.CType.MINION && x.Cost == 3);
        }

        public static void AddMurlocComboToLists()
        {
            var oneDrop = MainLists.HandCards_BoardCards.First(x => x.Type == Card.CType.MINION && x.Cost == 1 &&
                                                                    x.Id.ToString() != "CS2_188" /*Abusive Sergeant*/&&
                                                                    x.Id.ToString() != "NEW1_017");
            var twoDrop =
                MainLists.HandCards_BoardCards.OrderBy(x => x.Race == Card.CRace.MURLOC)
                    .First(x => x.Type == Card.CType.MINION && x.Cost == 2);

            var threeDrop = MainLists.HandCards_BoardCards.OrderByDescending(x => x.Quality)
                    .First(x => x.Type == Card.CType.MINION && x.Cost == 3);

            MainLists.whiteList.Add(oneDrop.Id.ToString());
            MainLists.whiteList.Add(twoDrop.Id.ToString());
            MainLists.whiteList.Add(threeDrop.Id.ToString());
        }
    }
}
