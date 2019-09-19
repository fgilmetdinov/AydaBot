using System.ComponentModel;

namespace AydaBot.Common.Enums
{
    public enum ErrorCode : byte
    {
        [Description("Успешно")]
        Ok = 0,
        [Description("Ошибка")]
        Error = 1,
        [Description("Сериал не найден")]
        SerialNotFound = 2,
        [Description("Пользователь не найден")]
        UserNotFound = 3,
        [Description("Подписка не найдена")]
        SuscriptionNotFound = 4,
        [Description("Подписка уже существует")]
        SubscriptionAlreadyExists = 5
    }
}
