namespace GOAL;

public class Lexer
{
    private string raw;
    private int current;

    public Lexer(string raw)
    {
        this.raw = raw;
        current = 0;
    }

    public List<Lextoken> Tokenize()
    {
        List<Lextoken> tokens = new List<Lextoken>();

        string buff = "";
        LextokenType type = LextokenType.Undefined;
        while (current < raw.Length)
        {
            char c = consume();

            if (char.IsNumber(c))
            {
                type = LextokenType.Number;
                buff += c;
            }
            else if ((type == LextokenType.Undefined && char.IsLetter(c)) ||
                    (type == LextokenType.Identifier && char.IsLetterOrDigit(c)))
            {
                type = LextokenType.Identifier;
                buff += c;
            }
            else if (c == '=' && type == LextokenType.Undefined)
            {
                type = LextokenType.Assignment;
                buff += c;
            }
            else if (c == '=' && type == LextokenType.Assignment)
            {
                type = LextokenType.BinaryOperator;
                buff += c;
            }
            else if (char.IsWhiteSpace(c))
            {
                tokens.Add(new Lextoken(buff, type));
                buff = "";
                type = LextokenType.Undefined;
            }
            else
            {
                throw new Exception($"Undefined token type. (\"{buff}\")");
            }
        }
        if (type != LextokenType.Undefined)
        {
            tokens.Add(new Lextoken(buff, type));
            buff = "";
            type = LextokenType.Undefined;
        }

        return tokens;
    }

    private char peek(int ahead = 0) => raw[current + ahead];
    private char consume() => raw[current++];
}
