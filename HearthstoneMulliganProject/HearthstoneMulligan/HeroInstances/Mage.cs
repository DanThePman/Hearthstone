using System.Collections.Generic;
using System.Linq;
using SmartBot.Database;
using SmartBot.Plugins.API;

namespace HearthstoneMulligan
{
    class Mage
    {
        public static bool IsHoldingAllSecretsInHand(List<CardTemplate> HandCards_BoardCards)
        {
            int secretsInHand = HandCards_BoardCards.Count(x => x.IsSecret);

            return secretsInHand == secretsInDeck;
        }

        private static int secretsInDeck
        {
            get
            {
                Deck ownDeck = Bot.CurrentDeck();//MulliganTester doesnt Handle this :(

                int? secretCountInDeck =
                    ownDeck?.Cards.Count(x => new BoardCard(x).ResultingBoardCard.IsSecret);

                return secretCountInDeck ?? int.MaxValue;
            }
        }
    }
}
