using IsengardClient.Backend;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
namespace IsengardClient.Tests
{
    [TestClass]
    public class StrategyTests
    {
        [TestMethod]
        public void TestStrategies()
        {
            Strategy strategy;

            List<Strategy> defaultStrategies = new List<Strategy>(Strategy.GetDefaultStrategies());

            int iIndex = 0;

            strategy = defaultStrategies[iIndex++];
            Assert.IsTrue(strategy.TypesWithStepsEnabled == CommandType.None);

            strategy = defaultStrategies[iIndex++];
            ValidateCastStrategy(strategy, null, new List<MagicStrategyStep>() { MagicStrategyStep.GenericHeal });
            ValidateIndefiniteAttackStrategy(strategy, true);

            strategy = defaultStrategies[iIndex++];
            ValidateCastStrategy(strategy, null, new List<MagicStrategyStep>() { MagicStrategyStep.OffensiveSpellAuto });
            ValidateIndefiniteAttackStrategy(strategy, true);

            strategy = defaultStrategies[iIndex++];
            ValidateCastStrategy(strategy, new List<MagicStrategyStep>() { MagicStrategyStep.Stun }, new List<MagicStrategyStep>() { MagicStrategyStep.OffensiveSpellAuto });
            ValidateIndefiniteAttackStrategy(strategy, false);

            strategy = defaultStrategies[iIndex++];
            ValidateCastStrategy(strategy, null, new List<MagicStrategyStep>() { MagicStrategyStep.StunWand, MagicStrategyStep.OffensiveSpellAuto });
            ValidateIndefiniteAttackStrategy(strategy, true);
        }

        private void ValidateCastStrategy(Strategy strategy, List<MagicStrategyStep> leadingSteps, List<MagicStrategyStep> indefiniteSteps)
        {
            Assert.IsTrue(strategy.HasAnyMagicSteps(null));
            int i = 0;
            int indefiniteStepsIndex = 0;
            foreach (var nextStep in strategy.GetMagicSteps())
            {
                MagicStrategyStep expectedStep = MagicStrategyStep.CurePoison;
                if (leadingSteps != null && i < leadingSteps.Count)
                {
                    expectedStep = leadingSteps[i];
                }
                else
                {
                    if (indefiniteSteps != null)
                    {
                        expectedStep = indefiniteSteps[indefiniteStepsIndex];
                        indefiniteStepsIndex = (indefiniteStepsIndex + 1) % indefiniteSteps.Count;
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
                if (nextStep != expectedStep)
                {
                    Assert.Fail();
                }
                i++;
                if (i == 10)
                {
                    break;
                }
            }
            int iExpectedCount;
            if (indefiniteSteps != null)
            {
                iExpectedCount = 10;
            }
            else
            {
                iExpectedCount = leadingSteps.Count;
            }
            Assert.AreEqual(i, iExpectedCount);
        }

        private void ValidateIndefiniteAttackStrategy(Strategy strategy, bool powerAttack)
        {
            Assert.IsTrue(strategy.HasAnyMeleeSteps(null));

            MeleeStrategyStep expectedStep;
            foreach (int j in new int[] { 1, 2, 10 })
            {
                int i = 0;
                foreach (var nextStep in strategy.GetMeleeSteps(powerAttack))
                {
                    if (powerAttack && i == 0)
                        expectedStep = MeleeStrategyStep.PowerAttack;
                    else
                        expectedStep = MeleeStrategyStep.RegularAttack;
                    if (nextStep != expectedStep)
                    {
                        Assert.Fail();
                    }
                    i++;
                    if (i == j) break;
                }
                Assert.AreEqual(i, j);

                var meleeSteps = strategy.GetMeleeSteps(powerAttack);
                var meleeEnumerator = meleeSteps.GetEnumerator();
                for (int k = 0; k < j+1; k++)
                {
                    if (powerAttack && k == 0)
                        expectedStep = MeleeStrategyStep.PowerAttack;
                    else
                        expectedStep = MeleeStrategyStep.RegularAttack;
                    Assert.AreEqual(meleeEnumerator.MoveNext(), k != j+1);
                    if (k != j+1)
                    {
                        Assert.AreEqual(meleeEnumerator.Current, expectedStep);
                    }
                }
            }
        }
    }
}
