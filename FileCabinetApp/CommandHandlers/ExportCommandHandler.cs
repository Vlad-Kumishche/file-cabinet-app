using System.Text;
using System.Xml;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for export command.
    /// </summary>
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Used service.</param>
        public ExportCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
            this.CommandName = "export";
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
            if (file.Exists)
            {
                Console.Write($"File is exist - rewrite {path}? [Y/n] ");
                char answer = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (answer != 'Y')
                {
                    return;
                }
            }

            try
            {
                var snapshot = this.fileCabinetService.MakeSnapshot();
                string messageToUser;
                switch (fileFormat)
                {
                    case "csv":
                        var csvWriter = new StreamWriter(path, false, Encoding.UTF8);
                        snapshot.SaveToCsv(csvWriter);
                        csvWriter.Close();
                        messageToUser = $"All records are exported to file {path}";
                        break;

                    case "xml":
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Indent = true;
                        settings.IndentChars = "\t";
                        settings.OmitXmlDeclaration = true;

                        var xmlWriter = XmlWriter.Create(path, settings);
                        snapshot.SaveToXml(xmlWriter);
                        xmlWriter.Close();
                        messageToUser = $"All records are exported to file {path}";
                        break;

                    default:
                        messageToUser = $"<param1> - unsuppurted file format.";
                        break;
                }

                Console.WriteLine(messageToUser);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine($"Export failed: can't open file {path}");
            }
        }
    }
}
