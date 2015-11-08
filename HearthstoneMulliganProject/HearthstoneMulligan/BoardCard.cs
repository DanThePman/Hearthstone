using SmartBot.Database;
using SmartBot.Plugins.API;

namespace HearthstoneMulligan
{
    /// <summary>
    /// Converts mulligan card class to a board card class
    /// </summary> 
    public class BoardCard
    {
        /// <summary>
        /// The resulting Card
        /// </summary> 
        public CardTemplate ResultingBoardCard { get; private set; }

        public bool IsMaxManaCard { get; private set; }

        public BoardCard(Card.Cards MulliganCard)
        {
            ResultingBoardCard = ConvertToBoardCard(MulliganCard);

            IsMaxManaCard = ResultingBoardCard.Cost == MainLists.MaxManaCost;
        }

        public BoardCard(string MulliganCard_STR)
        {
            Card.Cards _mulliganCard = CardTemplate.StringToCard(MulliganCard_STR);
            ResultingBoardCard = ConvertToBoardCard(_mulliganCard);

            IsMaxManaCard = ResultingBoardCard.Cost == MainLists.MaxManaCost;
        }

        private CardTemplate ConvertToBoardCard(Card.Cards MulliganCard)
        {
            return CardTemplate.LoadFromId(MulliganCard);
        }

        public bool IsClassCard()
        {
            return ResultingBoardCard.Class != Card.CClass.NONE;
        }
    }
}
