using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBot.Database;
using SmartBot.Plugins.API;

namespace HearthstoneMulligan
{
    class Mage
    {
        public static bool IsHoldingAllSecretsInHand(List<CardTemplate> HandCards_BoardCards)
        {
            //int secretsInHand = HandCards_BoardCards.Count(x => x.IsSecret);

            //if (secretsInHand == secretsInDeck)
            //    return true;

            return false;
        }

        private static int secretsInDeck
        {
            get
            {
                //Deck ownDeck = Bot.CurrentDeck();

                //int secretCountInDeck =
                //    ownDeck.Cards.Count(x => new BoardCard(x).ResultingBoardCard.IsSecret);

                //return secretCountInDeck;
                return 0;
            }
        }
    }
}
