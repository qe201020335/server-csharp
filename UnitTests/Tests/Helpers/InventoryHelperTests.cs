using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Utils;

namespace UnitTests.Tests.Helpers
{
    [TestClass]
    public class InventoryHelperTests
    {
        private InventoryHelper _helper;
        private PresetHelper _presetHelper;

        [TestInitialize]
        public void Initialize()
        {
            _helper = DI.GetService<InventoryHelper>();
            var databaseImporter = DI.GetService<DatabaseImporter>();
            _presetHelper = DI.GetService<PresetHelper>();
            Task.Factory.StartNew(() =>
            {
                databaseImporter.OnLoad();
            });
        }

        [TestMethod]
        public void GetItemSize_vss_val()
        {
            var vssValPreset = _presetHelper.GetDefaultPreset(
                ItemTpl.MARKSMANRIFLE_VSS_VINTOREZ_9X39_SPECIAL_SNIPER_RIFLE
            );

            var result = _helper.GetItemSize(
                ItemTpl.MARKSMANRIFLE_VSS_VINTOREZ_9X39_SPECIAL_SNIPER_RIFLE,
                vssValPreset.Parent,
                vssValPreset.Items
            );

            Assert.AreEqual(5, result.Item1);
            Assert.AreEqual(2, result.Item2);
        }

        [TestMethod]
        public void GetItemSize_m4a1()
        {
            var vssValPreset = _presetHelper.GetDefaultPreset(
                ItemTpl.ASSAULTRIFLE_COLT_M4A1_556X45_ASSAULT_RIFLE
            );

            var result = _helper.GetItemSize(
                ItemTpl.ASSAULTRIFLE_COLT_M4A1_556X45_ASSAULT_RIFLE,
                vssValPreset.Parent,
                vssValPreset.Items
            );

            Assert.AreEqual(5, result.Item1);
            Assert.AreEqual(2, result.Item2);
        }

        [TestMethod]
        public void GetItemSize_glock_17()
        {
            var vssValPreset = _presetHelper.GetDefaultPreset(ItemTpl.PISTOL_GLOCK_17_9X19);

            var result = _helper.GetItemSize(
                ItemTpl.PISTOL_GLOCK_17_9X19,
                vssValPreset.Parent,
                vssValPreset.Items
            );

            Assert.AreEqual(2, result.Item1);
            Assert.AreEqual(1, result.Item2);
        }
    }
}
