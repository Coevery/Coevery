using Piedone.Combinator.Models;

namespace Piedone.Combinator.Extensions
{
    public static class CombinedFileRecordExtensions
    {
        // Records can't have properties or methods whose are not persisted
        public static string GetFileName(this CombinedFileRecord record)
        {
            return record.HashCode + "-" + record.Slice;
        }
    }
}