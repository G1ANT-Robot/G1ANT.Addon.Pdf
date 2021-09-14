using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.PDF
{
    [Command(Name ="pdf.compare", Tooltip = "Chek if two pdf files are the same")]
    public class PdfCompareCommand : Command
    {
        public PdfCompareCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Name = "firstpdfpath", Required = true, Tooltip ="Path to first file to compare")]
            public TextStructure FirstPdfPath { get; set; }

            [Argument(Name = "secondpdfpath", Required = true, Tooltip ="Path to second file to compare")]
            public TextStructure SecondPdfPath { get; set; }

            [Argument(Name = "password", Required = false, Tooltip = "Password to the file/files")]
            public TextStructure Password { get; set; } = new TextStructure("");

            [Argument(Name = "result", Required = false, Tooltip = "Returns true if docuemnts are equal")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public void Execute(Arguments arguments)
        {
            var standardDecryptionHandler = new PdfStandardDecryptionHandler(arguments.Password.Value);
            var res = PdfDocument.DocumentsAreEqual(arguments.FirstPdfPath.Value, arguments.SecondPdfPath.Value, standardDecryptionHandler);
            
            Scripter.Variables.SetVariableValue(arguments.Result.Value, new BooleanStructure(res, null, null));
        }
    }
}
