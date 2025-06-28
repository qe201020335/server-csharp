using SPTarkov.Server.Core.Models.Eft.Profile;

namespace SPTarkov.Server.Core.Extensions
{
    public static class FullProfileExtensions
    {
        public static void StoreHydrationEnergyTempInProfile(
            this SptProfile fullProfile,
            double hydration,
            double energy,
            double temperature
        )
        {
            fullProfile.VitalityData.Hydration = hydration;
            fullProfile.VitalityData.Energy = energy;
            fullProfile.VitalityData.Temperature = temperature;
        }
    }
}
