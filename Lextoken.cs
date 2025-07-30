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
    MathOperator = 4,
    OpenParenthesis = 5,
    ClosedParenthesis = 6,
    Keyword = 7,
    String = 8,
    EndStatement = 9,
    True = 10,
    False = 11,
    Punctuation = 12,
}
