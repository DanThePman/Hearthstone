﻿using System.Linq;
using SmartBot.Plugins.API;

namespace HearthstoneMulligan
{
    class Murloc
    {
        public static bool IsMurlocDeck
        {
            get
            {
                //Deck ownDeck = Bot.CurrentDeck();

                //int murlocCount = ownDeck.Cards.Count(x => new BoardCard(x).ResultingBoardCard.Race == Card.CRace.MURLOC);

                //return murlocCount >= 7;
                return false;
            }
        }
    }
}