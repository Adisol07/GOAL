using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAL.Parsetokens;

public class VariableAssignment : IParsetoken
{
    public Lextoken Name { get; set; } = null!;
    public Lextoken Type { get; set; } = null!;
    public List<Lextoken> Statement { get; set; } = new List<Lextoken>();

    public VariableAssignment()
    { }
    public VariableAssignment(Lextoken name, Lextoken type, List<Lextoken> statement)
    {
        Name = name;
        Type = type;
        Statement = statement;
    }

    public dynamic Evaluate(Interpreter interpreter)
    {
        dynamic result = evaluate_expr(interpreter, Statement);
        return result;
    }

    private static dynamic evaluate_expr(Interpreter interpreter, List<Lextoken> tokens)
    {
        var rpn = to_rpn(tokens);
        return evaluate_rpn(interpreter, rpn);
    }

    private static List<Lextoken> to_rpn(List<Lextoken> tokens)
    {
        var output = new List<Lextoken>();
        var stack = new Stack<Lextoken>();
        foreach (var t in tokens)
        {
            if (t.Type is LextokenType.Number or
                          LextokenType.String or
                          LextokenType.Identifier or
                          LextokenType.True or
                          LextokenType.False)
            {
                output.Add(t);
            }
            else if (t.Type == LextokenType.OpenParenthesis)
            {
                stack.Push(t);
            }
            else if (t.Type == LextokenType.ClosedParenthesis)
            {
                while (stack.Peek().Type != LextokenType.OpenParenthesis)
                {
                    output.Add(stack.Pop());
                }
                stack.Pop();
            }
            else
            {
                while (stack.Count > 0 && precedence(stack.Peek()) >= precedence(t))
                {
                    output.Add(stack.Pop());
                }
                stack.Push(t);
            }
        }
        while (stack.Count > 0) output.Add(stack.Pop());
        return output;
    }

    private static int precedence(Lextoken t) => t.Value switch
    {
        "!" => 4,
        "*" or "/" => 3,
        "+" or "-" => 2,
        "==" or "!=" or "<" or ">" or "<=" or ">=" => 1,
        "&&" => 0,
        "||" => -1,
        _ => 0
    };

    private static dynamic evaluate_rpn(Interpreter interpreter, List<Lextoken> rpn)
    {
        var stack = new Stack<dynamic>();
        foreach (var t in rpn)
        {
            if (t.Type is LextokenType.Number or 
                          LextokenType.String or 
                          LextokenType.Identifier or 
                          LextokenType.True or 
                          LextokenType.False)
            {
                stack.Push(t.Type switch
                {
                    LextokenType.Number => int.Parse(t.Value),
                    LextokenType.String => t.Value.Trim('"'),
                    LextokenType.Identifier => interpreter.Variables[t.Value],
                    LextokenType.True => true,
                    LextokenType.False => false,
                    _ => throw new Exception()
                });
            }
            else
            {
                if (t.Value == "!")
                {
                    var aa = stack.Pop();
                    stack.Push(!to_bool(aa));
                    continue;
                }
                var b = stack.Pop();
                var a = stack.Pop();
                stack.Push(t.Value switch
                {
                    "+" => a + b,
                    "-" => a - b,
                    "*" => a * b,
                    "/" => a / b,
                    "==" => a == b,
                    "!=" => a != b,
                    "<" => a < b,
                    ">" => a > b,
                    "<=" => a <= b,
                    ">=" => a >= b,
                    "&&" => to_bool(a) && to_bool(b),
                    "||" => to_bool(a) || to_bool(b),
                    _ => throw new NotSupportedException()
                });
            }
        }
        return stack.Pop();
    }

    private static bool to_bool(dynamic v) => v switch
    {
        bool b => b,
        0 => false,
        _ => true
    };
}
