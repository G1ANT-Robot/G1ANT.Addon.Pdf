using System;
using System.Collections.Generic;
using System.Linq;
using G1ANT.Language;

namespace G1ANT.Addon.Pdf
{
    [Command(Name = "pdf.extractpages", Tooltip = "This command extracts pages to the new document")]
    public class PdfExtractPagesCommand : Command
    {
        public PdfExtractPagesCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "PDF structure to which a new page will be added")]
            public PdfStructure Pdf { get; set; }

            [Argument(Required = true, Tooltip = "List of page numbers")]
            public ListStructure Pages{ get; set; }

            [Argument(Required = false, Tooltip = "New pdf document")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf?.Value;
            if (pdf is null)
                throw new ArgumentNullException(nameof(arguments.Pdf));
            if (arguments.Pages?.Value is null)
                throw new ArgumentNullException(nameof(arguments.Pages));
            if (arguments.Pages.ListItemType != typeof(int) &&
                arguments.Pages.ListItemType != typeof(IntegerStructure))
                throw new ArgumentException("Pages need to be integers", nameof(arguments.Pages));

            var pages = arguments.Pages?.Value?.Select(x => (int)x).ToArray();
            var newpdf = pdf.ExtractPages(pages);
            Scripter.Variables.SetVariableValue(arguments.Result.Value, new PdfStructure(newpdf, null, Scripter));
        }
    }
}
