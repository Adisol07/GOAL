namespace GOAL;

public class Parser
{
    private List<Lextoken> Lextokens = new List<Lextoken>();

    public Parser(List<Lextoken> lextokens)
    {
        Lextokens = lextokens;
    }

    public List<Parsetoken> Parse()
    {
        List<Parsetoken> tokens = new List<Parsetoken>();

        return tokens;
    }
}
