using System;
using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TowerRangedWeaponMultiplier
{
    [StaticConstructorOnStartup]
    public class Fixes
    {
        [HarmonyPatch(typeof(ProjectileProperties), "GetDamageAmount",new Type[]{typeof(float),typeof(Thing),typeof(StringBuilder)})]
        private static class FixOne
        {
            [HarmonyPrefix]
            private static void Prefix(ref float damageMultiplier, Thing weapon)
            {
                if (weapon == null) return;
                Pawn_ApparelTracker apparelTracker = (weapon.ParentHolder as Pawn_EquipmentTracker)?.pawn?.apparel;
                if (apparelTracker != null)
                {
                    foreach (var apparel in apparelTracker.WornApparel)
                    {
                        damageMultiplier += apparel.GetStatValue(StatDef.Named("BANW_RangedWeapon_Damage"));
                    }
                }

                Pawn_EquipmentTracker equipmentTracker = (weapon.ParentHolder as Pawn_EquipmentTracker)?.pawn?.traits;
                if (equipmentTracker != null)
                {
                    Pawn pawn = equipmentTracker.pawn;
                    if (pawn != null && pawn.story != null && pawn.story.traits != null)
                    {
                        foreach (var trait in pawn.story.traits.allTraits)
                        {
                            damageMultiplier += trait.GetStatfactor(StatDef.Named("BANW_RangedWeapon_Damage_Trait"));
                        }
                    }
                }
            }
        }
    }
}
