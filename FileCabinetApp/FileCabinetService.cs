﻿using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Сlass provides a service for storing file cabinet records and operations on them.
    /// </summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <summary>
        /// Creates file cabinet records with given parameters.
        /// </summary>
        /// <param name="firstName">The first name of the person.</param>
        /// <param name="lastName">The last name of the person.</param>
        /// <param name="dateOfBirth">The date of birth of the person.</param>
        /// <param name="height">The height of the person.</param>
        /// <param name="cashSavings">The cash savings of the person.</param>
        /// <param name="favoriteLetter">The favorite letter of the person.</param>
        /// <returns>The id of the record.</returns>
        public int CreateRecord(string? firstName, string? lastName, DateTime dateOfBirth, short height, decimal cashSavings, char favoriteLetter)
        {
            ValidateRecord(firstName, lastName, dateOfBirth, height, cashSavings, favoriteLetter);

            // to avoid CS8604
            firstName ??= string.Empty;
            lastName ??= string.Empty;

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
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary[firstName].Add(record);
            }
            else
            {
                this.firstNameDictionary[firstName] = new List<FileCabinetRecord>() { record };
            }

            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary[lastName].Add(record);
            }
            else
            {
                this.lastNameDictionary[lastName] = new List<FileCabinetRecord>() { record };
            }

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                this.dateOfBirthDictionary[dateOfBirth].Add(record);
            }
            else
            {
                this.dateOfBirthDictionary[dateOfBirth] = new List<FileCabinetRecord>() { record };
            }

            return record.Id;
        }

        /// <summary>
        /// Edits file cabinet records with the specified id.
        /// </summary>
        /// <param name="id">The id of the record.</param>
        /// <param name="firstName">The first name of the person.</param>
        /// <param name="lastName">The last name of the person.</param>
        /// <param name="dateOfBirth">The date of birth of the person.</param>
        /// <param name="height">The height of the person.</param>
        /// <param name="cashSavings">The cash savings of the person.</param>
        /// <param name="favoriteLetter">The favorite letter of the person.</param>
        public void EditRecord(int id, string? firstName, string? lastName, DateTime dateOfBirth, short height, decimal cashSavings, char favoriteLetter)
        {
            ValidateRecord(firstName, lastName, dateOfBirth, height, cashSavings, favoriteLetter);

            var record = this.GetRecordById(id);
            record.FirstName = firstName;
            record.LastName = lastName;
            record.DateOfBirth = dateOfBirth;
            record.Height = height;
            record.CashSavings = cashSavings;
            record.FavoriteLetter = favoriteLetter;
        }

        /// <summary>
        /// Finds records by first name.
        /// </summary>
        /// <param name="firstName">The first name of the person.</param>
        /// <returns>Array of records.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                return this.firstNameDictionary[firstName].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Finds records by last name.
        /// </summary>
        /// <param name="lastName">The last name of the person.</param>
        /// <returns>Array of records.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                return this.lastNameDictionary[lastName].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Finds records by date of birth.
        /// </summary>
        /// <param name="date">The date of birth of the person.</param>
        /// <returns>Array of records.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(string date)
        {
            if (!DateTime.TryParse(date, out var dateOfBirth))
            {
                throw new ArgumentException($"Invalid {nameof(date)}", nameof(date));
            }

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                return this.dateOfBirthDictionary[dateOfBirth].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Gets record by id.
        /// </summary>
        /// <param name="id">The id of the record.</param>
        /// <returns>Required file cabinet record.</returns>
        /// <exception cref="ArgumentException">Thrown when record with given id was not found.</exception>
        public FileCabinetRecord GetRecordById(int id)
        {
            var record = this.list.Find(x => x.Id == id);
            if (record == null)
            {
                throw new ArgumentException($"There is no record with {nameof(id)} == {id}", nameof(id));
            }

            return record;
        }

        /// <summary>
        /// Gets all file cabinet records.
        /// </summary>
        /// <returns>Array of records.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        /// <summary>
        /// Gets the number of records.
        /// </summary>
        /// <returns>Number of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        private static void ValidateRecord(string? firstName, string? lastName, DateTime dateOfBirth, short height, decimal cashSavings, char favoriteLetter)
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
