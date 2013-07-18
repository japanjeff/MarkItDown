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
        MarkItDown.exe [-v] [-t TEMPLATE] FILES ... 
        MarkItDown.exe -h | --help
        MarkItDown.exe --version

      Options:
        FILES                   Markdown files to be converted to HTML
        -t --template TEMPLATE  Template HTML file to use (will replace 
                                the token {{ MarkItHere }} with Markdown)
        -v --verbose            Show more detail for errors
        -h --help               Show this screen and exit
        --version               Show version and exit
        
      ";

    static void Main(string[] args)
    {
      string templateHtml = null;
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

        if (arguments["--template"] != null)
        {
          var templateFilename = arguments["--template"].ToString();
          var templatePath = Path.GetFullPath(templateFilename);
          templateHtml = File.ReadAllText(templatePath);
        }

        var md = new Markdown();
        var fileNames = arguments["FILES"].AsList;

        foreach (var fileName in fileNames)
        {
          var inputPath = Path.GetFullPath(fileName.ToString());
          var markdown = File.ReadAllText(inputPath);
          var html = md.Transform(markdown);
          var outPath = GetOutputPath(inputPath);

          if (templateHtml != null)
          {
            html = templateHtml.Replace("{{ MarkItHere }}", html);
          }
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
