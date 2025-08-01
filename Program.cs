using Newtonsoft.Json;
using Spectre.Console;

namespace GOAL;

class Program
{
    static Config config = new Config();
    static string app_path = AppDomain.CurrentDomain.BaseDirectory;
    static string base_path = app_path;
    static string usr = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    static Dictionary<string, Goal> goals = new Dictionary<string, Goal>();
    static Table goal_table = new Table().Centered();

    static async Task Main(string[] args)
    {
        goal_table.AddColumn("Name");
        goal_table.AddColumn("Created");
        goal_table.AddColumn("Status");

        while (true)
        {
            AnsiConsole.ResetColors();
            AnsiConsole.Clear();
            AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .Start("Loading...", ctx =>
                {
                    CheckIntegrity();
                    LoadGoals();
                });
            AnsiConsole.Write(new FigletText("GOAL").Centered());

            AnsiConsole.Live(goal_table)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .Cropping(VerticalOverflowCropping.Top)
                .Start(ctx =>
                {
                    ctx.Refresh();
                });

            AnsiConsole.Foreground = Color.Grey37;
            AnsiConsole.WriteLine("Actions: ");
            AnsiConsole.WriteLine("[1] Open");
            AnsiConsole.WriteLine("[2] Delete");
            string action = AnsiConsole.Prompt<string>(new TextPrompt<string>("Action: "));
            switch (action.ToLower())
            {
                case "1":
                case "o":
                case "op":
                case "ope":
                case "open":
                    string open = AnsiConsole.Prompt<string>(new TextPrompt<string>("Goal name: "));
                    if (!goals.ContainsKey(open))
                    {
                        AnsiConsole.Foreground = Color.Red1;
                        AnsiConsole.WriteLine("\"" + open + "\" does not exist.");
                        await Task.Delay(2000);
                        break;
                    }
                    await goals[open].Run();
                    AnsiConsole.Markup("[yellow]Press ANY key to continue...[/]");
                    Console.ReadKey(true);
                    break;

                case "2":
                case "d":
                case "de":
                case "del":
                case "dele":
                case "delet":
                case "delete":
                    string delete = AnsiConsole.Prompt<string>(new TextPrompt<string>("Goal name: "));
                    break;

                default:
                    AnsiConsole.Foreground = Color.Red1;
                    AnsiConsole.WriteLine("Invalid action.");
                    await Task.Delay(2000);
                    break;
            }
        }
    }

    static void LoadGoals()
    {
        goals.Clear();
        goal_table.Rows.Clear();
        string[] goal_files = Directory.GetFiles(config.GoalsPath.ReplacePath(), "*.goal");
        for (int gf = 0; gf < goal_files.Length; gf++)
        {
            Goal goal = Goal.Load(goal_files[gf]);
            goals.Add(goal.Name, goal);

            goal_table.AddRow(new Text(goal.Name), new Text(goal.Created.ToString()), new Text(goal.Status));
        }
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
