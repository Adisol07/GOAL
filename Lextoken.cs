namespace GOAL;

public class Lextoken
{
    public string Value { get; set; } = null!;
    public LextokenType Type { get; set; } = LextokenType.Undefined;

    public Lextoken()
    { }
    public Lextoken(string value, LextokenType type)
    {
        Value = value;
        Type = type;
    }
}

public enum LextokenType
{
    Undefined = -1,
    Number = 0,
    Identifier = 1,
    Assignment = 2,
    BinaryOperator = 3,
}
