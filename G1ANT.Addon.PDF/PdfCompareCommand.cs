using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.PDF
{
    [Command(Name ="pdf.compare", Tooltip = "Check if two pdf files are the same")]
    public class PdfCompareCommand : Command
    {
        public PdfCompareCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Name = "path1", Required = true, Tooltip = "Path to the first pdf")]
            public TextStructure Path1 { get; set; }

            [Argument(Name = "path2", Required = true, Tooltip ="Path to the second pdf")]
            public TextStructure Path2 { get; set; }

            [Argument(Name = "password", Required = false, Tooltip = "Password to the file/files")]
            public TextStructure Password { get; set; }

            [Argument(Name = "result", Required = false, Tooltip = "Returns true if docuemnts are equal")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public void Execute(Arguments arguments)
        {
            bool result;

            if (arguments.Password == null)
            {
                result = PdfDocument.DocumentsAreEqual(arguments.Path1.Value, arguments.Path2.Value);
            }
            else
            {
                var standardDecryptionHandler = new PdfStandardDecryptionHandler(arguments.Password.Value);
                result = PdfDocument.DocumentsAreEqual(arguments.Path1.Value, arguments.Path2.Value, standardDecryptionHandler);
            }
            Scripter.Variables.SetVariableValue(arguments.Result.Value, new BooleanStructure(result, null, null));
        }
    }
}
