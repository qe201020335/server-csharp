using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Utils;

namespace UnitTests.Tests.Helpers
{
    [TestClass]
    public class BotGeneratorHelperTests
    {
        private BotGeneratorHelper _botGeneratorHelper;

        [TestInitialize]
        public void Initialize()
        {
            _botGeneratorHelper = DI.GetService<BotGeneratorHelper>();
            var databaseImporter = DI.GetService<DatabaseImporter>();
            Task.Factory.StartNew(() =>
            {
                databaseImporter.OnLoad();
            });
            ;
        }

        [TestMethod]
        public void AddItemWithChildrenToEquipmentSlot_fit_vertical()
        {
            var stashId = new MongoId();
            var equipmentId = new MongoId();
            var botInventory = new BotBaseInventory
            {
                Items = [],
                Stash = stashId,
                Equipment = equipmentId,
            };

            // Create backpack on player
            var backpack = new Item
            {
                Id = new MongoId(),
                // Has a 3grids, first is a 3hx5v grid
                Template = ItemTpl.BACKPACK_EBERLESTOCK_G2_GUNSLINGER_II_BACKPACK_DRY_EARTH,
                ParentId = equipmentId,
                SlotId = "Backpack",
            };
            botInventory.Items.Add(backpack);

            var rootWeaponId = new MongoId();
            var weaponWithChildren = CreateMp18(rootWeaponId);

            var result = _botGeneratorHelper.AddItemWithChildrenToEquipmentSlot(
                [EquipmentSlots.Backpack],
                rootWeaponId,
                ItemTpl.SHOTGUN_MP18_762X54R_SINGLESHOT_RIFLE,
                weaponWithChildren,
                botInventory
            );

            Assert.AreEqual(ItemAddedResult.SUCCESS, result);

            var weaponRoot = weaponWithChildren.FirstOrDefault(item => item.Id == rootWeaponId);
            Assert.AreEqual((weaponRoot.Location as ItemLocation).X, 0);
            Assert.AreEqual((weaponRoot.Location as ItemLocation).Y, 0);
            Assert.AreEqual((weaponRoot.Location as ItemLocation).R, ItemRotation.Vertical);
        }

        private static List<Item> CreateMp18(MongoId rootWeaponId)
        {
            var weaponWithChildren = new List<Item>();
            var weaponRoot = new Item
            {
                Id = rootWeaponId,
                Template = ItemTpl.SHOTGUN_MP18_762X54R_SINGLESHOT_RIFLE,
            };
            weaponWithChildren.Add(weaponRoot);
            var weaponStock = new Item
            {
                Id = new MongoId(),
                Template = ItemTpl.STOCK_MP18_WOODEN,
                ParentId = weaponRoot.Id,
                SlotId = "mod_stock",
            };
            weaponWithChildren.Add(weaponStock);
            var weaponBarrel = new Item
            {
                Id = new MongoId(),
                Template = ItemTpl.BARREL_MP18_762X54R_600MM,
                ParentId = weaponRoot.Id,
                SlotId = "mod_barrel",
            };
            weaponWithChildren.Add(weaponBarrel);

            return weaponWithChildren;
        }

        [TestMethod]
        public void AddItemWithChildrenToEquipmentSlot_fit_horizontal()
        {
            var stashId = new MongoId();
            var equipmentId = new MongoId();
            var botInventory = new BotBaseInventory
            {
                Items = [],
                Stash = stashId,
                Equipment = equipmentId,
            };

            // Create backpack on player
            var backpack = new Item
            {
                Id = new MongoId(),
                Template = ItemTpl.BACKPACK_ANA_TACTICAL_BETA_2_BATTLE_BACKPACK_OLIVE_DRAB,
                ParentId = equipmentId,
                SlotId = "Backpack",
            };
            botInventory.Items.Add(backpack);

            var rootWeaponId = new MongoId();
            var weaponWithChildren = CreateMp18(rootWeaponId);

            var result = _botGeneratorHelper.AddItemWithChildrenToEquipmentSlot(
                [EquipmentSlots.Backpack],
                rootWeaponId,
                ItemTpl.SHOTGUN_MP18_762X54R_SINGLESHOT_RIFLE,
                weaponWithChildren,
                botInventory
            );

            Assert.AreEqual(ItemAddedResult.SUCCESS, result);

            var weaponRoot = weaponWithChildren.FirstOrDefault(item => item.Id == rootWeaponId);
            Assert.AreEqual((weaponRoot.Location as ItemLocation).X, 0);
            Assert.AreEqual((weaponRoot.Location as ItemLocation).Y, 0);
            Assert.AreEqual((weaponRoot.Location as ItemLocation).R, ItemRotation.Horizontal);
        }

        /// <summary>
        /// Backpack with one bullet in top row, blocking gun from being placed at 0,0
        /// </summary>
        [TestMethod]
        public void AddItemWithChildrenToEquipmentSlot_fit_vertical_with_items_in_backpack()
        {
            var botInventory = new BotBaseInventory { Items = [] };
            var backpack = new Item
            {
                Id = new MongoId(),
                // Has a 3hx5v grid first
                Template = ItemTpl.BACKPACK_EBERLESTOCK_G2_GUNSLINGER_II_BACKPACK_DRY_EARTH,
                SlotId = "Backpack",
            };
            botInventory.Items.Add(backpack);

            botInventory.Items.Add(
                new Item
                {
                    Id = new MongoId(),
                    Template = ItemTpl.AMMO_762X25TT_AKBS,
                    ParentId = backpack.Id,
                    SlotId = "main",
                    Location = new ItemLocation
                    {
                        X = 0,
                        Y = 0,
                        R = ItemRotation.Horizontal,
                    },
                    Upd = new Upd { StackObjectsCount = 1 },
                }
            );

            var rootWeaponId = new MongoId();
            var weaponWithChildren = CreateMp18(rootWeaponId);

            var result = _botGeneratorHelper.AddItemWithChildrenToEquipmentSlot(
                [EquipmentSlots.Backpack],
                rootWeaponId,
                ItemTpl.SHOTGUN_MP18_762X54R_SINGLESHOT_RIFLE,
                weaponWithChildren,
                botInventory
            );

            Assert.AreEqual(ItemAddedResult.SUCCESS, result);

            var weaponRoot = weaponWithChildren.FirstOrDefault(item => item.Id == rootWeaponId);
            Assert.AreEqual((weaponRoot.Location as ItemLocation).X, 1);
            Assert.AreEqual((weaponRoot.Location as ItemLocation).Y, 0);
            Assert.AreEqual((weaponRoot.Location as ItemLocation).R, ItemRotation.Vertical);
        }

        /// <summary>
        /// No space for gun
        /// </summary>
        [TestMethod]
        public void AddItemWithChildrenToEquipmentSlot_no_space()
        {
            var botInventory = new BotBaseInventory { Items = [] };
            var backpack = new Item
            {
                Id = new MongoId(),
                // Has a 3hx5v grid first
                Template = ItemTpl.BACKPACK_EBERLESTOCK_G2_GUNSLINGER_II_BACKPACK_DRY_EARTH,
                SlotId = "Backpack",
            };
            botInventory.Items.Add(backpack);

            botInventory.Items.AddRange(
                new Item
                {
                    Id = new MongoId(),
                    Template = ItemTpl.AMMO_762X25TT_AKBS,
                    ParentId = backpack.Id,
                    SlotId = "main",
                    Location = new ItemLocation
                    {
                        X = 0,
                        Y = 0,
                        R = ItemRotation.Horizontal,
                    },
                    Upd = new Upd { StackObjectsCount = 1 },
                },
                new Item
                {
                    Id = new MongoId(),
                    Template = ItemTpl.AMMO_762X25TT_AKBS,
                    ParentId = backpack.Id,
                    SlotId = "main",
                    Location = new ItemLocation
                    {
                        X = 1,
                        Y = 0,
                        R = ItemRotation.Horizontal,
                    },
                    Upd = new Upd { StackObjectsCount = 1 },
                },
                new Item
                {
                    Id = new MongoId(),
                    Template = ItemTpl.AMMO_762X25TT_AKBS,
                    ParentId = backpack.Id,
                    SlotId = "main",
                    Location = new ItemLocation
                    {
                        X = 2,
                        Y = 0,
                        R = ItemRotation.Horizontal,
                    },
                    Upd = new Upd { StackObjectsCount = 1 },
                }
            );

            var rootWeaponId = new MongoId();
            var weaponWithChildren = CreateMp18(rootWeaponId);

            var result = _botGeneratorHelper.AddItemWithChildrenToEquipmentSlot(
                [EquipmentSlots.Backpack],
                rootWeaponId,
                ItemTpl.SHOTGUN_MP18_762X54R_SINGLESHOT_RIFLE,
                weaponWithChildren,
                botInventory
            );

            Assert.AreEqual(ItemAddedResult.NO_SPACE, result);
        }
    }
}
