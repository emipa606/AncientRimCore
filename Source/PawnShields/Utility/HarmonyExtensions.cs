using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace AncientRimShields
{
    public static class HarmonyExtensions
    {
        public static void SafeInsertRange(this List<CodeInstruction> instructions, int insertionIndex,
            IEnumerable<CodeInstruction> newInstructions,
            IEnumerable<Label> labelsToTransfer = null, IEnumerable<ExceptionBlock> blocksToTransfer = null)
        {
            var origInstruction = instructions[insertionIndex];
            instructions.InsertRange(insertionIndex, newInstructions);
            instructions[insertionIndex]
                .TransferLabelsAndBlocksFrom(origInstruction, labelsToTransfer, blocksToTransfer);
        }

        public static void SafeReplaceRange(this List<CodeInstruction> instructions, int insertionIndex, int count,
            IEnumerable<CodeInstruction> newInstructions,
            IEnumerable<Label> labelsToTransfer = null, IEnumerable<ExceptionBlock> blocksToTransfer = null)
        {
            var origInstruction = instructions[insertionIndex];
            instructions.ReplaceRange(insertionIndex, count, newInstructions);
            instructions[insertionIndex]
                .TransferLabelsAndBlocksFrom(origInstruction, labelsToTransfer, blocksToTransfer);
        }

        public static CodeInstruction TransferLabelsAndBlocksFrom(this CodeInstruction instruction,
            CodeInstruction otherInstruction,
            IEnumerable<Label> labelsToTransfer = null, IEnumerable<ExceptionBlock> blocksToTransfer = null)
        {
            if (labelsToTransfer == null)
            {
                instruction.labels.AddRange(otherInstruction.labels.PopAll());
            }
            else
            {
                instruction.labels.AddRange(labelsToTransfer.Where(otherInstruction.labels.Remove));
            }

            if (blocksToTransfer == null)
            {
                instruction.blocks.AddRange(otherInstruction.blocks.PopAll());
            }
            else
            {
                instruction.blocks.AddRange(blocksToTransfer.Where(otherInstruction.blocks.Remove));
            }

            return instruction;
        }

        public static bool IsBrfalse(this CodeInstruction instruction)
        {
            return instruction.opcode == OpCodes.Brfalse_S || instruction.opcode == OpCodes.Brfalse;
        }
    }
}