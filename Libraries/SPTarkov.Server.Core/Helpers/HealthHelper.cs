using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;
using BodyPartHealth = SPTarkov.Server.Core.Models.Eft.Common.Tables.BodyPartHealth;

namespace SPTarkov.Server.Core.Helpers;

[Injectable]
public class HealthHelper(
    TimeUtil _timeUtil,
    SaveServer _saveServer,
    ProfileHelper _profileHelper,
    ConfigServer _configServer
)
{
    protected readonly HealthConfig _healthConfig = _configServer.GetConfig<HealthConfig>();

    /// <summary>
    ///     Resets the profiles vitality/health and vitality/effects properties to their defaults
    /// </summary>
    /// <param name="sessionID">Session Id</param>
    /// <returns>Updated profile</returns>
    public SptProfile ResetVitality(string sessionID)
    {
        var profile = _saveServer.GetProfile(sessionID);

        profile.VitalityData.SetDefaults();

        return profile;
    }

    /// <summary>
    ///     Update player profile vitality values with changes from client request object
    /// </summary>
    /// <param name="sessionID">Session id</param>
    /// <param name="pmcProfileToUpdate">Player profile to apply changes to</param>
    /// <param name="healthChanges">Changes to apply </param>
    /// <param name="isDead">OPTIONAL - Is player dead</param>
    public void ApplyHealthChangesToProfile(
        string sessionID,
        PmcData pmcProfileToUpdate,
        BotBaseHealth healthChanges,
        bool isDead = false
    )
    {
        var fullProfile = _saveServer.GetProfile(sessionID);
        var profileEdition = fullProfile.ProfileInfo.Edition;
        var profileSide = fullProfile.CharacterData.PmcData.Info.Side;

        // Get matching 'side' e.g. USEC
        var matchingSide = _profileHelper.GetProfileTemplateForSide(profileEdition, profileSide);

        var defaultTemperature =
            matchingSide?.Character?.Health?.Temperature ?? new CurrentMinMax { Current = 36.6 };

        fullProfile.StoreHydrationEnergyTempInProfile(
            healthChanges.Hydration.Current ?? 0,
            healthChanges.Energy.Current ?? 0,
            defaultTemperature.Current ?? 0 // Reset profile temp to the default to prevent very cold/hot temps persisting into next raid
        );

        // Store limb effects from post-raid in profile
        foreach (var bodyPart in healthChanges.BodyParts)
        {
            // Effects
            if (healthChanges.BodyParts[bodyPart.Key].Effects is not null)
            {
                fullProfile.VitalityData.Health[bodyPart.Key].Effects = healthChanges
                    .BodyParts[bodyPart.Key]
                    .Effects;
            }

            // Limb hp
            if (!isDead)
            // Player alive, not is limb alive
            {
                fullProfile.VitalityData.Health[bodyPart.Key].Health.Current =
                    healthChanges.BodyParts[bodyPart.Key].Health.Current ?? 0;
            }
            else
            {
                fullProfile.VitalityData.Health[bodyPart.Key].Health.Current =
                    pmcProfileToUpdate.Health.BodyParts[bodyPart.Key].Health.Maximum
                        * _healthConfig.HealthMultipliers.Death
                    ?? 0;
            }
        }
        // Alter saved profiles Health with values from post-raid client data
        ModifyProfileHeathProperties(
            healthChanges.BodyParts,
            pmcProfileToUpdate,
            ["Dehydration", "Exhaustion"]
        );

        // Adjust hydration/energy/temp and limb hp using temp storage hydrated above
        SaveHealth(pmcProfileToUpdate, sessionID);

        // Reset temp storage
        ResetVitality(sessionID);

        // Update last edited timestamp
        pmcProfileToUpdate.Health.UpdateTime = _timeUtil.GetTimeStamp();
    }

    /// <summary>
    ///     Apply Health values to profile
    /// </summary>
    /// <param name="bodyPartChanges">Changes to apply</param>
    /// <param name="profileToAdjust">Player profile on server</param>
    /// <param name="effectsToSkip"></param>
    protected void ModifyProfileHeathProperties(
        Dictionary<string, BodyPartHealth> bodyPartChanges,
        PmcData profileToAdjust,
        HashSet<string>? effectsToSkip = null
    )
    {
        foreach (var (partName, partProperties) in bodyPartChanges)
        {
            // Process each effect for each part
            foreach (var (key, effectDetails) in partProperties.Effects)
            {
                // Null guard
                var matchingProfilePart = profileToAdjust.Health.BodyParts[partName];
                matchingProfilePart.Effects ??= new Dictionary<string, BodyPartEffectProperties>();

                // Effect already exists on limb in server profile, skip
                if (matchingProfilePart.Effects.ContainsKey(key))
                {
                    // Edge case - effect already exists at destination, but we don't want to overwrite details
                    if (effectsToSkip is not null && effectsToSkip.Contains(key))
                    {
                        matchingProfilePart.Effects[key] = null;
                    }

                    continue;
                }

                if (effectsToSkip is not null && effectsToSkip.Contains(key))
                // Do not pass skipped effect into profile
                {
                    continue;
                }

                var effectToAdd = new BodyPartEffectProperties { Time = effectDetails.Time ?? -1 };
                // Add effect to server profile
                if (matchingProfilePart.Effects.TryAdd(key, effectToAdd))
                {
                    matchingProfilePart.Effects[key] = effectToAdd;
                }
            }
        }
    }

    /// <summary>
    ///     Adjust hydration/energy/temperate and body part hp values in player profile to values in `profile.vitality`
    /// </summary>
    /// <param name="pmcData">Profile to update</param>
    /// <param name="sessionID">Session id</param>
    protected void SaveHealth(PmcData pmcData, string sessionID)
    {
        if (!_healthConfig.Save.Health)
        {
            return;
        }

        var profileHealth = _saveServer.GetProfile(sessionID).VitalityData;

        if (profileHealth.Hydration > pmcData.Health.Hydration.Maximum)
        {
            profileHealth.Hydration = pmcData.Health.Hydration.Maximum;
        }

        if (profileHealth.Energy > pmcData.Health.Energy.Maximum)
        {
            profileHealth.Energy = pmcData.Health.Energy.Maximum;
        }

        if (profileHealth.Temperature > pmcData.Health.Temperature.Maximum)
        {
            profileHealth.Temperature = pmcData.Health.Temperature.Maximum;
        }

        pmcData.Health.Hydration.Current = Math.Round(profileHealth.Hydration ?? 0);
        pmcData.Health.Energy.Current = Math.Round(profileHealth.Energy ?? 0);
        pmcData.Health.Temperature.Current = Math.Round(profileHealth.Temperature ?? 0);

        foreach (var (partName, partProperties) in pmcData.Health.BodyParts)
        {
            var matchingProfilePart = profileHealth.Health[partName];
            if (matchingProfilePart.Health.Maximum > partProperties.Health.Maximum)
            {
                matchingProfilePart.Health.Maximum = partProperties.Health.Maximum;
            }

            if (matchingProfilePart.Health.Current == 0)
            {
                matchingProfilePart.Health.Current =
                    partProperties.Health.Maximum * _healthConfig.HealthMultipliers.Blacked;
            }

            partProperties.Health.Current = Math.Round(matchingProfilePart.Health.Current ?? 0);
        }
    }

    /// <summary>
    ///     Save effects to profile
    ///     Works by removing all effects and adding them back from profile
    ///     Removes empty 'Effects' objects if found
    /// </summary>
    /// <param name="pmcData">Player profile</param>
    /// <param name="sessionID">Session id</param>
    /// <param name="bodyPartsWithEffects">Dictionary of body parts with effects that should be added to profile</param>
    /// <param name="deleteExistingEffects">Should effects be added back to profile</param>
    protected void SaveEffects(
        PmcData pmcData,
        string sessionID,
        Dictionary<string, BodyPartHealth> bodyPartsWithEffects,
        bool deleteExistingEffects = true
    )
    {
        // TODO: this will need to change, typing is all fucked up
        if (!_healthConfig.Save.Effects)
        {
            return;
        }

        foreach (var bodyPart in bodyPartsWithEffects)
        {
            // clear effects from profile bodyPart
            if (deleteExistingEffects)
            {
                pmcData.Health.BodyParts[bodyPart.Key].Effects =
                    new Dictionary<string, BodyPartEffectProperties>();
            }

            foreach (var effectType in bodyPartsWithEffects[bodyPart.Key].Effects)
            {
                var time = effectType.Value.Time;
                if (time is not null && time > 0)
                {
                    AddEffect(pmcData, effectType, time);
                }
                else
                {
                    AddEffect(pmcData, effectType);
                }
            }
        }
    }

    /// <summary>
    ///     Add effect to body part in profile
    /// </summary>
    /// <param name="pmcData">Player profile</param>
    /// <param name="effectType">Effect to add to body part</param>
    /// <param name="duration">How long the effect has left in seconds (-1 by default, no duration).</param>
    protected void AddEffect(
        PmcData pmcData,
        KeyValuePair<string, BodyPartEffectProperties> effectType,
        double? duration = -1
    )
    {
        var profileBodyPart = pmcData.Health.BodyParts[effectType.Key];
        profileBodyPart.Effects ??= new Dictionary<string, BodyPartEffectProperties>();

        profileBodyPart.Effects[effectType.Key] = new BodyPartEffectProperties { Time = duration };
    }
}
