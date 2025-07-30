namespace GOAL;

public class Lexer
{
    private string raw;
    private int current;

    public Lexer(string raw)
    {
        this.raw = raw.Trim();
        current = 0;
    }

    public List<Lextoken> Tokenize()
    {
        List<Lextoken> tokens = new List<Lextoken>();

        while (current < raw.Length)
        {
            char c = peek();

            if (char.IsDigit(c))
            {
                string buff = consume().ToString();
                while (char.IsDigit(peek()))
                {
                    buff += consume();
                }
                tokens.Add(new Lextoken(buff, LextokenType.Number));
            }
            else if (char.IsLetter(c))
            {
                string buff = consume().ToString();
                while (char.IsLetterOrDigit(peek()))
                {
                    buff += consume();
                }
                LextokenType type = (
                    is_keyword(buff) ? LextokenType.Keyword :
                                       LextokenType.Identifier);
                tokens.Add(new Lextoken(buff, type));
            }
            else if (c == '"')
            {
                consume();
                string buff = "";
                while (peek() != '"')
                {
                    buff += consume();
                }
                consume();
                tokens.Add(new Lextoken(buff, LextokenType.String));
            }
            else if (c == '=' && peek(1) != '=')
            {
                consume();
                tokens.Add(new Lextoken("=", LextokenType.Assignment));
            }
            else if (c == '=' && peek(1) == '=')
            {
                consume();
                consume();
                tokens.Add(new Lextoken("==", LextokenType.BinaryOperator));
            }
            else if (c == '+' || c == '-' || c == '*' || c == '/' || c == '%')
            {
                string buff = consume().ToString();
                if (peek() == '+' || peek() == '-' || peek() == '=')
                {
                    buff += consume();
                }
                tokens.Add(new Lextoken(buff, LextokenType.MathOperator));
            }
            else if (c == '(' || c == '[' || c == '{')
            {
                tokens.Add(new Lextoken(consume().ToString(), LextokenType.OpenParenthesis));
            }
            else if (c == ')' || c == ']' || c == '}')
            {
                tokens.Add(new Lextoken(consume().ToString(), LextokenType.ClosedParenthesis));
            }
            else if (char.IsWhiteSpace(c) || c == ';')
            {
                consume();
            }
            else
            {
                throw new Exception($"Syntax error (Undefined lex-token: \"{c}\")");
            }
        }

        return tokens;
    }

    private bool is_keyword(string value)
    {
        return value == "if" ||
               value == "else";
    }

    private char peek(int ahead = 0) => raw[current + ahead];
    private char consume() => raw[current++];
}
