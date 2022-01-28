using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Сlass provides a default service for storing file cabinet records and operations on them.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        protected override IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }
    }
}
