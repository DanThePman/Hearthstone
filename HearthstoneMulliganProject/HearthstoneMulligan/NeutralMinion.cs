using System;
using System.Windows;
using HearthstoneMulligan;
using SmartBot.Database;
using SmartBot.Plugins.API;

/// <summary>
/// Class which includes the quality of a neutral minion
/// </summary> 
public class NeutralMinion
{
    public static int MaxManaCostFromMain;
    public static void ManageNeutralMinion(Card.Cards card)
    {
        //<= max mana
        var boardCard = new BoardCard(card);

        if (boardCard.ResultingBoardCard.Quality >= ValueReader.MinCardQualityToInstantAddMinion) //epic by default
            MainLists.chosenCards.Add(card);
        else
        { //card quality not hight enough
            if (!ValueReader.AttendMinionValue ||
                boardCard.ResultingBoardCard.Cost < ValueReader.MinManaCostToAttendValue)//mana <= max cost & mana < MinManaCostToAttendValue
                MainLists.chosenCards.Add(card);
            else if (boardCard.ResultingBoardCard.Cost >= ValueReader.MinManaCostToAttendValue)
            {
                var minionCard = new NeutralMinion(card);
                NeutralMinion.Value requiredMinNeutralMinionValue =
                    minionCard.BoardCard.IsMaxManaCard && ValueReader.IncreaseMinMinionValueIfMaxCost
                    ?
                    ValueReader.IncreasedMinNeutralMinionValue
                    :
                    ValueReader.MinNeutralMinionValue;

                if (minionCard.CardValue >= requiredMinNeutralMinionValue)
                    MainLists.chosenCards.Add(card);
            }
        }
    }

    public static bool WouldTakeMinion(CardTemplate boardCard)
    {
        try
        {
            if (new NeutralMinion(boardCard).BoardCard == null) //not a neutral minion
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
                 boardCard.Cost < ValueReader.MinManaCostToAttendValue) && boardCard.Cost <= ValueReader.MaxManaCost)
                return true;

            if (boardCard.Cost > ValueReader.MaxManaCost)
                return false;

            //value has to be attended
            var minionCard = new NeutralMinion(card);
            NeutralMinion.Value requiredMinNeutralMinionValue =
                minionCard.BoardCard.IsMaxManaCard && ValueReader.IncreaseMinMinionValueIfMaxCost
                    ? ValueReader.IncreasedMinNeutralMinionValue
                    : ValueReader.MinNeutralMinionValue;

            return minionCard.CardValue >= requiredMinNeutralMinionValue;

            #endregion normalChecks
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return false;
        }
              
    }

    public BoardCard BoardCard { get; private set; }
    public Value CardValue { get; private set; }

    public enum Value : int
    {
        NotANeutralMinion = -int.MaxValue, //when ordering all hand cards for minions
        Bad = 0,
        Medium = 1,
        Good = 2,
        Excellent = 3,
        InWhiteList = int.MaxValue
    }

    public NeutralMinion(Card.Cards NeutralMinionMulliganCard)
    {
        BoardCard = !new BoardCard(NeutralMinionMulliganCard).IsClassCard() ?
            new BoardCard(NeutralMinionMulliganCard) : null;

        if (BoardCard != null)
            SetCardValue();
        else
            CardValue = Value.Bad;
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

        BoardCard = NeutralMinionBoardCard.Class == Card.CClass.NONE ?
            new BoardCard(NeutralMinionMulliganCardCard) : null;

        if (BoardCard != null)
            SetCardValue();
        else
            CardValue = Value.NotANeutralMinion;

        if (BoardCard != null && MainLists.whiteList.Contains(BoardCard.ResultingBoardCard.Name))
            CardValue = Value.InWhiteList;
    }

    private void SetCardValue()
    {
        float rawValue = (BoardCard.ResultingBoardCard.Atk + BoardCard.ResultingBoardCard.Health) / 2;
        float resultingValue = rawValue - BoardCard.ResultingBoardCard.Cost;

        if (resultingValue > 0)
            CardValue = Value.Good;
        else if (resultingValue == 0)
            CardValue = Value.Medium;
        else
            CardValue = Value.Bad;

        if (CardEffects.HasBadEffect(BoardCard.ResultingBoardCard))
            ReduceCardValue();

        if (CardEffects.HasGoodEffect(BoardCard.ResultingBoardCard))
            IncreaseCardValue();

        if (CardEffects.HasInspire(BoardCard.ResultingBoardCard))
            CardValue = Value.Excellent;
    }

    private void IncreaseCardValue()
    {
        switch (CardValue)
        {
            case Value.Good:
                CardValue = Value.Excellent;
                break;
            case Value.Medium:
                CardValue = Value.Good;
                break;
            case Value.Bad:
                CardValue = Value.Medium;
                break;
        }
    }

    private void ReduceCardValue()
    {
        switch (CardValue)
        {
            case Value.Excellent:
                CardValue = Value.Good;
                break;
            case Value.Good:
                CardValue = Value.Medium;
                break;
            case Value.Medium:
                CardValue = Value.Bad;
                break;
        }
    }
}