using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAL.Parsetokens;

public class For : IParsetoken
{
    public List<IParsetoken> Declaration { get; set; } = new List<IParsetoken>();
    public List<Lextoken> Condition { get; set; } = new List<Lextoken>();
    public List<IParsetoken> Change { get; set; } = new List<IParsetoken>();
    public List<IParsetoken> Tokens { get; set; } = new List<IParsetoken>();

    public For()
    { }
    public For(List<IParsetoken> declaration, List<Lextoken> condition, List<IParsetoken> change, List<IParsetoken> tokens)
    {
        Declaration = declaration;
        Condition = condition;
        Change = change;
        Tokens = tokens;
    }

    public bool Evaluate(Interpreter interpreter) => 
        Expression.Evaluate(interpreter, Condition) == true;
}
