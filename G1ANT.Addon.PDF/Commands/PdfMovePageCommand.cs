using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.Pdf
{
    [Command(Name = "pdf.movepage", Tooltip = "This command moves the page to different position")]
    public class PdfMovePageCommand : Command
    {
        public PdfMovePageCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "PDF structure to operates on")]
            public PdfStructure Pdf { get; set; }

            [Argument(Required = true, Tooltip = "Page to be moved")]
            public IntegerStructure Page { get; set; }

            [Argument(Required = true, Tooltip = "Destination position of the page")]
            public IntegerStructure Destination { get; set; }
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf?.Value;
            if (pdf is null)
                throw new ArgumentNullException(nameof(arguments.Pdf));

            pdf.MovePage(arguments.Page.Value, arguments.Destination.Value);
        }
    }
}
