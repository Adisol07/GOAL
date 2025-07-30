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
}
