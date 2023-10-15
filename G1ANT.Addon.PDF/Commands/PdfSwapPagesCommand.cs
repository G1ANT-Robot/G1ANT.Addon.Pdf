using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.Pdf
{
    [Command(Name = "pdf.swappage", Tooltip = "This command swaps two pages")]
    public class PdfSwapPagesCommand : Command
    {
        public PdfSwapPagesCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "PDF structure to operates on")]
            public PdfStructure Pdf { get; set; }

            [Argument(Required = true, Tooltip = "First page")]
            public IntegerStructure Page1 { get; set; }

            [Argument(Required = true, Tooltip = "Second page")]
            public IntegerStructure Page2 { get; set; }
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf?.Value;
            if (pdf is null)
                throw new ArgumentNullException(nameof(arguments.Pdf));

            pdf.SwapPages(arguments.Page1.Value, arguments.Page2.Value);
        }
    }
}
