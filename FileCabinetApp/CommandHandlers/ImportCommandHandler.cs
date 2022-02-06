using System.Xml;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for import command.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        public ImportCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
            this.CommandName = "import";
        }

        /// <inheritdoc/>
        protected override void Command(string parameters)
        {
            const int paramsNumber = 2;
            string[] parameterExplanations = new string[] { "file format", "path" };
            if (!GetParameters(paramsNumber, parameters, parameterExplanations, out var paramsArray))
            {
                return;
            }

            string fileFormat;
            string path;

            try
            {
                fileFormat = paramsArray[0];
                path = paramsArray[1];
            }
            catch
            {
                Console.WriteLine($"Invalid parameters. <param1> - {parameterExplanations[0]}. <param2> - {parameterExplanations[1]}");
                return;
            }

            var file = new FileInfo(path);
            if (!file.Exists)
            {
                Console.WriteLine($"Import error: file {path} is not exist.");
                return;
            }

            try
            {
                string message = string.Empty;
                var snapshot = this.FileCabinetService.MakeSnapshot();
                Console.WriteLine("Import started.");
                switch (fileFormat)
                {
                    case "csv":
                        using (var reader = new StreamReader(path))
                        {
                            snapshot.LoadFromCsv(reader);
                            int countOfRestoredrecords = this.FileCabinetService.Restore(snapshot);
                            message = $"{countOfRestoredrecords} records were imported from {path}";
                        }

                        break;

                    case "xml":
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Indent = true;
                        settings.IndentChars = "\t";
                        settings.OmitXmlDeclaration = true;

                        using (var xmlReader = XmlReader.Create(path))
                        {
                            snapshot.LoadFromXmlWithXmlSerializer(xmlReader);
                            int countOfRestoredrecords = this.FileCabinetService.Restore(snapshot);
                            message = $"{countOfRestoredrecords} records were imported from {path}";
                        }

                        break;

                    default:
                        message = $"<param1> - unsuppurted file format.";
                        break;
                }

                Console.WriteLine(message);
            }
            catch
            {
                Console.WriteLine($"Import failed: can't open file {path}");
            }
        }
    }
}
