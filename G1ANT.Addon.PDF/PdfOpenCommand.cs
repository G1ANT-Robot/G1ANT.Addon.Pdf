using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;

namespace G1ANT.Addon.PDF
{
    [Command(Name = "pdf.open", Tooltip = "Command using to open pdf document. If document dosen't exist it will be created")]
    public class PdfOpenCommand : Command
    {

        public PdfOpenCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments: CommandArguments
        {
            // Pytanie co robimy jak nie ma pliku. Tworzymy nowy czy wyrzucamy blad?
            [Argument(Name = "path", Required = false, Tooltip = "Path to file. If file does not exist it will be created")]
            public TextStructure path { get; set; }
                    
            [Argument(Name = "create", Required = false, Tooltip = "If a file doesn’t exist, the command will create it")]
            public BooleanStructure CreateIfNotExist { get; set; }

            [Argument(Name = "result", Required =false, Tooltip = "Name of a variable where the ID number of this Excel instance will be stored")]
            // chyba jednak nie rozumiem
            // czy to ma sens? chyba tak, jeśli przekazujemy do komendy istniejącą strukturę
            public PdfStructure Result { get; set; } = new PdfStructure();

        }

        public void execute(Arguments arguments)
        {
            // przypisanie ale to przeciez bez sensu
           // var pdfFile = arguments.Result;

        }
    }
}
