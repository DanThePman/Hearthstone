using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SmartBot.Database;
using SmartBot.Plugins.API;

namespace HearthstoneMulligan
{
    /// <summary>
    /// Only for Neutral cards needed (subtracts coin of hand card count etc...)
    /// </summary>
    class DeckCalculation
    {
        private IEnumerable<CardTemplate> currentHandCards;
        private Deck currentDeck;
        private int handCount;
        private int mediumCardsInHandCount;
        private float minProbability;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_minProbability">Min probability of coninue drawing a good card</param>
        public DeckCalculation (float _minProbability)
        {
            List<CardTemplate> HandCards_BoardCards = MainLists.HandCards_BoardCards;

            bool coinInHand = HandCards_BoardCards.Any(x => x.Id.ToString() == MainLists.whiteList[0]);

            currentHandCards = coinInHand ? HandCards_BoardCards.ToArray().
                Where(x => x.Id.ToString() != MainLists.whiteList[0]) : HandCards_BoardCards.ToArray();
            currentDeck = Bot.CurrentDeck();
            handCount = coinInHand ? HandCards_BoardCards.Count : HandCards_BoardCards.Count - 1;

            mediumCardsInHandCount = MainLists.HandCards_BoardCards.Count(x => new NeutralMinion(x).BoardCard != null
                && NeutralMinion.WouldTakeMinion(x) && !MainLists.whiteList.Contains(x.Id.ToString()));

            if (_minProbability > 100)
                _minProbability = 100;
            else if (_minProbability < 0)
                _minProbability = 0;

            minProbability = _minProbability;
        }
        /// <summary>
        /// Inifinive
        /// </summary>
        /// <returns></returns>
        private int GetAcceptablePlusCountInDeckLeft()
        {
            return currentDeck.Cards.Count(x => 

                (MainLists.whiteList.Contains(x)) 
                || 
                (
                new NeutralMinion(CardTemplate.LoadFromId(x)).BoardCard != null && 
                    NeutralMinion.WouldTakeMinion(CardTemplate.LoadFromId(x))
                )
                

                && 
                !currentHandCards.ToList().Contains(
                       CardTemplate.LoadFromId(CardTemplate.StringToCard(x))
                ));
        }

        /// <summary>
        /// 0 - 10 index with 11 elements => 0 - 10+ mana
        /// </summary>
        /// <returns></returns>
        public static int[] GetCurves()
        {
            int[] curveArray = new int[11];

            for (int i = 0; i < 11; i++)
            {
                if (i == 10)
                {
                    curveArray[i] = MainLists.currentDeck.Cards.Count(x => CardTemplate.LoadFromId(x).Cost >= i);
                }
                else
                curveArray[i] = MainLists.currentDeck.Cards.Count(x => CardTemplate.LoadFromId(x).Cost == i);
            }

            return curveArray;
        }

        private int restCardsInDeck
        {
            get { return currentDeck.Cards.Count - handCount; }
        }
        /// <summary>
        /// When replacing not whitelisted
        /// </summary>
        /// <returns></returns>
        private float GetProbabilityOfDrawingAcceptable(int acceptableCardCountInDeck)
        {
            // ReSharper disable once RedundantCast
            return acceptableCardCountInDeck / ((float)restCardsInDeck + 1f)/* +replace not being good enough*/;
        }

        /// <summary>
        /// Generates a tree for medium+ drawing and stops when min probability of drawing another one is reached
        /// int => Medium neutral minions in hand count
        /// float => Probability
        /// </summary>
        /// <returns></returns>
        public Tuple<int, float> GenerateTreeWhiteListDraw()
        {
            float probability_LinearInterpolated = -1;
            int badCardsLeft = mediumCardsInHandCount;

            if (badCardsLeft == 0)
                return new Tuple<int, float>(0, 0);

            int acceptableInDeckCount = GetAcceptablePlusCountInDeckLeft();
            while (badCardsLeft > 0)
            {
                if (probability_LinearInterpolated == -1) //init value
                {
                    probability_LinearInterpolated =
                        GetProbabilityOfDrawingAcceptable(acceptableInDeckCount);
                }
                else
                {
                    probability_LinearInterpolated *=
                        GetProbabilityOfDrawingAcceptable(acceptableInDeckCount);
                }

                if (probability_LinearInterpolated*100 < minProbability)
                    return new Tuple<int, float>(badCardsLeft, (probability_LinearInterpolated /=
                        GetProbabilityOfDrawingAcceptable(acceptableInDeckCount))*100 );

                --acceptableInDeckCount;//drawing one => one less in deck
                --badCardsLeft;//get out of the loop & less bad in hand
            }

            return new Tuple<int, float>(0, probability_LinearInterpolated*100);
        }
    }
}
