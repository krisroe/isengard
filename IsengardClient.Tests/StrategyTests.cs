using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Globalization;

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
            ValidateCastStrategy(strategy, null, true, MagicStrategyStep.GenericHeal);
            ValidateIndefiniteAttackStrategy(strategy, true);

            strategy = defaultStrategies[iIndex++];
            ValidateCastStrategy(strategy, null, true, MagicStrategyStep.OffensiveSpellAuto);
            ValidateIndefiniteAttackStrategy(strategy, true);

            strategy = defaultStrategies[iIndex++];
            ValidateCastStrategy(strategy, new List<MagicStrategyStep>() { MagicStrategyStep.Stun }, true, MagicStrategyStep.OffensiveSpellAuto);
            ValidateIndefiniteAttackStrategy(strategy, false);

            strategy = defaultStrategies[iIndex++];
            ValidateCastStrategy(strategy, new List<MagicStrategyStep>() { MagicStrategyStep.Stun, MagicStrategyStep.OffensiveSpellAuto, MagicStrategyStep.OffensiveSpellAuto, MagicStrategyStep.Stun }, true, MagicStrategyStep.OffensiveSpellAuto);
            ValidateIndefiniteAttackStrategy(strategy, true);

            strategy = defaultStrategies[iIndex++];
            ValidateCastStrategy(strategy, new List<MagicStrategyStep>() { MagicStrategyStep.Stun, MagicStrategyStep.OffensiveSpellAuto, MagicStrategyStep.OffensiveSpellAuto, MagicStrategyStep.Stun, MagicStrategyStep.OffensiveSpellAuto, MagicStrategyStep.OffensiveSpellAuto }, false, MagicStrategyStep.OffensiveSpellAuto);
            ValidateIndefiniteAttackStrategy(strategy, false);
        }

        private void ValidateCastStrategy(Strategy strategy, List<MagicStrategyStep> leadingSteps, bool indefinite, MagicStrategyStep indefiniteStep)
        {
            Assert.IsTrue(strategy.HasAnyMagicSteps());
            int i = 0;
            foreach (var nextStep in strategy.GetMagicSteps())
            {
                MagicStrategyStep expectedStep;
                if (leadingSteps != null && i < leadingSteps.Count)
                {
                    expectedStep = leadingSteps[i];
                }
                else
                {
                    if (!indefinite)
                    {
                        Assert.Fail();
                    }
                    expectedStep = indefiniteStep;
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
            if (indefinite)
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
            Assert.IsTrue(strategy.HasAnyMeleeSteps());

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
