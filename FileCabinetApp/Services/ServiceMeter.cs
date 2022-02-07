using System.Collections.ObjectModel;
using System.Diagnostics;
using FileCabinetApp.Data;

namespace FileCabinetApp.Service
{
    /// <summary>
    /// File cabinet service which displays information about the execution time of the operation.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly Stopwatch watch;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        public ServiceMeter(IFileCabinetService fileCabinetService)
        {
            this.service = fileCabinetService;
            this.watch = new Stopwatch();
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordArgs recordToCreate)
        {
            this.watch.Reset();
            this.watch.Start();

            var recordId = this.service.CreateRecord(recordToCreate);

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.CreateRecord)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
            return recordId;
        }

        /// <inheritdoc/>
        public void EditRecord(RecordArgs recordToEdit)
        {
            this.watch.Reset();
            this.watch.Start();

            this.service.CreateRecord(recordToEdit);

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.EditRecord)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string sourceDate)
        {
            this.watch.Reset();
            this.watch.Start();

            var records = this.service.FindByDateOfBirth(sourceDate);

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.FindByDateOfBirth)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
            return records;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.watch.Reset();
            this.watch.Start();

            var records = this.service.FindByDateOfBirth(firstName);

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.FindByFirstName)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
            return records;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.watch.Reset();
            this.watch.Start();

            var records = this.service.FindByDateOfBirth(lastName);

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.FindByLastName)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
            return records;
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecordById(int id)
        {
            this.watch.Reset();
            this.watch.Start();

            var record = this.service.GetRecordById(id);

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.GetRecordById)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
            return record;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.watch.Reset();
            this.watch.Start();

            var records = this.service.GetRecords();

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.GetRecords)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
            return records;
        }

        /// <inheritdoc/>
        public (int, int) GetStat()
        {
            this.watch.Reset();
            this.watch.Start();

            var stat = this.service.GetStat();

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.GetStat)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
            return stat;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.watch.Reset();
            this.watch.Start();

            var snapshot = this.service.MakeSnapshot();

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.MakeSnapshot)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
            return snapshot;
        }

        /// <inheritdoc/>
        public void Purge()
        {
            this.watch.Reset();
            this.watch.Start();

            this.service.Purge();

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.Purge)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
        }

        /// <inheritdoc/>
        public bool Remove(int recordId)
        {
            this.watch.Reset();
            this.watch.Start();

            var isSuccessful = this.service.Remove(recordId);

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.Remove)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
            return isSuccessful;
        }

        /// <inheritdoc/>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.watch.Reset();
            this.watch.Start();

            var amountOfRestoredRecords = this.service.Restore(snapshot);

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.Restore)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
            return amountOfRestoredRecords;
        }
    }
}