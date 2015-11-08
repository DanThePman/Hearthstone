using System;
using System.Linq;
using SmartBot.Plugins.API;

namespace HearthstoneMulligan.HeroInstances
{
    class Murloc
    {
        public static bool IsMurlocDeck
        {
            get
            {
                try
                {
                    int murlocCount = MainLists.currentDeck.Cards.
                        Count(x => new BoardCard(x).ResultingBoardCard.Race == Card.CRace.MURLOC);

                    return murlocCount >= 7;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
