using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.PDF
{
    [Command(Name = "pdf.removepage", Tooltip = "Removes given page")]
    public class PdfRemovePageCommand : Command
    {

        public PdfRemovePageCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Name = "pdf", Required = true, Tooltip = "PDF structure to wich a new page will be added")]
            public PdfStructure Pdf { get; set; }

            [Argument(Name = "pagenumber", Required = true, Tooltip = "Page to remove. Starting from 1")]
            public IntegerStructure PageNumber { get; set; }
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf.Value;

            int pageToRemove = arguments.PageNumber.Value - 1;
            if (arguments.PageNumber.Value <= 0)
            {
                throw new ArgumentException("Page number can't be smaller than 1");
            }
            else
            {
                pdf.RemovePage(pageToRemove);
            }
        }
    }
}
