using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AncientRimShields
{
    /// <summary>
    ///     Assists in generating shields for pawns.
    /// </summary>
    public static class PawnShieldGenerator
    {
        public static List<ThingStuffPair> allShieldPairs;
        public static List<ThingStuffPair> workingShields = new List<ThingStuffPair>();

        /*static PawnShieldGenerator()
        {
            //Initialise all shields.
            Reset();
        }*/

        /// <summary>
        ///     Tries to generate a shield for the pawn.
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="request"></param>
        public static void TryGenerateShieldFor(Pawn pawn, PawnGenerationRequest request)
        {
            //Shield stuff.
            var generatorProps =
                request.KindDef?.GetModExtension<ShieldPawnGeneratorProperties>();

            //Abort early if there is no mention at all of shield properties.
            if (generatorProps == null)
            {
                return;
            }

            workingShields = new List<ThingStuffPair>();

            //Initial filtering
            if (generatorProps.shieldTags.NullOrEmpty())
            {
                Log.Warning("PawnShields :: XML element shieldTags is null or empty for " + request.KindDef.defName);
                return;
            }

            if (!(pawn?.RaceProps?.ToolUser ?? false))
            {
                Log.Warning("PawnShields :: " + request.KindDef.defName +
                            " is not a ToolUser or Humanlike in RaceProps.");
                return;
            }

            if (!(pawn.health?.capacities.CapableOf(PawnCapacityDefOf.Manipulation) ?? false))
            {
                Log.Warning("PawnShields :: " + request.KindDef.defName + " is not capable of manipulation.");
                return;
            }

            if (pawn.story != null && pawn.WorkTagIsDisabled(WorkTags.Violent))
            {
                return;
            }

            var generatorPropsShieldMoney = generatorProps.shieldMoney;
            var randomInRange = generatorPropsShieldMoney.RandomInRange;
            if (allShieldPairs != null)
            {
                foreach (var w in allShieldPairs)
                {
                    if (!(w.Price <= randomInRange))
                    {
                        continue;
                    }

                    if (w.thing.weaponTags.NullOrEmpty())
                    {
                        continue;
                    }

                    if (!generatorProps.shieldTags.Any(tag =>
                        w.thing.weaponTags.Contains(tag)))
                    {
                        continue;
                    }

                    if (w.thing.generateAllowChance >= 1f ||
                        Rand.ValueSeeded(pawn.thingIDNumber ^ 28554824) <= w.thing.generateAllowChance)
                    {
                        workingShields.Add(w);
                    }
                }
            }

            if (workingShields.NullOrEmpty())
            {
                Log.Warning("No working shields found for " + pawn.Label + "::" + pawn.KindLabel);
                return;
            }

            if (!workingShields.TryRandomElementByWeight(w => w.Commonality * w.Price, out var thingStuffPair))
            {
                return;
            }

            var thingWithComps =
                (ThingWithComps) ThingMaker.MakeThing(thingStuffPair.thing, thingStuffPair.stuff);
            PawnGenerator.PostProcessGeneratedGear(thingWithComps, pawn);
            pawn.equipment?.AddEquipment(thingWithComps);
            //Log.Message(pawn.Label + " added shield " + thingWithComps.Label);
        }

        /// <summary>
        ///     Resets the shield generator.
        /// </summary>
        public static void Reset()
        {
            bool IsShield(ThingDef td)
            {
                return td.equipmentType != EquipmentType.Primary &&
                       td.HasComp(typeof(CompShield));
            }

            allShieldPairs = ThingStuffPair.AllWith(IsShield);

            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs.Where(IsShield))
            {
                var num = (from pa in allShieldPairs
                    where pa.thing == thingDef
                    select pa).Sum(pa => pa.Commonality);
                if (thingDef == null)
                {
                    continue;
                }

                var num2 = thingDef.generateCommonality / num;
                if (num2 == 1f)
                {
                    continue;
                }

                for (var i = 0; i < allShieldPairs.Count; i++)
                {
                    var thingStuffPair = allShieldPairs[i];
                    if (thingStuffPair.thing == thingDef)
                    {
                        allShieldPairs[i] = new ThingStuffPair(thingStuffPair.thing, thingStuffPair.stuff,
                            thingStuffPair.commonalityMultiplier * num2);
                    }
                }
            }
        }
    }
}