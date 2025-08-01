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
            else if (t.Type == LextokenType.Keyword &&
                    (t.Value == "if" || t.Value == "elseif" || t.Value == "else"))
            {
                Lextoken type = consume();
                List<Lextoken> condition = new List<Lextoken>();
                if (t.Value != "else")
                {
                    consume();
                    while (peek().Type != LextokenType.OpenParenthesis && peek().Value != "{")
                    {
                        condition.Add(consume());
                    }
                    condition.RemoveAt(condition.Count - 1);
                }
                List<Lextoken> selection_tokens = new List<Lextoken>();
                consume();
                int open_brackets = 1;
                while (open_brackets > 0)
                {
                    if (peek().Type == LextokenType.OpenParenthesis && peek().Value == "{")
                    {
                        open_brackets++;
                    }
                    if (peek().Type == LextokenType.ClosedParenthesis && peek().Value == "}")
                    {
                        open_brackets--;
                    }

                    selection_tokens.Add(consume());
                }

                Parser selection_parser = new Parser(selection_tokens);
                List<IParsetoken> selection = selection_parser.Parse();

                if (type.Value == "if")
                {
                    tokens.Add(new Parsetokens.Selection(condition, selection));
                }
                else if (type.Value == "elseif")
                {
                    Parsetokens.Selection last_selection = (Parsetokens.Selection)tokens.Last();
                    last_selection.ElseIfs.Add(new Parsetokens.Selection(condition, selection));
                }
                else if (type.Value == "else")
                {
                    Parsetokens.Selection last_selection = (Parsetokens.Selection)tokens.Last();
                    last_selection.Else = new Parsetokens.Selection(condition, selection);
                }
            }
            else if (t.Type == LextokenType.Keyword &&
                     t.Value == "for")
            {
                consume();
                consume();
                List<Lextoken> declaration = new List<Lextoken>();
                List<Lextoken> condition = new List<Lextoken>();
                List<Lextoken> change = new List<Lextoken>();

                while (peek().Type != LextokenType.EndStatement)
                {
                    declaration.Add(consume());
                }
                declaration.Add(consume());
                while (peek().Type != LextokenType.EndStatement)
                {
                    condition.Add(consume());
                }
                consume();
                while (peek().Type != LextokenType.OpenParenthesis && peek().Value != "{")
                {
                    change.Add(consume());
                }
                change.RemoveAt(change.Count - 1);
                change.Add(new Lextoken(";", LextokenType.EndStatement));
                List<Lextoken> for_tokens = new List<Lextoken>();
                consume();
                int open_brackets = 1;
                while (open_brackets > 0)
                {
                    if (peek().Type == LextokenType.OpenParenthesis && peek().Value == "{")
                    {
                        open_brackets++;
                    }
                    if (peek().Type == LextokenType.ClosedParenthesis && peek().Value == "}")
                    {
                        open_brackets--;
                    }

                    for_tokens.Add(consume());
                }

                Parser for_decl_parser = new Parser(declaration);
                List<IParsetoken> decl = for_decl_parser.Parse();
                Parser for_change_parser = new Parser(change);
                List<IParsetoken> chng = for_change_parser.Parse();
                Parser for_parser = new Parser(for_tokens);
                List<IParsetoken> for_parsetokens = for_parser.Parse();

                tokens.Add(new Parsetokens.For(decl, condition, chng, for_parsetokens));
            }
            else
            {
                consume();
            }
        }

        return tokens;
    }

    public Lextoken peek(int ahead = 0)
    {
        if (current + ahead >= lextokens.Count)
            throw new Exception("Error while parsing. Limit: \"" + lextokens.Count + "\", requested: \"" + (current + ahead) + "\"");

        return lextokens[current + ahead];
    }
    public Lextoken consume()
    {
        if (current >= lextokens.Count)
            throw new Exception("Error while parsing. Limit: \"" + lextokens.Count + "\", requested: \"" + current + "\"");

        return lextokens[current++];
    }
}
