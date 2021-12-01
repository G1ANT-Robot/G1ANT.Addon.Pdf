using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.PDF
{
    [Command(Name = "pdf.addpage", Tooltip = "This command adds new page to the pdf document")]
    public class PdfAddPageCommand : Command
    {
        public PdfAddPageCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Name = "pdf", Required = true, Tooltip = "PDF structure to which a new page will be added")]
            public PdfStructure Pdf { get; set; }
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf.Value;
            pdf.AddPage();
        }
    }
}
