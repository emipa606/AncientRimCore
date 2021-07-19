using System;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace AncientRimShields
{
    public struct LocalVar : IEquatable<LocalVar>
    {
        private readonly LocalVariableInfo local;

        public bool IsPinned => local.IsPinned;

        public int LocalIndex => local.LocalIndex;

        public Type LocalType => local.LocalType;

        internal LocalVar(LocalVariableInfo local)
        {
            this.local = local;
        }

        public CodeInstruction ToLdloc()
        {
            // ILGenerator.Emit(OpCodes.Ldloc, LocalBuilder) automatically emits the proper opcode and operand.
            if (local is LocalBuilder localBuilder)
            {
                return new CodeInstruction(OpCodes.Ldloc, localBuilder);
            }

            var index = LocalIndex;
            switch (index)
            {
                case 0:
                    return new CodeInstruction(OpCodes.Ldloc_0);
                case 1:
                    return new CodeInstruction(OpCodes.Ldloc_1);
                case 2:
                    return new CodeInstruction(OpCodes.Ldloc_2);
                case 3:
                    return new CodeInstruction(OpCodes.Ldloc_3);
                default:
                    if (index <= byte.MaxValue)
                    {
                        return new CodeInstruction(OpCodes.Ldloc_S, (byte) index);
                    }
                    else
                    {
                        return new CodeInstruction(OpCodes.Ldloc, (short) index);
                    }
            }
        }

        public CodeInstruction ToLdloca()
        {
            // ILGenerator.Emit(OpCodes.Ldloca, LocalBuilder) automatically emits the proper opcode and operand.
            if (local is LocalBuilder localBuilder)
            {
                return new CodeInstruction(OpCodes.Ldloca, localBuilder);
            }

            var index = LocalIndex;
            if (index <= byte.MaxValue)
            {
                return new CodeInstruction(OpCodes.Ldloca_S, (byte) index);
            }

            return new CodeInstruction(OpCodes.Ldloca, (short) index);
        }

        public CodeInstruction ToStloc()
        {
            // ILGenerator.Emit(OpCodes.Stloc, LocalBuilder) automatically emits the proper opcode and operand.
            if (local is LocalBuilder localBuilder)
            {
                return new CodeInstruction(OpCodes.Stloc, localBuilder);
            }

            var index = LocalIndex;
            switch (index)
            {
                case 0:
                    return new CodeInstruction(OpCodes.Stloc_0);
                case 1:
                    return new CodeInstruction(OpCodes.Stloc_1);
                case 2:
                    return new CodeInstruction(OpCodes.Stloc_2);
                case 3:
                    return new CodeInstruction(OpCodes.Stloc_3);
                default:
                    if (index <= byte.MaxValue)
                    {
                        return new CodeInstruction(OpCodes.Stloc_S, (byte) index);
                    }
                    else
                    {
                        return new CodeInstruction(OpCodes.Stloc, (short) index);
                    }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is LocalVar other && LocalIndex == other.LocalIndex;
        }

        public bool Equals(LocalVar other)
        {
            return LocalIndex == other.LocalIndex;
        }

        public static bool operator ==(LocalVar lhs, LocalVar rhs)
        {
            return lhs.LocalIndex == rhs.LocalIndex;
        }

        public static bool operator !=(LocalVar lhs, LocalVar rhs)
        {
            return lhs.LocalIndex != rhs.LocalIndex;
        }

        public override int GetHashCode()
        {
            return LocalIndex;
        }

        public override string ToString()
        {
            return local.ToString();
        }
    }
}