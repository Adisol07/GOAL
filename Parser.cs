namespace GOAL;

public class Parser
{
    private List<Lextoken> lextokens = new List<Lextoken>();
    private int current = 0;

    public Parser(List<Lextoken> lextokens)
    {
        this.lextokens = lextokens;
    }

    public List<Parsetoken> Parse()
    {
        List<Parsetoken> tokens = new List<Parsetoken>();

        while (current < lextokens.Count)
        {
            Lextoken t = peek();
        }

        return tokens;
    }

    public Lextoken peek(int ahead = 0) => lextokens[current + ahead];
    public Lextoken consume() => lextokens[current++];
}
