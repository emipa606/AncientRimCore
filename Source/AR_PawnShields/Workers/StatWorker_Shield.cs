using RimWorld;
using Verse;

namespace AR_PawnShields;

// XXX: This should be an abstract class. It's only non-abstract for compatibility with stale bundled versions of PawnShields
// in other mods, and those stale versions have StatDefs that directly reference StatWorker_Shield.
// RimWorld as of version 1.1 has earlier loaded assemblies and later loaded XMLs have higher precedence, leading to a situation
// where a user can end up loading this version of PawnShields.dll and an outdated version of the StatDefs that still directly
// references StatWorker_Shield. So StatWorker_Shield must be a concrete class rather than an abstract one.
public class StatWorker_Shield : StatWorker
{
    public override void FinalizeValue(StatRequest req, ref float val, bool applyPostProcess)
    {
        if (IsDisabledForShield(req))
        {
            val = 0f;
        }
        else
        {
            base.FinalizeValue(req, ref val, applyPostProcess);
        }
    }

    public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
    {
        var text = base.GetExplanationFinalizePart(req, numberSense, finalVal);
        if (IsDisabledForShield(req))
        {
            text += $" ({GetDisabledExplanation()})";
        }

        return text;
    }

    // This is needed to prevent "base value" from being shown for disabled stats.
    public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
    {
        return IsDisabledForShield(req) ? "" : base.GetExplanationUnfinalized(req, numberSense);
    }

    public override bool ShouldShowFor(StatRequest req)
    {
        return req.Def is ThingDef def && def.HasComp(typeof(CompShield));
    }

    private bool IsDisabledForShield(StatRequest req)
    {
        if (req.Def is not ThingDef def)
        {
            return false;
        }

        var shieldProps = def.GetCompShieldProperties();
        return shieldProps != null && IsDisabledForShield(shieldProps);
    }

    protected virtual bool IsDisabledForShield(CompProperties_Shield shieldProps)
    {
        return false;
    }

    protected virtual string GetDisabledExplanation()
    {
        return "";
    }
}