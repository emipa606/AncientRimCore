using Verse;

namespace AR_PawnShields;

public class StatWorker_Shield_BaseRangedBlockChance : StatWorker_Shield
{
    protected override bool IsDisabledForShield(CompProperties_Shield shieldProps)
    {
        return !shieldProps.canBlockRanged;
    }

    protected override string GetDisabledExplanation()
    {
        return "ShieldBlockRangedNever".Translate();
    }
}