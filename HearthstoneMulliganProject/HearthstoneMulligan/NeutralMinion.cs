using System;
using System.Windows;
using HearthstoneMulligan;
using SmartBot.Database;
using SmartBot.Plugins.API;

namespace HearthstoneMulligan
{
    /// <summary>
    /// Class which includes the quality of a neutral minion
    /// </summary> 
    public class NeutralMinion
    {
        public static void ManageNeutralMinion(Card.Cards card)
        {
            //<= max mana
            var boardCard = new BoardCard(card);

            if (boardCard.ResultingBoardCard.Quality >= ValueReader.MinCardQualityToInstantAddMinion) //epic by default
                MainLists.chosenCards.Add(card);
            else
            {
                //card quality not hight enoughbut <= max mana
                if (!ValueReader.AttendMinionValue ||
                    boardCard.ResultingBoardCard.Cost < ValueReader.MinManaCostToAttendValue)
                    //mana <= max cost & mana < MinManaCostToAttendValue
                    MainLists.chosenCards.Add(card);
                else if (boardCard.ResultingBoardCard.Cost >= ValueReader.MinManaCostToAttendValue)
                {
                    var minionCard = new NeutralMinion(card);
                    MinionValue.Value requiredMinNeutralMinionValue =
                        minionCard.minionBoardCard.IsMaxManaCard && ValueReader.IncreaseMinMinionValueIfMaxCost
                            ? ValueReader.IncreasedMinNeutralMinionValue
                            : ValueReader.MinNeutralMinionValue;

                    if (minionCard.thisCardValue >= requiredMinNeutralMinionValue)
                        MainLists.chosenCards.Add(card);
                }
            }
        }

        public static bool WouldTakeMinion(CardTemplate boardCard)
        {
            try
            {
                if (new NeutralMinion(boardCard).minionBoardCard == null) //not a neutral minion
                    return false;
                //<= max mana
                Card.Cards card = CardTemplate.StringToCard(boardCard.Id.ToString());

                #region normalChecks

                if (MainLists.whiteList.Contains(boardCard.Id.ToString()))
                    return true;
                if (MainLists.blackList.Contains((boardCard.Id.ToString())))
                    return false;

                if (boardCard.Quality >= ValueReader.MinCardQualityToInstantAddMinion) //epic by default
                    return true;

                //card quality not hight enough and mana to high too
                if ((!ValueReader.AttendMinionValue ||
                     boardCard.Cost < ValueReader.MinManaCostToAttendValue) && boardCard.Cost <= MainLists.MaxManaCost)
                    return true;

                if (boardCard.Cost > MainLists.MaxManaCost)
                    return false;

                //value has to be attended
                var minionCard = new NeutralMinion(card);
                MinionValue.Value requiredMinNeutralMinionValue =
                    minionCard.minionBoardCard.IsMaxManaCard && ValueReader.IncreaseMinMinionValueIfMaxCost
                        ? ValueReader.IncreasedMinNeutralMinionValue
                        : ValueReader.MinNeutralMinionValue;

                return minionCard.thisCardValue >= requiredMinNeutralMinionValue;

                #endregion normalChecks
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

        }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public BoardCard minionBoardCard { get; private set; }
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public MinionValue.Value thisCardValue { get; private set; }

        private NeutralMinion(Card.Cards NeutralMinionMulliganCard)
        {
            minionBoardCard = new BoardCard(NeutralMinionMulliganCard).IsClassCard() ||
                NeutralMinionMulliganCard.ToString() == "GAME_005"
                ?
                null                
                :
                new BoardCard(NeutralMinionMulliganCard);

            thisCardValue =
                minionBoardCard != null
                    ? MinionValue.SetCardValue(minionBoardCard)
                    : MinionValue.Value.NotANeutralMinion;

            if (minionBoardCard != null && MainLists.whiteList.Contains(minionBoardCard.ResultingBoardCard.Name))
                thisCardValue = MinionValue.Value.InWhiteList;
        }


        private Card.Cards BoardToMulliganCard(CardTemplate boardCard)
        {
            return CardTemplate.StringToCard(boardCard.Id.ToString());
        }

        /// <summary>
        /// load obvious whitelist first to determine if Value.InWhiteList
        /// </summary>
        /// <param name="NeutralMinionBoardCard"></param>
        public NeutralMinion(CardTemplate NeutralMinionBoardCard)
        {
            Card.Cards NeutralMinionMulliganCardCard = BoardToMulliganCard(NeutralMinionBoardCard);

            minionBoardCard = NeutralMinionBoardCard.Class != Card.CClass.NONE ||
                NeutralMinionMulliganCardCard.ToString() == "GAME_005"
                ?
                null             
                :
                new BoardCard(NeutralMinionMulliganCardCard);

            thisCardValue = 
                minionBoardCard != null ? MinionValue.SetCardValue(minionBoardCard) : 
                MinionValue.Value.NotANeutralMinion;

            if (minionBoardCard != null && MainLists.whiteList.Contains(minionBoardCard.ResultingBoardCard.Name))
                thisCardValue = MinionValue.Value.InWhiteList;
        }
    }
}