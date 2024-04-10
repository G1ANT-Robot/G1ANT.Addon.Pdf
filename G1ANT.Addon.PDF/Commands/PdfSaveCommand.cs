using System;
using G1ANT.Language;

namespace G1ANT.Addon.Pdf
{
    [Command(Name = "pdf.save", Tooltip = "Saves pdf file in given path")]
    public class PdfSaveCommand : Command
    {
        public PdfSaveCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Path where file will be saved")]
            public TextStructure Path { get; set; }

            [Argument(Required = true, Tooltip = "Pdf structure which will be saved")]
            public PdfStructure Pdf { get; set; }

            [Argument(Required = false, Tooltip = "Password to protect pdf")]
            public TextStructure Password { get; set; }

            [Argument(Required = false, Tooltip = "Password of the owner of pdf. If empty, it will be set to 'password' argument ")]
            public TextStructure OwnerPassword { get; set; }
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf.Value;
            if (pdf is null)
                throw new ArgumentNullException(nameof(arguments.Pdf));
            if (string.IsNullOrEmpty(arguments.Path?.Value))
                throw new ArgumentNullException("Path cannot be empty");

            pdf.Save(arguments.Path.Value, arguments.Password?.Value, arguments.OwnerPassword?.Value);
        }
    }
}
