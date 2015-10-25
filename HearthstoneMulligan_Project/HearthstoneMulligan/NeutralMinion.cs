using HearthstoneMulligan;
using SmartBot.Database;
using SmartBot.Plugins.API;

/// <summary>
/// Class which includes the quality of a neutral minion
/// </summary> 
public class NeutralMinion
{
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