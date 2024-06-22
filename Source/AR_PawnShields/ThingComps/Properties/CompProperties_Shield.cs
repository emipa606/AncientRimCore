using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace AR_PawnShields;

/// <summary>
///     Shield properties.
/// </summary>
public class CompProperties_Shield : CompProperties
{
    /// <summary>
    ///     Determines whether the shield can be automatically discarded by the shield wielder or not.
    /// </summary>
    public readonly bool canBeAutoDiscarded = true;

    /// <summary>
    ///     Can the shield block ranged attacks?
    /// </summary>
    public readonly bool canBlockMelee = true;

    /// <summary>
    ///     Can the shield block melee attacks?
    /// </summary>
    public readonly bool canBlockRanged = true;

    /// <summary>
    ///     How much percent of the damage is converted to fatigue damage on a blocked attack.
    /// </summary>
    public readonly float damageToFatigueFactor = 0.05f;

    /// <summary>
    ///     Default blocking sound if no sounds are defined.
    /// </summary>
    public readonly SoundDef defaultSound = null;

    /// <summary>
    ///     The hit points percentage threshold when the shield is automatically discarded by the shield wielder.
    /// </summary>
    public readonly float healthAutoDiscardThreshold = 0.19f;

    [Obsolete("use the Shield_BaseMeleeBlockChance stat")]
    public readonly float meleeBlockChanceFactor = 1.0f;

    [Obsolete("use the Shield_BaseRangedBlockChance stat")]
    public readonly float rangedBlockChanceFactor = 0.5f;

    /// <summary>
    ///     Shield rendering properties.
    /// </summary>
    public readonly ShieldRenderProperties renderProperties = new ShieldRenderProperties();

    /// <summary>
    ///     Does the shield take damage when blocking?
    /// </summary>
    public readonly bool shieldTakeDamage = true;

    [Obsolete("use the Shield_DamageAbsorbed stat")]
    public readonly float shieldTakeDamageFactor = 0.8f;

    /// <summary>
    ///     The stuff the shield is made out of can be linked to here.
    /// </summary>
    public readonly List<StuffedSound> sounds = [];
    //SoundDefOf.MetalHitImportant;

    /// <summary>
    ///     Helps to fetch sounds easier.
    /// </summary>
    [Unsaved] public readonly Dictionary<StuffCategoryDef, SoundDef> stuffedSounds =
        new Dictionary<StuffCategoryDef, SoundDef>();

    /// <summary>
    ///     If true it will attempt to use a colored version of the shield. e.g. stuff
    /// </summary>
    public readonly bool useColoredVersion = true;

    /// <summary>
    ///     If true the shield wielder gets fatigued from a blocked attack.
    /// </summary>
    public readonly bool useFatigue = false;

    /// <summary>
    ///     Graphic when shield is worn.
    /// </summary>
    public GraphicData wieldedGraphic;

    public CompProperties_Shield()
    {
        compClass = typeof(CompShield);
    }

    public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
    {
        yield return GetStatDrawEntry("ShieldHitPointAutoDiscardThreshold", canBeAutoDiscarded,
            healthAutoDiscardThreshold, 2);
        yield return GetStatDrawEntry("ShieldDamageToFatigueFactor", useFatigue, damageToFatigueFactor, 1);
    }

    private static StatDrawEntry GetStatDrawEntry(string baseKey, bool enabled, float value,
        int displayPriorityWithinCategory)
    {
        var valueString = (enabled ? value : 0f).ToStringPercent();
        var reportText = $"{(baseKey + "Ex").Translate()}\n\n{"StatsReport_FinalValue".Translate()}: {valueString}";
        if (!enabled)
        {
            reportText += $" ({(baseKey + "Never").Translate()})";
        }

        return new StatDrawEntry(ShieldStatsDefOf.Shield, baseKey.Translate(), valueString, reportText,
            displayPriorityWithinCategory);
    }

    public override void ResolveReferences(ThingDef parentDef)
    {
        base.ResolveReferences(parentDef);

        // Setup stuffed sounds dictionary.
        if (sounds != null)
        {
            foreach (var stuffedSound in sounds)
            {
                stuffedSounds[stuffedSound.stuffCategory] = stuffedSound.sound;
            }
        }

        // Default missing stat bases to obsolete field values.
#pragma warning disable CS0618 // Type or member is obsolete
        SetDefaultStatBaseValue(parentDef, ShieldStatsDefOf.Shield_DamageAbsorbed, shieldTakeDamageFactor);
        // Note: Some existing mods use erroneous numbers like 10.0 or treat it like a percentage (0-100 rather than 0-1),
        // and never noticed any issues because the melee/rangedBlockChanceFactor fields never actually worked
        // (Shield_BaseMeleeBlockChance defaulted to 1 (100%) and Shield_BaseRangedBlockChance defaulted to 0.5 (50%),
        // so to prevent a sudden buff in block chance for pawns with poor melee skills, cap melee/RangedBlockChanceFactor values at 1 (100%).
        SetDefaultStatBaseValue(parentDef, ShieldStatsDefOf.Shield_BaseMeleeBlockChance,
            Mathf.Min(meleeBlockChanceFactor, 1.0f));
        SetDefaultStatBaseValue(parentDef, ShieldStatsDefOf.Shield_BaseRangedBlockChance,
            Mathf.Min(rangedBlockChanceFactor, 1.0f));
#pragma warning restore CS0618 // Type or member is obsolete
    }

    private static void SetDefaultStatBaseValue(ThingDef parentDef, StatDef statDef, float defaultBaseValue)
    {
        if (!parentDef.StatBaseDefined(statDef))
        {
            parentDef.SetStatBaseValue(statDef, defaultBaseValue);
        }
    }
}