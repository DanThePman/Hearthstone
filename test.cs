//[SetVersion(1.0.0.1)]

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartBot.Database;
using SmartBot.Mulligan;
using SmartBot.Plugins.API;

namespace SmartBotUI.Mulligan
{
    public static class Extension
    {
        public static void AddOrUpdate<TKey, TValue>(
            this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }
    }

    [Serializable]


    public class bMulliganProfile : MulliganProfile
    {

        private readonly Dictionary<string, bool> _whiteList;

        private readonly List<Card.Cards> _cardsToKeep;

        private const string Coin = "GAME_005"; //Coin Declaration
        private static bool _hasCoin;

        //[Optional] <Declare Cards in your deck>

        public bMulliganProfile()
        {
            _whiteList = new Dictionary<string, bool>();
            _cardsToKeep = new List<Card.Cards>();
        }



        public List<Card.Cards> HandleMulligan(List<Card.Cards> Choices, Card.CClass opponentClass, Card.CClass ownClass)
        {

            _hasCoin = Choices.Count > 3; //it will be true if number of choices is 4

            switch (opponentClass)
            /*
             * Adjust Mulligan based on the class you are facing
             * You can also adjust it on your own mulligan by creating similar
             * switch case with your own class in case you want AIO mulligan
             */
            {
                case Card.CClass.SHAMAN:
                    {
                        break;
                    }
                case Card.CClass.PRIEST:
                    {
                        break;
                    }
                case Card.CClass.MAGE:
                    {
                        break;
                    }
                case Card.CClass.PALADIN:
                    {
                        break;
                    }
                case Card.CClass.WARRIOR:
                    {
                        break;
                    }
                case Card.CClass.WARLOCK:
                    {
                        break;
                    }
                case Card.CClass.HUNTER:
                    {
                        break;
                    }
                case Card.CClass.ROGUE:
                    {
                        break;
                    }
                case Card.CClass.DRUID:
                    {
                        break;
                    }
                case Card.CClass.NONE: //you will never encounter this in the mulligan scenario
                    {
                        break;
                    }
                case Card.CClass.JARAXXUS: //you will never encounter this in the mulligan scenario
                    {
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException("opponentClass", opponentClass, null);
            }


            /*
             * Code below is final stage of your mulligan which will go through your whitelist
             * and your choices to see which cards are good to keep
             * It checks if you want to keep the card, do you want more than 1 copy of it
             * Non LINQ version of 6 lines below is this:
             * --------------------------------
             foreach (var s in Choices)
             {
                bool keptOneAlready = false;
                foreach (var c in _cardsToKeep)
                {
                    if (c.ToString() == s.ToString())
                    {
                        keptOneAlready = true;
                        break;
                    }
                }
                if (_whiteList.ContainsKey(s.ToString()))
                {
                    if (!keptOneAlready | _whiteList[s.ToString()]) 
                        _cardsToKeep.Add(s);
                }
             }
             * --------------------------------
             * You will not need to change anything with code below, unless you really want to. 
             */
            foreach (var s in from s in Choices let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString()) where _whiteList.ContainsKey(s.ToString()) where !keptOneAlready | _whiteList[s.ToString()] select s)
            {
                _cardsToKeep.Add(s);
            }
            return _cardsToKeep;
        }
    }
}
