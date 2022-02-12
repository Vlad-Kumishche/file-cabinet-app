﻿using FileCabinetApp.Data;
using FileCabinetApp.Printers;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for list command.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private readonly IRecordPrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        /// <param name="printer">Used printer.</param>
        public ListCommandHandler(IFileCabinetService fileCabinetService, IRecordPrinter printer)
            : base(fileCabinetService)
        {
            this.CommandName = "list";
            this.printer = printer;
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            var records = this.FileCabinetService.GetRecords();
            this.printer.Print(records, new List<string>());
            Console.WriteLine();
        }
    }
}
