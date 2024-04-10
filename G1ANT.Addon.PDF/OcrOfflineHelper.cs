using BitMiracle.Docotic.Pdf;
using G1ANT.Language;
using System;
using System.IO;
using System.Linq;

namespace G1ANT.Addon.PDF
{
    public static class OcrOfflineHelper
    {
        private static string tessdataFolder = "pdf.ocr.models";
        private static string projectModelPrefix = "pdf.ocr.models.";
        public static string OcrModelsFolder { get => Path.Combine(AbstractSettingsContainer.Instance.UserDocsAddonFolder.FullName, tessdataFolder); }

        public static void UnpackNeededAssemblies()
        {
            UnpackOcrModels();
            UnpackNeededAssemblies("x86");
            UnpackNeededAssemblies("x64");
            UnpackTesseract();
        }

        private static void UnpackOcrModels()
        {
            Directory.CreateDirectory(OcrModelsFolder);
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();
            var modelNames = resources.Where(x => x.Contains(".traineddata")).ToList();
            foreach (var modelName in modelNames)
            {
                var originalName = assembly.GetResourceNameWithoutPrefix(modelName);
                originalName = originalName.Replace(projectModelPrefix, "");
                var destFile = Path.Combine(OcrModelsFolder, originalName);
                if (!File.Exists(destFile))
                {
                    using (var resource = assembly.GetManifestResourceStream(modelName))
                    {
                        using (var file = new FileStream(destFile, FileMode.Create, FileAccess.Write))
                        {
                            resource.CopyTo(file);
                        }
                    }
                }
            }
        }

        private static void UnpackNeededAssemblies(string version)
        {
            var neededLibs = new string[] { "leptonica-1.82.0.dll", "tesseract50.dll" };
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var executingPath = Path.GetDirectoryName(assembly.Location);
            var dirPath = Path.Combine(executingPath, version);
            Directory.CreateDirectory(dirPath);
            var l = Directory.EnumerateFiles(dirPath);
            var resources = assembly.GetManifestResourceNames();

            foreach (var lib in neededLibs)
            {
                if (l.Where(x => x.Contains(lib)).SingleOrDefault() == null)
                {
                    var resName = resources.Where(x => x.Contains(version) && x.Contains(lib)).FirstOrDefault();
                    if (resName != null)
                    {
                        using (var resource = assembly.GetManifestResourceStream(resName))
                        {
                            using (var file = new FileStream(Path.Combine(dirPath, lib), FileMode.Create, FileAccess.Write))
                            {
                                resource.CopyTo(file);
                            }
                        }
                    }
                }
            }
        }

        private static void UnpackTesseract()
        {
            var executingPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var fileName = "Tesseract.dll";
            var fullPath = Path.Combine(executingPath, fileName);
            var resourceName = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(r => r.Contains(fileName)).FirstOrDefault();
            if (!File.Exists(fullPath))
            {
                using (var resource = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                using (var file = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }
        }
    }
}
