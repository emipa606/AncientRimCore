using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace AR_PawnShields;

public class Locals(MethodBase method, ILGenerator ilGenerator)
{
    private readonly IList<LocalVariableInfo> locals = method.GetMethodBody()?.LocalVariables;

    public LocalVar FromLdloc(CodeInstruction instruction)
    {
        if (IsLdloc(instruction, out var local))
        {
            return local;
        }

        throw new ArgumentException("Expected ldloc-type instruction, actual instruction: " + instruction);
    }

    public bool IsLdlocOrLdloca(CodeInstruction instruction)
    {
        return instruction.IsLdloc();
    }

    public bool IsLdloc(CodeInstruction instruction)
    {
        return
            instruction.opcode == OpCodes.Ldloc ||
            instruction.opcode == OpCodes.Ldloc_S ||
            instruction.opcode == OpCodes.Ldloc_0 ||
            instruction.opcode == OpCodes.Ldloc_1 ||
            instruction.opcode == OpCodes.Ldloc_2 ||
            instruction.opcode == OpCodes.Ldloc_3;
    }

    public bool IsLdloc(CodeInstruction instruction, out LocalVar local)
    {
        if (instruction.opcode == OpCodes.Ldloc || instruction.opcode == OpCodes.Ldloc_S)
        {
            local = new LocalVar(
                (LocalVariableInfo)instruction.operand); // note: LocalBuilder derives from LocalVariableInfo
        }
        else if (instruction.opcode == OpCodes.Ldloc_0)
        {
            local = new LocalVar(locals[0]);
        }
        else if (instruction.opcode == OpCodes.Ldloc_1)
        {
            local = new LocalVar(locals[1]);
        }
        else if (instruction.opcode == OpCodes.Ldloc_2)
        {
            local = new LocalVar(locals[2]);
        }
        else if (instruction.opcode == OpCodes.Ldloc_3)
        {
            local = new LocalVar(locals[3]);
        }
        else
        {
            local = default;
            return false;
        }

        return true;
    }

    public bool IsLdloc(CodeInstruction instruction, LocalVar local)
    {
        return IsLdloc(instruction, out var otherLocal) && local == otherLocal;
    }

    public LocalVar FromLdloca(CodeInstruction instruction)
    {
        if (IsLdloca(instruction, out var local))
        {
            return local;
        }

        throw new ArgumentException("Expected ldloca-type instruction, actual instruction: " + instruction);
    }

    public bool IsLdloca(CodeInstruction instruction)
    {
        return instruction.opcode == OpCodes.Ldloca || instruction.opcode == OpCodes.Ldloca_S;
    }

    public bool IsLdloca(CodeInstruction instruction, out LocalVar local)
    {
        if (instruction.opcode == OpCodes.Ldloca || instruction.opcode == OpCodes.Ldloca_S)
        {
            local = new LocalVar(
                (LocalVariableInfo)instruction.operand); // note: LocalBuilder derives from LocalVariableInfo
            return true;
        }

        local = default;
        return false;
    }

    public bool IsLdloca(CodeInstruction instruction, LocalVar local)
    {
        return IsLdloca(instruction, out var otherLocal) && local == otherLocal;
    }

    public LocalVar FromStloc(CodeInstruction instruction)
    {
        if (IsStloc(instruction, out var local))
        {
            return local;
        }

        throw new ArgumentException("Expected stloc-type instruction, actual instruction: " + instruction);
    }

    public bool IsStloc(CodeInstruction instruction)
    {
        return instruction.IsStloc();
    }

    public bool IsStloc(CodeInstruction instruction, out LocalVar local)
    {
        if (instruction.opcode == OpCodes.Stloc || instruction.opcode == OpCodes.Stloc_S)
        {
            local = new LocalVar(
                (LocalVariableInfo)instruction.operand); // note: LocalBuilder derives from LocalVariableInfo
        }
        else if (instruction.opcode == OpCodes.Stloc_0)
        {
            local = new LocalVar(locals[0]);
        }
        else if (instruction.opcode == OpCodes.Stloc_1)
        {
            local = new LocalVar(locals[1]);
        }
        else if (instruction.opcode == OpCodes.Stloc_2)
        {
            local = new LocalVar(locals[2]);
        }
        else if (instruction.opcode == OpCodes.Stloc_3)
        {
            local = new LocalVar(locals[3]);
        }
        else
        {
            local = default;
            return false;
        }

        return true;
    }

    public bool IsStloc(CodeInstruction instruction, LocalVar local)
    {
        return IsStloc(instruction, out var otherLocal) && local == otherLocal;
    }

    public LocalVar DeclareLocal(Type localType)
    {
        return new LocalVar(ilGenerator.DeclareLocal(localType));
    }

    public LocalVar DeclareLocal<T>()
    {
        return new LocalVar(ilGenerator.DeclareLocal(typeof(T)));
    }
}