using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.PDF
{
    [Command(Name = "pdf.open", Tooltip = "Command using to open pdf document.")]
    public class PdfOpenCommand : Command
    {
        public PdfOpenCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Name = "path", Required = true, Tooltip = "Path to file")]
            public TextStructure Path { get; set; }

            [Argument(Name = "password", Required = false, Tooltip = "Password to file")]
            public TextStructure Password { get; set; }

            [Argument(Name = "result", Required = false, Tooltip = "Returns PDF structure")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public void Execute(Arguments arguments)
        {
            FileStream fs = File.Open(arguments.Path.Value, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            var standardDecryptionHandler = arguments.Password != null ? new PdfStandardDecryptionHandler(arguments.Password.Value) : null;
            var pdfFile = standardDecryptionHandler == null ? new PdfDocument(fs) : new PdfDocument(fs, standardDecryptionHandler);

            Scripter.Variables.SetVariableValue(arguments.Result.Value, new PdfStructure(pdfFile, null, Scripter));
        }
    }
}
