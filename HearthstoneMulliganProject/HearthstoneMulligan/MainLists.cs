using System.Collections.Generic;
using HearthstoneMulligan.Specific_Mulligans;
using SmartBot.Database;
using SmartBot.Plugins.API;

namespace HearthstoneMulligan
{
    class MainLists
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static List<string> whiteList = new List<string>() { "GAME_005" /*Coin*/ };
        public static List<string> blackList = new List<string>();
        public static List<Card.Cards> chosenCards = new List<Card.Cards>();

        public static List<CardTemplate> HandCards_BoardCards = new List<CardTemplate>();
        public static List<Card.Cards> HandCards_native = new List<Card.Cards>();

        public static Card.CClass OwnClass = Card.CClass.NONE;
        public static Card.CClass OpponentClass = Card.CClass.NONE;

        public static DeckTypeDetector.DeckType CurrentDeckType = DeckTypeDetector.DeckType.UNKNOWN;

        public static int MaxManaCost = 3;

        public static Deck currentDeck;
    }
}
