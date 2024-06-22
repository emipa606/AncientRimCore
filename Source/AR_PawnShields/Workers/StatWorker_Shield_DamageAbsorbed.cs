using Verse;

namespace AR_PawnShields;

public class StatWorker_Shield_DamageAbsorbed : StatWorker_Shield
{
    protected override bool IsDisabledForShield(CompProperties_Shield shieldProps)
    {
        return !shieldProps.shieldTakeDamage;
    }

    protected override string GetDisabledExplanation()
    {
        return "ShieldAbsorbDamageNever".Translate();
    }
}