using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Profile;

namespace SPTarkov.Server.Core.Extensions
{
    public static class VitalityExtensions
    {
        public static void SetDefaults(this Vitality? vitality)
        {
            vitality ??= new Vitality
            {
                Health = null,
                Energy = 0,
                Temperature = 0,
                Hydration = 0,
            };

            vitality.Health = new Dictionary<string, BodyPartHealth>
            {
                {
                    "Head",
                    new BodyPartHealth
                    {
                        Health = new CurrentMinMax { Current = 0 },
                        Effects = new Dictionary<string, BodyPartEffectProperties>(),
                    }
                },
                {
                    "Chest",
                    new BodyPartHealth
                    {
                        Health = new CurrentMinMax { Current = 0 },
                        Effects = new Dictionary<string, BodyPartEffectProperties>(),
                    }
                },
                {
                    "Stomach",
                    new BodyPartHealth
                    {
                        Health = new CurrentMinMax { Current = 0 },
                        Effects = new Dictionary<string, BodyPartEffectProperties>(),
                    }
                },
                {
                    "LeftArm",
                    new BodyPartHealth
                    {
                        Health = new CurrentMinMax { Current = 0 },
                        Effects = new Dictionary<string, BodyPartEffectProperties>(),
                    }
                },
                {
                    "RightArm",
                    new BodyPartHealth
                    {
                        Health = new CurrentMinMax { Current = 0 },
                        Effects = new Dictionary<string, BodyPartEffectProperties>(),
                    }
                },
                {
                    "LeftLeg",
                    new BodyPartHealth
                    {
                        Health = new CurrentMinMax { Current = 0 },
                        Effects = new Dictionary<string, BodyPartEffectProperties>(),
                    }
                },
                {
                    "RightLeg",
                    new BodyPartHealth
                    {
                        Health = new CurrentMinMax { Current = 0 },
                        Effects = new Dictionary<string, BodyPartEffectProperties>(),
                    }
                },
            };
        }
    }
}
