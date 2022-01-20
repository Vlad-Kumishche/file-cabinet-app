﻿using System.Globalization;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string? firstName, string? lastName, DateTime dateOfBirth, short height, decimal cashSavings, char favoriteLetter)
        {
            const int minLength = 2;
            const int maxLength = 60;
            DateTime minDate = new DateTime(1950, 1, 1);
            const short minHeight = 40;
            const short maxHeight = 300;
            const decimal minCashSavings = 0M;
            const decimal maxCashSavings = 10_000_000M;

            ValidateOnlyLetters(firstName, minLength, maxLength);
            ValidateOnlyLetters(lastName, minLength, maxLength);
            ValidateDate(dateOfBirth, minDate);
            ValidateNumber(height, minHeight, maxHeight);
            ValidateNumber(cashSavings, minCashSavings, maxCashSavings);
            ValidateOnlyLetters(favoriteLetter.ToString(), 1, 1);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Height = height,
                CashSavings = cashSavings,
                FavoriteLetter = favoriteLetter,
            };

            this.list.Add(record);

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        private static void ValidateOnlyLetters(string? line, int minLengh, int maxLength)
        {
            if (string.IsNullOrEmpty(line))
            {
                throw new ArgumentNullException(nameof(line));
            }
            else if (line.Length < minLengh || line.Length > maxLength)
            {
                throw new ArgumentException($"{nameof(line)}.Length does not meet the requirements.", nameof(line));
            }
            else
            {
                foreach (char c in line)
                {
                    if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z'))
                    {
                        throw new ArgumentException($"{nameof(line)} contains a non-Latin character.", nameof(line));
                    }
                }
            }
        }

        private static void ValidateDate(DateTime date, DateTime minDate)
        {
            if (date < minDate || date >= DateTime.Now)
            {
                throw new ArgumentException($"Invalid {nameof(date)}. Min date: {minDate.ToString("MM / dd / yyyy", CultureInfo.InvariantCulture)}, max date: today.", nameof(date));
            }
        }

        private static void ValidateNumber(short number, short minNumber, short maxNumber)
        {
            if (number < minNumber || number > maxNumber)
            {
                throw new ArgumentException($"The {nameof(number)} is not within the allowed range.", nameof(number));
            }
        }

        private static void ValidateNumber(decimal number, decimal minNumber, decimal maxNumber)
        {
            if (number < minNumber || number > maxNumber)
            {
                throw new ArgumentException($"The {nameof(number)} is not within the allowed range.", nameof(number));
            }
        }
    }
}