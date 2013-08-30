using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                                (File globs using */? also accepted)
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
          if (verbose) Console.WriteLine("Using template " + templatePath);
        }

        var md = new Markdown();
        var paths = arguments["FILES"].AsList.ToArray();
        var files = paths.SelectMany(x => GlobFiles(x.ToString()));

        foreach (var fileName in files)
        {
          if(verbose) Console.WriteLine("Processing " + fileName);
          var markdown = File.ReadAllText(fileName);
          var html = md.Transform(markdown);
          var outPath = GetOutputPath(fileName);

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
      return Path.Combine(directory ?? "", baseName + ".html");
    }

    /// <remarks>
    /// Based on http://stackoverflow.com/a/7619345/945456
    /// and http://stackoverflow.com/q/15961648/945456
    /// </remarks>
    private static IEnumerable<String> GlobFiles(string path)
    {
      string directory = Path.GetDirectoryName(path);
      string filePattern = Path.GetFileName(path) ?? "";

      if (String.IsNullOrEmpty(directory))
        directory = Directory.GetCurrentDirectory();

      directory = Path.GetFullPath(directory);

      if (!Directory.Exists(directory))
        return Enumerable.Empty<String>();

      var extension = Path.GetExtension(filePattern) ?? "";
      return Directory.EnumerateFiles(directory, filePattern)
        .Where(x => x.EndsWith(extension));
    }
  }
}
