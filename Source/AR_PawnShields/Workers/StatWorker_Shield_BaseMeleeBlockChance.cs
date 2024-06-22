using Verse;

namespace AR_PawnShields;

public class StatWorker_Shield_BaseMeleeBlockChance : StatWorker_Shield
{
    protected override bool IsDisabledForShield(CompProperties_Shield shieldProps)
    {
        return !shieldProps.canBlockMelee;
    }

    protected override string GetDisabledExplanation()
    {
        return "ShieldBlockMeleeNever".Translate();
    }
}