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
    [Command(Name = "pdf.ispassword", Tooltip = "Checks if PDF is password protected. Return true if yes.")]
    public class PdfIsPasswordCommand : Command
    {

        public PdfIsPasswordCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Name = "path", Required = true, Tooltip = "Path to PDF file")]
            public TextStructure Path { get; set; }

            [Argument(Name = "result", Required = false, Tooltip = "Returns true if PDF is password protected")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public void Execute(Arguments arguments)
        {
            using (FileStream fs = File.Open(arguments.Path.Value, FileMode.Open, FileAccess.Read, FileShare.None))
            {

                bool res;

                try
                {
                    var pdfFile = new PdfDocument(fs);
                    res = false;
                }
                catch (IncorrectPasswordException e)
                {
                    res = true;
                }

                Scripter.Variables.SetVariableValue(arguments.Result.Value, new BooleanStructure(res, null, null));
            }
            
        }

    }
}
