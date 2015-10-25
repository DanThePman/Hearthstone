using System.Collections.Generic;
using SmartBot.Database;
using SmartBot.Plugins.API;

namespace HearthstoneMulligan
{
    /// <summary>
    /// Class which (only) indicates if the mulligan card has a bad effect
    /// </summary> 
    class CardEffects
    {
        private static List<string> CardsWithBadProperties = new List<string>
        {
            //Toxitron
            "EX1_577",//The Beast
            "CS2_227",//Venture Co. Mercenary
            "EX1_045",//Ancient Watcher
            "NEW1_030",//Deathwing
            "FP1_001",//Zombie Chow
            //"TU4c_001",//King Mukla
            "NEW1_021",//Doomsayer
            "GVG_076",//Explosive Sheep
            "EX1_097",//Abomination
            "FP1_024",//Unstable Ghoul
            "NEW1_020",//Wild Pyromancer
            "EX1_616",//Mana Wraith
            "FP1_017",//Nerub'ar Weblord
            "EX1_308",//Soulfire
    };

        public static bool HasBadEffect(Card.Cards _card)
        {
            var card = new BoardCard(_card).ResultingBoardCard;
            return CardsWithBadProperties.Contains(card.Name) || card.Overload > 0;
        }

        public static bool HasBadEffect(CardTemplate _card)
        {
            return CardsWithBadProperties.Contains(_card.Name) || _card.Overload > 0;
        }

        public static bool HasGoodEffect(Card.Cards _card)
        {
            var card = new BoardCard(_card).ResultingBoardCard;
            return card.Charge || card.Divineshield || card.Enrage || card.Freeze ||
                card.Stealth || card.Taunt || card.Windfury || card.Spellpower > 0;
        }

        public static bool HasGoodEffect(CardTemplate _card)
        {
            return _card.Charge || _card.Divineshield || _card.Enrage || _card.Freeze ||
                _card.Stealth || _card.Taunt || _card.Windfury || _card.Spellpower > 0;
        }

        public static bool HasInspire(Card.Cards _card)
        {
            var card = new BoardCard(_card).ResultingBoardCard;
            return card.Inspire;
        }

        public static bool HasInspire(CardTemplate _card)
        {
            return _card.Inspire;
        }
    }
}
