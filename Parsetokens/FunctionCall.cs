using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAL.Parsetokens;

public class FunctionCall : IParsetoken
{
    public Lextoken Name { get; set; } = null!;
    public List<List<Lextoken>> Args { get; set; } = new List<List<Lextoken>>();

    public FunctionCall()
    { }
    public FunctionCall(Lextoken name, List<List<Lextoken>> args)
    {
        Name = name;
        Args = args;
    }

    public string[] Prepare(Interpreter interpreter)
    {
        string[] args = new string[Args.Count];
        for (int i = 0; i < Args.Count; i++)
        {
            dynamic eval = Expression.Evaluate(interpreter, Args[i]);
            args[i] = eval.ToString();
        }
        return args;
    }
}
