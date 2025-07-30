using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAL.Parsetokens;

public class While : IParsetoken
{
    public List<Lextoken> Condition { get; set; } = new List<Lextoken>();
    public List<IParsetoken> Tokens { get; set; } = new List<IParsetoken>();

    public While()
    { }
    public While(List<Lextoken> condition, List<IParsetoken> tokens)
    {
        Condition = condition;
        Tokens = tokens;
    }

    public bool Evaluate(Interpreter interpreter) => 
        Expression.Evaluate(interpreter, Condition) == true;
}
