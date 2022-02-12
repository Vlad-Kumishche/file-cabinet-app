using System.Collections.ObjectModel;
using System.Diagnostics;
using FileCabinetApp.Data;

namespace FileCabinetApp.Services
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
        public int CreateRecord(RecordParameters recordToCreate)
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
        public int Insert(RecordParameters recordToInsert)
        {
            this.watch.Reset();
            this.watch.Start();

            var recordId = this.service.Insert(recordToInsert);

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.Insert)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
            return recordId;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<int> Update(List<KeyValuePair<string, string>> newParameters, List<KeyValuePair<string, string>> searchOptions, string logicalOperator)
        {
            this.watch.Reset();
            this.watch.Start();

            var updatedRecordIds = this.service.Update(newParameters, searchOptions, logicalOperator);

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.Update)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
            return updatedRecordIds;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> SelectByOptions(List<KeyValuePair<string, string>> searchOptions, string logicalOperator)
        {
            this.watch.Reset();
            this.watch.Start();

            var selectedRecords = this.service.SelectByOptions(searchOptions, logicalOperator);

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.SelectByOptions)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
            return selectedRecords;
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
        public ReadOnlyCollection<int> Delete(List<KeyValuePair<string, string>> searchOptions, string logicalOperator)
        {
            this.watch.Reset();
            this.watch.Start();

            var deletedRecordIds = this.service.Delete(searchOptions, logicalOperator);

            this.watch.Stop();

            Console.WriteLine($"{nameof(this.service.Delete)} method execution duration is {this.watch.ElapsedTicks} ticks.");
            Console.WriteLine();
            return deletedRecordIds;
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