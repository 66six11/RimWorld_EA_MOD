using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace archive.newcentury
{
    // ===== 组件属性类 =====
    public class HediffCompProperties_HaloBoost : HediffCompProperties
    {
        public List<HaloLevelConfig> levels = new List<HaloLevelConfig>();

        public HediffCompProperties_HaloBoost()
        {
            this.compClass = typeof(HediffComp_HaloBoost);
        }
    }

    public class HaloLevelConfig
    {
        public float healthOffset = 0f;
        public float armorOffsetSharp = 0f;   // 替换 Sharp
        public float armorOffsetBlunt = 0f;  // 替换 Blunt
    }

    // ===== 状态主类 =====
    public class Hediff_HaloEnhancement : HediffWithComps
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            ApplyBonuses();
        }

        private void ApplyBonuses()
        {
            var comp = this.TryGetComp<HediffComp_HaloBoost>();
            comp?.ApplyBonuses();
        }
    }

    // ===== 组件类 =====
    public class HediffComp_HaloBoost : HediffComp
    {
        public HediffCompProperties_HaloBoost Props => (HediffCompProperties_HaloBoost)this.props;

        private const string HaloDefName = "Halo";

        public override void CompPostMake()
        {
            base.CompPostMake();
            ApplyBonuses();
        }

        public void ApplyBonuses()
        {
            if (Pawn == null || Pawn.health == null || Pawn.health.hediffSet == null)
                return;

            var haloDef = DefDatabase<HediffDef>.GetNamedSilentFail(HaloDefName);
            if (haloDef == null)
            {
                Log.Error($"[HaloBoost] Required HediffDef '{HaloDefName}' not found.");
                return;
            }

            var haloPart = Pawn.health.hediffSet.GetFirstHediffOfDef(haloDef);
            if (haloPart == null)
                return;

            int level = Math.Max(1, Math.Min(Props.levels.Count, SeverityToInt()));

            if (level < 1 || level > Props.levels.Count)
            {
                Log.Warning($"[HaloBoost] Level {level} out of range for {Pawn.Name}'s Halo enhancement.");
                return;
            }

            var config = Props.levels[level - 1];

            // 避免不断叠加造成严重度失控
            haloPart.Severity = config.healthOffset;

            Log.Message($"[HaloBoost] Applied Level {level} to {Pawn.Name}: {config.healthOffset} Health");
        }

        private int SeverityToInt()
        {
            float severity = this.parent?.Severity ?? 0f;
            return (int)Math.Round(severity * 5); // 0~1 → 1~5
        }
    }
}