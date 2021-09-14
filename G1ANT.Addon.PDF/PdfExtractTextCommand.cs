using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.PDF
{
    [Command(Name = "pdf.extracttext", Tooltip = "Extract text from whole pdf")]
    public class PdfExtractTextCommand : Command
    {
        public PdfExtractTextCommand (AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Name = "pdf", Required = true, Tooltip = "Document to extract text from")]
            public PdfStructure Pdf { get; set; }

            [Argument(Name = "result", Required = false, Tooltip = "Result where extracted text will be stored")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf.Value;
            Scripter.Variables.SetVariableValue(arguments.Result.Value, new TextStructure(pdf.GetTextWithFormatting(), null, null));
        }
    }
}
