using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;

namespace SPTarkov.Server.Core.Extensions
{
    public static class ProfileExtensions
    {
        /// <summary>
        ///     Return all quest items current in the supplied profile
        /// </summary>
        /// <param name="profile">Profile to get quest items from</param>
        /// <returns>List of item objects</returns>
        public static List<Item> GetQuestItemsInProfile(this PmcData profile)
        {
            return profile
                ?.Inventory?.Items.Where(i => i.ParentId == profile.Inventory.QuestRaidItems)
                .ToList();
        }

        /// <summary>
        ///     Upgrade hideout wall from starting level to interactable level if necessary stations have been upgraded
        /// </summary>
        /// <param name="profile">Profile to upgrade wall in</param>
        public static void UnlockHideoutWallInProfile(this PmcData profile)
        {
            var profileHideoutAreas = profile.Hideout.Areas;
            var waterCollector = profileHideoutAreas.FirstOrDefault(x =>
                x.Type == HideoutAreas.WaterCollector
            );
            var medStation = profileHideoutAreas.FirstOrDefault(x =>
                x.Type == HideoutAreas.MedStation
            );
            var wall = profileHideoutAreas.FirstOrDefault(x =>
                x.Type == HideoutAreas.EmergencyWall
            );

            // No collector or med station, skip
            if (waterCollector is null && medStation is null)
            {
                return;
            }

            // If med-station > level 1 AND water collector > level 1 AND wall is level 0
            if (waterCollector?.Level >= 1 && medStation?.Level >= 1 && wall?.Level <= 0)
            {
                wall.Level = 3;
            }
        }

        /// <summary>
        ///     Does the provided profile contain any condition counters
        /// </summary>
        /// <param name="profile"> Profile to check for condition counters </param>
        /// <returns> Profile has condition counters </returns>
        public static bool ProfileHasConditionCounters(this PmcData profile)
        {
            if (profile.TaskConditionCounters is null)
            {
                return false;
            }

            return profile.TaskConditionCounters.Count > 0;
        }

        /// <summary>
        ///     Get a specific common skill from supplied profile
        /// </summary>
        /// <param name="profile">Player profile</param>
        /// <param name="skill">Skill to look up and return value from</param>
        /// <returns>Common skill object from desired profile</returns>
        public static CommonSkill? GetSkillFromProfile(this PmcData profile, SkillTypes skill)
        {
            return profile?.Skills?.Common?.FirstOrDefault(s => s.Id == skill);
        }
    }
}
