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
            _ = databaseImporter.OnLoad();
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

            var weaponWithChildren = new List<Item>();
            var weaponRoot = new Item
            {
                Id = new MongoId(),
                Template = ItemTpl.SHOTGUN_MP18_762X54R_SINGLESHOT_RIFLE,
            };
            weaponWithChildren.Add(weaponRoot);

            var result = _botGeneratorHelper.AddItemWithChildrenToEquipmentSlot(
                [EquipmentSlots.Backpack],
                weaponRoot.Id,
                weaponRoot.Template,
                weaponWithChildren,
                botInventory
            );

            Assert.Equals(result, ItemAddedResult.SUCCESS);
            Assert.Equals((weaponRoot.Location as ItemLocation).X, 0);
            Assert.Equals((weaponRoot.Location as ItemLocation).Y, 0);
            Assert.Equals((weaponRoot.Location as ItemLocation).R, ItemRotation.Vertical);
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

            var weaponWithChildren = new List<Item>();
            var weaponRoot = new Item
            {
                Id = new MongoId(),
                Template = ItemTpl.SHOTGUN_MP18_762X54R_SINGLESHOT_RIFLE,
            };
            weaponWithChildren.Add(weaponRoot);

            var result = _botGeneratorHelper.AddItemWithChildrenToEquipmentSlot(
                [EquipmentSlots.Backpack],
                weaponRoot.Id,
                weaponRoot.Template,
                weaponWithChildren,
                botInventory
            );

            Assert.Equals(result, ItemAddedResult.SUCCESS);
            Assert.Equals((weaponRoot.Location as ItemLocation).X, 0);
            Assert.Equals((weaponRoot.Location as ItemLocation).Y, 0);
            Assert.Equals((weaponRoot.Location as ItemLocation).R, ItemRotation.Horizontal);
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
                        X = 1,
                        Y = 0,
                        R = ItemRotation.Horizontal,
                    },
                    Upd = new Upd { StackObjectsCount = 1 },
                }
            );

            var weaponWithChildren = new List<Item>();
            var weaponRoot = new Item
            {
                Id = new MongoId(),
                Template = ItemTpl.SHOTGUN_MP18_762X54R_SINGLESHOT_RIFLE,
            };
            weaponWithChildren.Add(weaponRoot);

            var result = _botGeneratorHelper.AddItemWithChildrenToEquipmentSlot(
                [EquipmentSlots.Backpack],
                weaponRoot.Id,
                weaponRoot.Template,
                weaponWithChildren,
                botInventory
            );

            Assert.Equals(result, ItemAddedResult.SUCCESS);
            Assert.Equals((weaponRoot.Location as ItemLocation).X, 0);
            Assert.Equals((weaponRoot.Location as ItemLocation).Y, 1);
            Assert.Equals((weaponRoot.Location as ItemLocation).R, ItemRotation.Vertical);
        }
    }
}
