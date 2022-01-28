using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Сlass provides a custom service for storing file cabinet records and operations on them.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        protected override IRecordValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}
