using System;
using System.ComponentModel;
using Hcs.Model;

namespace Hcs
{
    public interface IFileApi
    {
        void Upload(IAttachment data, string context);
    }

    public enum PerformServiceError
    {
        [Description("Запрос принят.")]
        HCS_SRV_10000,
        [Description("Запрос в обработке.")]
        HCS_SRV_20000,

        [Description("Не удалось сформировать данные для запроса в ГИС ЖКХ.")]
        HCS_DAT_00001,
        [Description("Не удалось прочитать данные, полученные от ГИС ЖКХ.")]
        HCS_DAT_00002,
        [Description("Данные не соответствуют схеме.")]
        HCS_DAT_09999,

        [Description("Не указан ФИАС.")]
        HCS_DAT_10001,
        [Description("Тип дома в ГИС ЖКХ отличается от указанного.")]
        HCS_DAT_10002,
    }
}