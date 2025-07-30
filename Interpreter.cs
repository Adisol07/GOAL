using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOAL.Parsetokens;
using Newtonsoft.Json;

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

            await ExecuteSingle(this, instruction);

            await Task.Delay(1);
        }
    }
    public async static Task ExecuteSingle(Interpreter interpreter, IParsetoken instruction)
    {
        if (instruction is VariableAssignment varass)
        {
            if (!interpreter.Variables.ContainsKey(varass.Name.Value))
                interpreter.Variables.Add(varass.Name.Value, null!);

            switch (varass.Type.Value)
            {
                case "=":
                    interpreter.Variables[varass.Name.Value] = varass.Evaluate(interpreter);
                    break;
                case "+=":
                    interpreter.Variables[varass.Name.Value] += varass.Evaluate(interpreter);
                    break;
                case "-=":
                    interpreter.Variables[varass.Name.Value] -= varass.Evaluate(interpreter);
                    break;
                default:
                    throw new Exception($"Invalid assignment operator.");
            }
        }
        else if (instruction is FunctionCall funcall)
        {
            dynamic result = Functions[funcall.Name.Value].Invoke(funcall.Prepare(interpreter));
        }
        else if (instruction is Selection sel)
        {
            if (sel.Evaluate(interpreter))
            {
                foreach (IParsetoken instr in sel.Tokens)
                {
                    await ExecuteSingle(interpreter, instr);
                }
            }
            else
            {
                bool found_elseif = false;
                foreach (Selection elseif in sel.ElseIfs)
                {
                    if (elseif.Evaluate(interpreter))
                    {
                        foreach (IParsetoken instr in elseif.Tokens)
                        {
                            await ExecuteSingle(interpreter, instr);
                        }
                        found_elseif = true;
                        break;
                    }
                }
                if (sel.Else != null && !found_elseif)
                {
                    foreach (IParsetoken instr in sel.Else.Tokens)
                    {
                        await ExecuteSingle(interpreter, instr);
                    }
                }
            }
        }
        else if (instruction is For frr)
        {
            foreach (IParsetoken instr in frr.Declaration)
            {
                await ExecuteSingle(interpreter, instr);
            }
            while (frr.Evaluate(interpreter))
            {
                foreach (IParsetoken instr in frr.Tokens)
                {
                    await ExecuteSingle(interpreter, instr);
                }
                foreach (IParsetoken instr in frr.Change)
                {
                    await ExecuteSingle(interpreter, instr);
                }
            }
        }
    }
}
