using Microsoft.VisualStudio.TestTools.UnitTesting;
using IsengardClient;
namespace IsengardClient.Tests
{
    [TestClass]
    public class MacroTests
    {
        [TestMethod]
        public void TestMacros()
        {
            Macro m = Macro.GenerateCannedMacro("A*");

            foreach (var _ in m.GetMagicSteps())
            {
                Assert.Fail();
            }
            int i = 0;
            foreach (var nextStep in m.GetMeleeSteps(false))
            {
                if (nextStep != MeleeCombatStep.RegularAttack)
                {
                    Assert.Fail();
                }
                i++;
                if (i == 10)
                {
                    break;
                }
            }
            Assert.AreEqual(i, 10);

            var magicSteps = m.GetMagicSteps();
            var magicEnumerator = magicSteps.GetEnumerator();
            bool move = magicEnumerator.MoveNext();
            Assert.AreEqual(move, false);

            var meleeSteps = m.GetMeleeSteps(false);
            var meleeEnumerator = meleeSteps.GetEnumerator();
            for (int j = 0; j < 10; j++)
            {
                Assert.AreEqual(meleeEnumerator.MoveNext(), true);
                Assert.AreEqual(meleeEnumerator.Current.Value, MeleeCombatStep.RegularAttack);
            }
        }
    }
}
