using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAL.Parsetokens;

public class FunctionCall : IParsetoken
{
    public Lextoken Name { get; set; } = null!;
    public List<Lextoken> Args { get; set; } = new List<Lextoken>();

    public FunctionCall()
    { }
    public FunctionCall(Lextoken name, List<Lextoken> args)
    {
        Name = name;
        Args = args;
    }

    public string[] Prepare(Interpreter interpreter)
    {
        string[] args = new string[Args.Count];
        for (int i = 0; i < Args.Count; i++)
        {
            if (Args[i].Type == LextokenType.Identifier)
            {
                args[i] = interpreter.Variables[Args[i].Value].ToString();
            }
            else
            {
                args[i] = Args[i].Value;
            }
        }
        return args;
    }
}
