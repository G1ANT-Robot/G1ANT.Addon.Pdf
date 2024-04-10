using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.Pdf
{
    [Command(Name = "pdf.removepage", Tooltip = "Removes given page")]
    public class PdfRemovePageCommand : Command
    {
        public PdfRemovePageCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "PDF structure from which the given page will be removed")]
            public PdfStructure Pdf { get; set; }

            [Argument(Required = true, Tooltip = "Page to be removed")]
            public IntegerStructure PageNumber { get; set; }
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf?.Value;
            if (pdf is null)
                throw new ArgumentNullException(nameof(arguments.Pdf));
            var pdfPageCount = pdf.PageCount;
            if (arguments.PageNumber.Value < 1)
            {
                throw new ArgumentException("Page number can't be smaller than 1");
            }
            else if (arguments.PageNumber.Value >= pdfPageCount)
            {
                throw new ArgumentException("Page number can't be bigger than number of pages");
            } 
            pdf.RemovePage(arguments.PageNumber.Value - 1);
        }
    }
}
