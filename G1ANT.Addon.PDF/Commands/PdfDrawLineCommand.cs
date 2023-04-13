using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.Pdf
{
    [Command(Name = "pdf.drawline", Tooltip = "This command draws line on the document")]
    public class PdfDrawLineCommand : Command
    {
        public PdfDrawLineCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "PDF structure to operates on")]
            public PdfStructure Pdf { get; set; }

            [Argument(Required = true, Tooltip = "Page number")]
            public IntegerStructure Page { get; set; }

            [Argument(Required = true, Tooltip = "Starting position")]
            public PointStructure From { get; set; }

            [Argument(Required = true, Tooltip = "Ending position")]
            public PointStructure To { get; set; }
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf?.Value;
            if (pdf is null)
                throw new ArgumentNullException(nameof(arguments.Pdf));

            pdf.DrawLine(arguments.Page.Value, arguments.From.Value, arguments.To.Value);
        }
    }
}
