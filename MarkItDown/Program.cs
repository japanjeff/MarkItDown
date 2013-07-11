using System;
using System.IO;
using System.Reflection;
using DocoptNet;
using MarkdownSharp;

namespace MarkItDown
{
  class Program
  {
    private const string Usage = @"MarkItDown
      Generates HTML files based on the contents of the Markdown files provided
      Given input README.md, it will create and overwrite README.html

      Usage:
        MarkItDown.exe [-v | --verbose] FILES ... 
        MarkItDown.exe (-h | --help)
        MarkItDown.exe --version

      Options:
        FILES         Markdown files to be converted to HTML
        -h --help     Show this screen
        -v --verbose  Show more detail for errors
        --version     Show version
        
      ";

    static void Main(string[] args)
    {
      var verbose = false;

      try
      {
        var arguments = new Docopt().Apply(Usage, args);
        verbose = arguments["--verbose"].IsTrue;

        if (arguments["--version"].IsTrue)
        {
          Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Version);
          return;
        }

        var md = new Markdown();
        var fileNames = arguments["FILES"].AsList;

        foreach (var fileName in fileNames)
        {
          var inputPath = Path.GetFullPath(fileName.ToString());
          var markdown = File.ReadAllText(inputPath);
          var html = md.Transform(markdown);
          var outPath = GetOutputPath(inputPath);
          File.WriteAllText(outPath, html);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(verbose ? ex.ToString() : ex.Message);
      }
    }

    private static string GetOutputPath(string inputPath)
    {
      var baseName = Path.GetFileNameWithoutExtension(inputPath);
      var directory = Path.GetDirectoryName(inputPath);
      return Path.Combine(directory, baseName + ".html");
    }
  }
}
