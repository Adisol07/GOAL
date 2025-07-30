using Newtonsoft.Json;

namespace GOAL;

public class Parser
{
    private List<Lextoken> lextokens = new List<Lextoken>();
    private int current = 0;

    public Parser(List<Lextoken> lextokens)
    {
        this.lextokens = lextokens;
        current = 0;
    }

    public List<IParsetoken> Parse()
    {
        current = 0;
        List<IParsetoken> tokens = new List<IParsetoken>();

        while (current < lextokens.Count)
        {
            Lextoken t = peek();

            if (t.Type == LextokenType.Identifier && 
               (peek(1).Type == LextokenType.Assignment || 
               peek(1).Type == LextokenType.MathOperator))
            {
                Lextoken name = consume();
                Lextoken assignment_type = consume();
                List<Lextoken> statement = new List<Lextoken>();
                while (peek().Type != LextokenType.EndStatement)
                {
                    statement.Add(consume());
                }
                consume();
                tokens.Add(new Parsetokens.VariableAssignment(name, assignment_type, statement));
            }
            else if (t.Type == LextokenType.Identifier &&
                     peek(1).Type == LextokenType.OpenParenthesis)
            {
                Lextoken name = consume();
                List<List<Lextoken>> args = new List<List<Lextoken>>();
                args.Add(new List<Lextoken>());
                int current_arg = 0;
                consume();
                while (peek().Type != LextokenType.EndStatement)
                {
                    Lextoken at = consume();
                    if (at.Type == LextokenType.Punctuation)
                    {
                        current_arg++;
                        args.Add(new List<Lextoken>());
                        continue;
                    }

                    args[current_arg].Add(at);
                }
                args.Last().RemoveAt(args.Last().Count - 1);
                consume();
                tokens.Add(new Parsetokens.FunctionCall(name, args));
            }
            else
            {
                consume();
            }
        }

        return tokens;
    }

    public Lextoken peek(int ahead = 0) => lextokens[current + ahead];
    public Lextoken consume() => lextokens[current++];
}
