using System;
using System.Collections.Generic;
using System.Linq;
using SmartBot.Database;

namespace HearthstoneMulligan.HeroInstances
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
                try
                {
                    int secretCountInDeck =
                        MainLists.currentDeck.Cards.Count(x => new BoardCard(x).ResultingBoardCard.IsSecret);

                    return secretCountInDeck;
                }
                catch
                {
                    return 0;
                }
            }
        }
    }
}
