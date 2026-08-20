namespace XTrendApp.Web.Common;

public static class Logger
{
    public static bool Verbose { get; set; } = false;

    public static void Info(string message)
    {
        Console.WriteLine(message);
    }

    public static void Debug(string message)
    {
        if (Verbose)
            Console.WriteLine(message);
    }

    public static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void Success(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}