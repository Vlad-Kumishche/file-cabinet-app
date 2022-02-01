using FileCabinetApp;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Generates default records.
    /// </summary>
    public class DefaultRecordGenerator : IRecordGenerator
    {
        /// <inheritdoc/>
        public FileCabinetRecord GenerateRecord(int id)
        {
            var rand = new Random();

            return new FileCabinetRecord
            {
                Id = id,
                FirstName = GenerateString(rand, rand.Next(2, 60)),
                LastName = GenerateString(rand, rand.Next(2, 60)),
                DateOfBirth = new DateTime(rand.Next(1950, DateTime.Now.Year - 1), rand.Next(1, 12), rand.Next(1, 28)),
                Height = (short)rand.Next(40, 300),
                CashSavings = (decimal)rand.Next(0, 10_000_000),
                FavoriteLetter = (char)rand.Next(97, 122),
            };
        }

        /// <inheritdoc/>
        public List<FileCabinetRecord> GenerateRecords(int startId, int amount)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            for (int i = 0; i < amount; i++)
            {
                records.Add(this.GenerateRecord(startId + i));
            }

            return records;
        }

        private static string GenerateString(Random random, int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var stringChars = new char[length];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
    }
}