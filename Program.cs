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
    static int selected_goal = -1;

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

            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Escape)
            {
                Environment.Exit(0);
            }
            else if (key.Key == ConsoleKey.UpArrow)
            {
                if (selected_goal > 0)
                {
                    selected_goal--;
                }
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                if (selected_goal < goal_table.Rows.Count - 1)
                {
                    selected_goal++;
                }
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                await goals.ElementAt(selected_goal).Value.Run();
                AnsiConsole.Markup("[yellow]Press ANY key to continue...[/]");
                Console.ReadKey(true);
                continue;
            }
            else if (key.Key == ConsoleKey.Backspace || key.Key == ConsoleKey.Delete || key.Key == ConsoleKey.X || key.Key == ConsoleKey.D)
            {

            }
        }
    }

    static void LoadGoals()
    {
        Style selected_style = new Style(Color.Yellow, null, Decoration.Bold);

        goals.Clear();
        goal_table.Rows.Clear();
        string[] goal_files = Directory.GetFiles(config.GoalsPath.ReplacePath(), "*.goal");
        for (int gf = 0; gf < goal_files.Length; gf++)
        {
            Goal goal = Goal.Load(goal_files[gf]);
            goals.Add(goal.Name, goal);

            goal_table.AddRow(
                    new Text(goal.Name, (gf == selected_goal ? selected_style : null)),
                    new Text(goal.Created.ToString(), (gf == selected_goal ? selected_style : null)),
                    new Text(goal.Status, (gf == selected_goal ? selected_style : null)));
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
