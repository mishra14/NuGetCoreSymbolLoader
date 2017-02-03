using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Packaging;

namespace NuGetCoreSymbolLoader
{
    class Program
    {
        private static int _numberOfThreads = 8;
        static void Main(string[] args)
        {
            if (args.Count() != 1)
            {
                Console.WriteLine("Please enter following arguments - ");
                Console.WriteLine("arg[0]: NuGet.CLient git repository root  path");
                Console.WriteLine("Exiting...");
                return;
            }

            // For testing
            //var rootPath = @"E:\NuGet.Client";

            var rootPath = args[0];
            var nupkgsPath = Path.Combine(rootPath, @"Nupkgs");
            var symbolsPath = Path.Combine(rootPath, @"artifacts", @"VS15", @"symbols");

            var symbolNupkgs = Directory.GetFiles(nupkgsPath, "NuGet.*.symbols.nupkg")
                .ToArray();

            var parentSrcPath = Path.Combine(symbolsPath, @"src");
            if (!Directory.Exists(parentSrcPath))
            {
                Directory.CreateDirectory(parentSrcPath);
            }

            ParallelOptions ops = new ParallelOptions { MaxDegreeOfParallelism = _numberOfThreads };
            Parallel.ForEach(symbolNupkgs, ops, symbolNupkg =>
            {
                try
                {
                    ExtractAndPlaceSymbolsAndSources(symbolNupkg, symbolsPath, parentSrcPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        private static void ExtractAndPlaceSymbolsAndSources(string nupkgPath, string symbolsPath, string parentSrcPath)
        {
            using (var archive = ZipFile.Open(nupkgPath, ZipArchiveMode.Read))
            {
                var symbolFiles = archive
                    .Entries
                    .Where(e => e.FullName.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase))
                    .Where(e => e.FullName.Contains("net45") || e.FullName.Contains("net46"))
                    .ToArray();

                if (symbolFiles.Count() > 0)
                {
                    foreach(var symbolFile in symbolFiles)
                    {
                        symbolFile.ExtractToFile(Path.Combine(symbolsPath, Path.GetFileName(symbolFile.FullName)), overwrite: true);
                    }
                }

                var srcFiles = archive
                    .Entries
                    .Where(e => e.FullName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase));

                if (srcFiles.Count() > 0)
                {
                    foreach (var srcFile in srcFiles)
                    {
                        var srcFileDir = Path.Combine(parentSrcPath, Path.GetFileName(nupkgPath));
                        var srcFilePath = Path.Combine(srcFileDir, Path.GetFileName(srcFile.FullName));
                        if (!Directory.Exists(srcFileDir))
                        {
                            Directory.CreateDirectory(srcFileDir);
                        }
                        srcFile.ExtractToFile(srcFilePath, overwrite: true);
                    }
                }
            }
        }
    }
}
