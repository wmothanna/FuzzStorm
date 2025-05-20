namespace FuzzStorm.Models;

internal static class Helper
{
    public static string ExceptionMsgSettingNull(string objName)
    {
        return $"{objName} connot be null";
    }
    public static string ExceptionMsgGettingNull(string objName)
    {
        return $"{objName} is null";
    }

    public static void InvalidCmdArgument(string message, Int32 exitCode = 0)
    {
        Console.WriteLine
        (
            $"\n{message}\r\n" +
            $"try --help for usage\n"
        );
        Environment.Exit(exitCode);
    }
}
