using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneMulligan
{
    //sorts neutral minions by value if not listed and not enough card quality etc...
    public class MinionValue
    {
        private static Dictionary<string, float> valueCoefficients = new Dictionary<string, float>();

        private static Value IncreaseCardValue(Value currentValue)
        {
            switch (currentValue)
            {
                case Value.Good:
                    return Value.Excellent;
                case Value.Medium:
                    return Value.Good;
                case Value.Bad:
                    return Value.Medium;
            }

            return currentValue;
        }

        private static Value ReduceCardValue(Value currentValue)
        {
            switch (currentValue)
            {
                case Value.Excellent:
                    return Value.Good;
                case Value.Good:
                    return Value.Medium;
                case Value.Medium:
                    return Value.Bad;
            }

            return currentValue;
        }
        public static Value SetCardValue(BoardCard minionBoardCard)
        {
            Value resultingValue;

            float rawValue = (minionBoardCard.ResultingBoardCard.Atk + minionBoardCard.ResultingBoardCard.Health) / 2;
            float resultingValueFloat = rawValue - minionBoardCard.ResultingBoardCard.Cost;

            if (resultingValueFloat > 0)
                resultingValue = Value.Good;
            else if (resultingValueFloat == 0)
                resultingValue = Value.Medium;
            else
                resultingValue = Value.Bad;

            if (CardEffects.HasBadEffect(minionBoardCard.ResultingBoardCard))
                resultingValue = ReduceCardValue(resultingValue);

            if (CardEffects.HasGoodEffect(minionBoardCard.ResultingBoardCard))
                resultingValue = IncreaseCardValue(resultingValue);

            if (CardEffects.HasInspire(minionBoardCard.ResultingBoardCard))
                resultingValue = Value.Excellent;

            return resultingValue;
        }

        // ReSharper disable once EnumUnderlyingTypeIsInt
        public enum Value : int
        {
            NotANeutralMinion = -int.MaxValue + 1, //when ordering all hand cards for minions
            Bad = 0,
            Medium = 1,
            Good = 2,
            Excellent = 3,
            InWhiteList = int.MaxValue - 1
        }
    }
}
