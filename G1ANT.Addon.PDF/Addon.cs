﻿using G1ANT.Addon.PDF;
using G1ANT.Language;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

// Please remember to refresh G1ANT.Language.dll in references

namespace G1ANT.Addon.Pdf
{
    [Addon(Name = "PDF", Tooltip = "Addon for mantaining PDF files")]
    [Copyright(Author = "G1ANT LTD", Copyright = "G1ANT LTD", Email = "hi@g1ant.com", Website = "www.g1ant.com")]
    [License(Type = "LGPL", ResourceName = "License.txt")]
    //[CommandGroup(Name = "", Tooltip = "")]
    public class Addon : Language.Addon
    {

        public override void Check()
        {
            base.Check();
            // Check integrity of your Addon
            // Throw exception if this Addon needs something that doesn't exists
        }

        public override void LoadDlls()
        {
            base.LoadDlls();
            // All dlls embeded in resources will be loaded automatically,
            // but you can load here some additional dlls:

            // Assembly.Load("...")
            OcrOfflineHelper.UnpackNeededAssemblies();
        }

        public override void Initialize()
        {
            base.Initialize();
            // Insert some code here to initialize Addon's objects
        }

        public override void Dispose()
        {
            base.Dispose();
            // Insert some code here which will dispose all unecessary objects when this Addon will be unloaded
        }
    }
}