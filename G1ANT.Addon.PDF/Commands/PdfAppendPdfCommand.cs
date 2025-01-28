using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.Pdf
{
    [Command(Name = "pdf.appendpdf", Tooltip = "This command appends one pdf document to the current")]
    public class PdfAppendPdfCommand : Command
    {
        public PdfAppendPdfCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "PDF structure to append to")]
            public PdfStructure Pdf { get; set; }

            [Argument(Required = true, Tooltip = "PDF structure to which to append")]
            public PdfStructure OtherPdf { get; set; }

            [Argument(Required = false, Tooltip = "List of pages to append or empty to append all pages")]
            public ListStructure Pages { get; set; }
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf?.Value;
            var pdfOther = arguments.OtherPdf?.Value;
            var pages = arguments.Pages?.Value?.Cast<int>().ToList();
            if (pdf is null)
                throw new ArgumentNullException(nameof(arguments.Pdf));
            if (pdfOther is null)
                throw new ArgumentNullException(nameof(arguments.OtherPdf));


            pdf.AppendDocument(pdfOther, pages);
        }
    }
}
