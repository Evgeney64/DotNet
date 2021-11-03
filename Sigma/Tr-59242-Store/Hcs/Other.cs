using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Hcs.Model;

namespace Hcs
{
    public interface IFileApi
    {
        Task UploadAsync(IAttachment data, string context);
    }

    [Obsolete]
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

    public class TransactionPartitioner
    {
        public static IList<IGrouping<int, ObjectInfo>> Partition(IEnumerable<ObjectInfo> objectInfo, int maxListCount)
        {
            if (maxListCount < 1)
            {
                maxListCount = 1;
            }

            var groupedObjectInfo = objectInfo
                .GroupBy(o => new { o.Operation, o.Group })
                .SelectMany(l =>
                {
                    int groupCount = l.Count();
                    int chunkSize = groupCount / maxListCount + (groupCount % maxListCount > 0 ? 1 : 0);
                    return l
                        .Select((x, i) => new { Index = i, Value = x })
                        .GroupBy(x => x.Index % chunkSize, x => x.Value, (k, g) => new Grouping<int, ObjectInfo>(l.Key.Operation, g));
                })
                .ToList<IGrouping<int, ObjectInfo>>();

            return groupedObjectInfo;
        }
    }
}