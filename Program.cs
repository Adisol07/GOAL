using Newtonsoft.Json;
using Spectre.Console;

namespace GOAL;

class Program
{
    static Config config = new Config();
    static string app_path = AppDomain.CurrentDomain.BaseDirectory;
    static string base_path = app_path;
    static string usr = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    static async Task Main(string[] args)
    {
        CheckIntegrity();
        // AnsiConsole.Write(new FigletText("GOAL").Centered().Color(Color.Green3_1));

        Lexer lexer = new Lexer(File.ReadAllText("$/a.goal".ReplacePath()));
        Console.WriteLine(JsonConvert.SerializeObject(lexer.Tokenize(), Formatting.Indented));
        Parser parser = new Parser(lexer.Tokenize());
        Console.WriteLine(JsonConvert.SerializeObject(parser.Parse(), Formatting.Indented));

        // await AnsiConsole.Progress()
        //     .AutoRefresh(true)
        //     .AutoClear(true)
        //     .StartAsync(async ctx =>
        //     {
        //         var task = ctx.AddTask("Downloading", maxValue: 100);
        //         while (!task.IsFinished)
        //         {
        //             task.Increment(1);
        //             await Task.Delay(250);
        //         }
        //     }
        // );
        //string p = AnsiConsole.Prompt<string>(new TextPrompt<string>(""));
    }

    static void CheckIntegrity()
    {
        if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
        {
            base_path = $"{usr}/.config/goal";
            if (!Directory.Exists(base_path))
            {
                Directory.CreateDirectory(base_path);
            }
        }

        if (!File.Exists(base_path + "/config.json"))
        {
            if (OperatingSystem.IsWindows())
            {
                config.GoalsPath = @"C:\Users\" + Environment.UserName + @"\Goals";
            }
            else if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
            {
                config.GoalsPath = $"{usr}/Goals";
            }
            File.WriteAllText(base_path + "/config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(base_path + "/config.json"))!;

        if (!Directory.Exists(config.GoalsPath.ReplacePath()))
        {
            Directory.CreateDirectory(config.GoalsPath.ReplacePath());
        }
    }
}
