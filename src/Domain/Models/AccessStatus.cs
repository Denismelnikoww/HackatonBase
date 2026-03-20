using System.ComponentModel;
using System.Reflection;

namespace Domain.Models;

public enum AccessStatus
{
    [Description("Разрешить")] Granted,

    [Description("Запретить")] Denied,

    [Description("Истек срок токена или неизвестные причины")]
    Unknown,

    [Description("Пользователь был заблокирован или удален")]
    Banned,
}

public static class AccessStatusExtension
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}