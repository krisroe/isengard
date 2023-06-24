using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
namespace IsengardClient.Tests
{
    [TestClass]
    public class EntityTests
    {
        [TestMethod]
        public void TestEntityCreation()
        {
            List<string> errorMessages = new List<string>();
            ItemEntity item;
            MobEntity mob;

            errorMessages.Clear();
            item = Entity.GetEntity("21 sets of 2 gold coins", EntityTypeFlags.Item, errorMessages, null, false) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 21);
            Assert.IsTrue(item.Count == 2);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.GoldCoins);

            errorMessages.Clear();
            item = Entity.GetEntity("2 gold coins", EntityTypeFlags.Item, errorMessages, null, false) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 1);
            Assert.IsTrue(item.Count == 2);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.GoldCoins);

            errorMessages.Clear();
            item = Entity.GetEntity("2 sets of 25 copper pieces", EntityTypeFlags.Item, errorMessages, null, false) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 2);
            Assert.IsTrue(item.Count == 25);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.CopperPieces);

            errorMessages.Clear();
            item = Entity.GetEntity("400 copper pieces", EntityTypeFlags.Item, errorMessages, null, false) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 1);
            Assert.IsTrue(item.Count == 400);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.CopperPieces);

            errorMessages.Clear();
            item = Entity.GetEntity("21 sets of cloth armor", EntityTypeFlags.Item, errorMessages, null, false) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 1);
            Assert.IsTrue(item.Count == 21);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.ClothArmor);

            errorMessages.Clear();
            item = Entity.GetEntity("cloth armor", EntityTypeFlags.Item, errorMessages, null, false) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 1);
            Assert.IsTrue(item.Count == 1);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.ClothArmor);

            errorMessages.Clear();
            item = Entity.GetEntity("a cloth hat", EntityTypeFlags.Item, errorMessages, null, false) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 1);
            Assert.IsTrue(item.Count == 1);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.ClothHat);

            errorMessages.Clear();
            item = Entity.GetEntity("two cloth hats", EntityTypeFlags.Item, errorMessages, null, false) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 1);
            Assert.IsTrue(item.Count == 2);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.ClothHat);

            errorMessages.Clear();
            item = Entity.GetEntity("pot of gold", EntityTypeFlags.Item, errorMessages, null, false) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 1);
            Assert.IsTrue(item.Count == 1);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.PotOfGold);

            errorMessages.Clear();
            item = Entity.GetEntity("two sets of nunchukus", EntityTypeFlags.Item, errorMessages, null, false) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 1);
            Assert.IsTrue(item.Count == 2);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.Nunchukus);

            errorMessages.Clear();
            mob = Entity.GetEntity("a vagrant", EntityTypeFlags.Mob, errorMessages, null, false) as MobEntity;
            Assert.IsTrue(mob != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(mob.SetCount == 1);
            Assert.IsTrue(mob.Count == 1);
            Assert.IsTrue(mob.MobType == MobTypeEnum.Vagrant);

            errorMessages.Clear();
            mob = Entity.GetEntity("two vagrants", EntityTypeFlags.Mob, errorMessages, null, false) as MobEntity;
            Assert.IsTrue(mob != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(mob.SetCount == 1);
            Assert.IsTrue(mob.Count == 2);
            Assert.IsTrue(mob.MobType == MobTypeEnum.Vagrant);
        }

        [TestMethod]
        public void TestRoomProcessing()
        {
            RoomTransitionInfo oRTI = null;
            int? iDamage = null;
            TrapType? trapType = null;
            Action<FeedLineParameters, RoomTransitionInfo, int, TrapType> a = (flParams, rti, d, tt) =>
            {
                oRTI = rti;
                iDamage = d;
                trapType = tt;
            };

            FeedLineParameters flp = new FeedLineParameters(null);
            flp.PlayerNames = new HashSet<string>();
            oRTI = null;
            iDamage = null;
            trapType = null;
            RoomTransitionSequence.ProcessRoom("Room", "None", "an elven guard", null, null, a, flp, RoomTransitionType.Initial, 0, TrapType.None, false);
            Assert.IsTrue(oRTI != null);
            Assert.IsTrue(oRTI.Mobs.Count == 1);
            Assert.IsTrue(oRTI.Mobs[0] is MobEntity);
            Assert.IsTrue(oRTI.Mobs[0].MobType.Value == MobTypeEnum.ElvenGuard);
        }
    }
}
