using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace IsengardClient.Tests
{
    [TestClass]
    public class EntityTests
    {
        [TestMethod]
        public void TestEntityInitialization()
        {
            HashSet<MobTypeEnum> validateMobTypes = new HashSet<MobTypeEnum>();
            foreach (var next in MobEntity.SingularMobMapping)
            {
                if (!validateMobTypes.Contains(next.Value))
                {
                    validateMobTypes.Add(next.Value);
                }
            }
            foreach (var next in MobEntity.PluralMobMapping)
            {
                if (!validateMobTypes.Contains(next.Value))
                {
                    validateMobTypes.Add(next.Value);
                }
            }
            foreach (MobTypeEnum next in Enum.GetValues(typeof(MobTypeEnum)))
            {
                if (!validateMobTypes.Contains(next))
                {
                    throw new InvalidOperationException();
                }
            }

            HashSet<ItemTypeEnum> validateItemTypes = new HashSet<ItemTypeEnum>();
            foreach (var next in ItemEntity.SingularItemMapping)
            {
                if (!validateItemTypes.Contains(next.Value))
                {
                    validateItemTypes.Add(next.Value);
                }
            }
            foreach (var next in ItemEntity.PluralItemMapping)
            {
                if (!validateItemTypes.Contains(next.Value))
                {
                    validateItemTypes.Add(next.Value);
                }
            }
            foreach (ItemTypeEnum next in Enum.GetValues(typeof(ItemTypeEnum)))
            {
                if (!validateItemTypes.Contains(next))
                {
                    throw new InvalidOperationException();
                }
            }
        }

        [TestMethod]
        public void TestEntityCreation()
        {
            List<string> errorMessages = new List<string>();
            ItemEntity item;
            MobEntity mob;

            errorMessages.Clear();
            item = Entity.GetEntity("21 sets of 2 gold coins", EntityTypeFlags.Item, errorMessages, null) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 21);
            Assert.IsTrue(item.Count == 2);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.GoldCoins);

            errorMessages.Clear();
            item = Entity.GetEntity("2 gold coins", EntityTypeFlags.Item, errorMessages, null) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 1);
            Assert.IsTrue(item.Count == 2);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.GoldCoins);

            errorMessages.Clear();
            item = Entity.GetEntity("2 sets of 25 copper pieces", EntityTypeFlags.Item, errorMessages, null) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 2);
            Assert.IsTrue(item.Count == 25);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.CopperPieces);

            errorMessages.Clear();
            item = Entity.GetEntity("400 copper pieces", EntityTypeFlags.Item, errorMessages, null) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 1);
            Assert.IsTrue(item.Count == 400);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.CopperPieces);

            errorMessages.Clear();
            item = Entity.GetEntity("21 sets of cloth armor", EntityTypeFlags.Item, errorMessages, null) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 1);
            Assert.IsTrue(item.Count == 21);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.ClothArmor);

            errorMessages.Clear();
            item = Entity.GetEntity("cloth armor", EntityTypeFlags.Item, errorMessages, null) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 1);
            Assert.IsTrue(item.Count == 1);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.ClothArmor);

            errorMessages.Clear();
            item = Entity.GetEntity("a cloth hat", EntityTypeFlags.Item, errorMessages, null) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 1);
            Assert.IsTrue(item.Count == 1);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.ClothHat);

            errorMessages.Clear();
            item = Entity.GetEntity("two cloth hats", EntityTypeFlags.Item, errorMessages, null) as ItemEntity;
            Assert.IsTrue(item != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(item.SetCount == 1);
            Assert.IsTrue(item.Count == 2);
            Assert.IsTrue(item.ItemType == ItemTypeEnum.ClothHat);

            errorMessages.Clear();
            mob = Entity.GetEntity("a vagrant", EntityTypeFlags.Mob, errorMessages, null) as MobEntity;
            Assert.IsTrue(mob != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(mob.SetCount == 1);
            Assert.IsTrue(mob.Count == 1);
            Assert.IsTrue(mob.MobType == MobTypeEnum.Vagrant);

            errorMessages.Clear();
            mob = Entity.GetEntity("two vagrants", EntityTypeFlags.Mob, errorMessages, null) as MobEntity;
            Assert.IsTrue(mob != null);
            Assert.IsTrue(errorMessages.Count == 0);
            Assert.IsTrue(mob.SetCount == 1);
            Assert.IsTrue(mob.Count == 2);
            Assert.IsTrue(mob.MobType == MobTypeEnum.Vagrant);
        }

        public void TestRoomProcessing()
        {
            RoomTransitionInfo oRTI = null;
            int? iDamage = null;
            Action<RoomTransitionInfo, int> a = (rti, d) =>
            {
                oRTI = rti;
                iDamage = d;
            };

            FeedLineParameters flp = new FeedLineParameters(null);
            flp.PlayerNames = new HashSet<string>();
            oRTI = null;
            iDamage = null;
            RoomTransitionSequence.ProcessRoom("Room", "None", "a BOGUS,elven guard", null, null, a, flp, RoomTransitionType.Initial, 0);
            Assert.IsTrue(oRTI != null);
            Assert.IsTrue(oRTI.Mobs.Count == 2);
            Assert.IsTrue(oRTI.Mobs[0] is UnknownMobEntity);
            Assert.IsTrue(oRTI.Mobs[1] is MobEntity);
            Assert.IsTrue(oRTI.Mobs[1].MobType.Value == MobTypeEnum.ElvenGuard);
        }
    }
}
