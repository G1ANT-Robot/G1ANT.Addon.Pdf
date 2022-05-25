using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using System.IO;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.PDF
{
    [Command(Name = "pdf.addpage", Tooltip = "This command adds new page to the pdf document")]
    public class PdfExtractImageCommand : Command
    {
        public PdfExtractImageCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Name = "pdf", Required = true, Tooltip = "PDF structure to which a new page will be added")]
            public PdfStructure Pdf { get; set; }

            [Argument(Name = "result", Required = false, Tooltip = "Result file name")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");

            [Argument(Name = "resolution", Required = false, Tooltip = "Resolution in DPI. Default: 300")]
            public IntegerStructure Resolution { get; set; } = new IntegerStructure(300);

            [Argument(Name = "pagenumber", Required = false, Tooltip = "Number of page to extract. Default: 1")]
            public IntegerStructure PageNumber { get; set; } = new IntegerStructure(1);
        }

        public void Execute(Arguments arguments)
        {
            var options = PdfDrawOptions.Create();
            options.VerticalResolution = arguments.Resolution.Value;
            options.HorizontalResolution = arguments.Resolution.Value;
            var fileName = Path.ChangeExtension(Path.GetTempFileName(), ".png");

            arguments.Pdf.Value.Pages[arguments.PageNumber.Value - 1].Save(fileName, options);

            Scripter.Variables.SetVariableValue(arguments.Result.Value, new PathStructure(fileName, null, Scripter));
        }
    }
}
