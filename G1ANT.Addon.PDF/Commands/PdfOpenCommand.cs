using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;
using System.ComponentModel;
using System.Windows.Forms;
using G1ANT.Addon.PDF.Models;

namespace G1ANT.Addon.Pdf
{
    [Command(Name = "pdf.open", Tooltip = "Command using to open pdf document.")]
    public class PdfOpenCommand : Command
    {
        public PdfOpenCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Path to file")]
            public TextStructure Path { get; set; }

            [Argument(Required = false, Tooltip = "Password to file")]
            public TextStructure Password { get; set; }

            [Argument(Required = false, Tooltip = "Returns PDF structure")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public void Execute(Arguments arguments)
        {
            var pdfDoc = PdfModel.Open(arguments.Path.Value, arguments.Password?.Value);
            Scripter.Variables.SetVariableValue(arguments.Result.Value, new PdfStructure(pdfDoc, null, Scripter));
        }
    }
}
