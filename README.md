MarkItDown
==========

.NET command line program that generates HTML files from Markdown files 

Usage
-----

To generate HTML files from some Markdown files, simply pass the names of the
Markdown files as command line arguments. The example below will produce
(and overwrite) `README.html` and `examples.html` in the same directory.

    > MarkItDown.exe README.md examples.md

Here's the doc string you'll get if you invoke MarkItDown with the `-h` option:

    Generates HTML files based on the contents of the Markdown files provided
    Given input README.md, it will create and overwrite README.html

    Usage:
      MarkItDown.exe [-v] [-t TEMPLATE] FILES ... 
      MarkItDown.exe -h | --help
      MarkItDown.exe --version

    Options:
      FILES           Markdown files to be converted to HTML
      -t --template   Template HTML file to use (the token {{ MarkItHere }} 
                      will be replaced with the Markdown generated)
      -v --verbose    Show more detail for errors
      -h --help       Show this screen
      --version       Show version
      
*Note:* The HTML generated is not a complete HTML document. Rather, it is
a HTML snippet you might put inside the `<body>` tag of an HTML document. You
can use a template file with the token `{{ MarkItHere }}` to create full HTML
documents. The token will be replaced with the HTML generated for the Markdown.

Dependencies
------------

Most of the work is done by the awesome packages listed below.

These are installed using NuGet so if you have NuGet setup to download missing
packages, it should automatically fetch them for you. Even without NuGet,
it should build with msbuild if you set the environment variable
'EnableNuGetPackageRestore' to 'true'.

- [MarkdownSharp](https://code.google.com/p/markdownsharp/)
- [docopt.net](https://github.com/docopt/docopt.net)

Change Log
---------

MarkItDown uses [semantic versioning](http://semver.org/).

- 1.1.0.0
  - Added an option that lets you provide a template to insert the generated
    Markdown into.
- 1.0.0.0
  - Initial release

License
-------

This software is released under the MIT License. See `LICENSE-MIT` for details.
