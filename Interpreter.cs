using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOAL.Parsetokens;

namespace GOAL;

public class Interpreter
{
    private int current = 0;

    public static Dictionary<string, Func<string[], dynamic[]>> Functions { get; set; } = new Dictionary<string, Func<string[], dynamic[]>>();

    public List<IParsetoken> Instructions { get; private set; } = new List<IParsetoken>();
    public Dictionary<string, dynamic> Variables { get; set; } = new Dictionary<string, dynamic>();

    public Interpreter()
    { 
        if (!Functions.ContainsKey("print"))
        {
            Functions.Add("print", (args) =>
            {
                Console.WriteLine(args[0]);
                return null!;
            });
        }
    }

    public async Task Execute(List<IParsetoken> tokens)
    {
        Instructions = tokens;
        current = 0;

        while (current < tokens.Count)
        {
            IParsetoken instruction = tokens[current++];

            if (instruction is VariableAssignment varass)
            {
                if (!Variables.ContainsKey(varass.Name.Value))
                    Variables.Add(varass.Name.Value, null!);

                switch (varass.Type.Value)
                {
                    case "=":
                        Variables[varass.Name.Value] = varass.Evaluate(this);
                        break;
                    case "+=":
                        Variables[varass.Name.Value] += varass.Evaluate(this);
                        break;
                    case "-=":
                        Variables[varass.Name.Value] -= varass.Evaluate(this);
                        break;
                    default:
                        throw new Exception($"Invalid assignment operator.");
                }
            }
            else if (instruction is FunctionCall funcall)
            {
                dynamic result = Functions[funcall.Name.Value].Invoke(funcall.Prepare(this));
            }

            await Task.Delay(1);
        }
    }
}
