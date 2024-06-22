using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace AR_PawnShields;

// Code copied between JecsTools and PawnShields - keep them in sync!
public static class HarmonyExtensions
{
    public static void SafeInsertRange(this List<CodeInstruction> instructions, int insertionIndex,
        IEnumerable<CodeInstruction> newInstructions,
        IEnumerable<Label> labelsToTransfer = null, IEnumerable<ExceptionBlock> blocksToTransfer = null)
    {
        var origInstruction = instructions[insertionIndex];
        instructions.InsertRange(insertionIndex, newInstructions);
        instructions[insertionIndex].TransferLabelsAndBlocksFrom(origInstruction, labelsToTransfer, blocksToTransfer);
    }

    public static void SafeReplaceRange(this List<CodeInstruction> instructions, int insertionIndex, int count,
        IEnumerable<CodeInstruction> newInstructions,
        IEnumerable<Label> labelsToTransfer = null, IEnumerable<ExceptionBlock> blocksToTransfer = null)
    {
        var origInstruction = instructions[insertionIndex];
        instructions.ReplaceRange(insertionIndex, count, newInstructions);
        instructions[insertionIndex].TransferLabelsAndBlocksFrom(origInstruction, labelsToTransfer, blocksToTransfer);
    }

    public static void TransferLabelsAndBlocksFrom(this CodeInstruction instruction,
        CodeInstruction otherInstruction,
        IEnumerable<Label> labelsToTransfer = null, IEnumerable<ExceptionBlock> blocksToTransfer = null)
    {
        instruction.labels.AddRange(labelsToTransfer == null
            ? otherInstruction.labels.PopAll()
            : labelsToTransfer.Where(otherInstruction.labels.Remove));

        instruction.blocks.AddRange(blocksToTransfer == null
            ? otherInstruction.blocks.PopAll()
            : blocksToTransfer.Where(otherInstruction.blocks.Remove));
    }

    public static bool IsBrfalse(this CodeInstruction instruction)
    {
        return instruction.opcode == OpCodes.Brfalse_S || instruction.opcode == OpCodes.Brfalse;
    }
}