namespace GOAL;

public class Goal
{
    private Lexer? lexer;
    private Parser? parser;
    private Interpreter? interpreter;

    public string Name { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string Raw { get; set; } = null!;
    public DateTime Created { get; set; } = DateTime.MinValue;
    public string Status { get; set; } = null!;

    public Goal()
    { }

    public async Task Run()
    {
        await interpreter!.Execute(parser!.Parse());
    }

    public static Goal Load(string fileName)
    {
        Goal goal = new Goal();

        goal.Name = Path.GetFileNameWithoutExtension(fileName);
        goal.FileName = fileName;
        goal.Raw = File.ReadAllText(fileName);
        goal.Created = File.GetCreationTime(fileName);
        goal.Status = "Unknown";

        goal.lexer = new Lexer(goal.Raw);
        goal.parser = new Parser(goal.lexer.Tokenize());
        goal.interpreter = new Interpreter();

        return goal;
    }
}
