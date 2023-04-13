using G1ANT.Addon.PDF.Models;
using G1ANT.Language;

namespace G1ANT.Addon.Pdf
{
    [Command(Name = "pdf.create", Tooltip = "Creates pdf document")]
    public class PdfCreateCommand : Command
    {
        public PdfCreateCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Required = false, Tooltip = "Result where to store the new pdf document")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public void Execute(Arguments arguments)
        {
            Scripter.Variables.SetVariableValue(arguments.Result.Value, new PdfStructure(new PdfModel(), null, Scripter));
        }
    }
}
