using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace TowerRangedWeaponMultiplier
{
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public class PatchMain
    {
        static PatchMain()
        {
            Harmony harmony = new Harmony("TowerRangedWeaponMultiplier_HarmonyPatch");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}