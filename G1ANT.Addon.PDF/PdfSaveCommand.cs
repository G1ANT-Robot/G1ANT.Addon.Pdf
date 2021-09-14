using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.PDF
{
    [Command(Name = "pdf.save", Tooltip ="Saves pdf file in given path")]
    public class PdfSaveCommand : Command
    {
        public PdfSaveCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Name = "path", Required = true, Tooltip = "Path where file will be saved")]
            public TextStructure Path { get; set; }

            [Argument(Name = "pdf", Required = true, Tooltip = "PDFstructure which will be saved")]
            public PdfStructure Pdf { get; set; } 
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf.Value;
            pdf.Save(arguments.Path.Value);
        }
    }
}
