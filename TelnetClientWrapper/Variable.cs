using System;
namespace IsengardClient
{
    public class Variable
    {
        public Variable()
        {
        }

        public static Variable CopyVariable(Variable copied)
        {
            Variable ret;
            switch (copied.Type)
            {
                case VariableType.Bool:
                    ret = new BooleanVariable();
                    ((BooleanVariable)ret).Value = ((BooleanVariable)copied).Value;
                    break;
                case VariableType.Int:
                    ret = new IntegerVariable();
                    ((IntegerVariable)ret).Value = ((IntegerVariable)copied).Value;
                    break;
                case VariableType.String:
                    ret = new StringVariable();
                    ((StringVariable)ret).Value = ((StringVariable)copied).Value;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            ret.Name = copied.Name;
            ret.Type = copied.Type;
            return ret;
        }

        public string Name { get; set; }
        public VariableType Type { get; set; }
    }

    public class BooleanVariable : Variable
    {
        public bool Value { get; set; }
    }

    public class IntegerVariable : Variable
    {
        public int Value { get; set; }
    }

    public class StringVariable : Variable
    {
        public string Value { get; set; }
    }

    public enum VariableType
    {
        Bool,
        Int,
        String,
    }
}
