using Markdig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ssg.Core
{
    public class MarkdownToHtml
    {
        public static void ConvertDirectoryFiles(string inputDirectory, string outputDirectory) 
        {
            string[] inputDirectoryFiles = Directory.GetFiles(inputDirectory);

            foreach (string inputFilePath in inputDirectoryFiles)
            {
                string ext = Path.GetExtension(inputFilePath);

                if (ext == ".md")
                {
                    // var dt = File.GetLastWriteTime(inputFile);
                    string outputFileName = Path.GetFileName(inputFilePath).Replace(".md", ".html");
                    string outputFilePath = Path.Combine(outputDirectory, outputFileName);

                    GenerateHtmlFile(inputFilePath, outputFilePath);

                }
            }
        }

        private static void GenerateHtmlFile(string inputFile, string outputFile)
        {
            string fileType = "post";

            string inputContentMd = File.ReadAllText(inputFile);
            string outputContentHtml = Markdown.ToHtml(inputContentMd);

            // Modify HTML
            // Correct links href
            outputContentHtml = outputContentHtml.Replace(@".md"">", @".html"">");
            
            var outputHTML = GetLayoutFileContent(fileType);

            int index = outputHTML.IndexOf("{{content}}");
            outputHTML.RemoveAt(index);
            outputHTML.Insert(index, outputContentHtml);

            File.WriteAllLines(outputFile, outputHTML.ToArray());

            if (fileType.ToLower() == "post")
                GenerateSidebarToc(outputFile);
        }

        private static void GenerateSidebarToc(string outputFile)
        {
            string[] lines = File.ReadAllLines(outputFile);
            List<string> tocElements = new List<string>();

            int countH1 = 0;
            int countH2 = 0;

            tocElements.Add("<ul>");

            for (int i = 0; i < lines.Length; i++)
            {

                if (lines[i].StartsWith("<h1>") || lines[i].StartsWith("<h2>"))
                {
                    string element = lines[i].Substring(1, 2);

                    // Extract title
                    string title = lines[i].Replace($"<{element}>", "").Replace($"</{element}>", "");

                    // add id to the HTML element
                    string id = "";
                    if (element == "h1")
                    {
                        countH1++;
                        id = $"{element}-{countH1}";
                    } 
                    else
                    {
                        countH2++;
                        id = $"{element}-{countH2}";
                    }
                    lines[i] = lines[i].Replace($"<{element}>", $"<{element} id=\"{id}\">");

                    // add link in the TOC
                    tocElements.Add($"<li class=\"toc-{element}\"><a href=\"#{id}\">{title}</a></li>");
                }

            }

            tocElements.Add("</ul>");

            // Insert the TOC
            string tocHTML = "";

            foreach (string element in tocElements)
            {
                tocHTML += $"{element}\n";
            }

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "{{sidebar-toc}}")
                {
                    lines[i] = tocHTML;
                }
            }

            File.WriteAllLines(outputFile, lines);

        }

        private static List<string> GetLayoutFileContent(string layoutType)
        {
            // Get working Templates directories
            //string workingDirectory = Environment.CurrentDirectory;            
            //string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            //string projectDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string layoutFilename = $"{layoutType.ToLower()}.html";
            string layoutFile = Path.Combine("Templates", layoutFilename);
            string cssFile = Path.Combine("Templates", "assets", "main.css");

            // Get HTML Template content
            var layoutContent = File.ReadAllLines(layoutFile).ToList();
            var cssContent = File.ReadAllText(cssFile);

            // Insert CSS from file
            int indexCss = layoutContent.IndexOf("{{css}}");
            layoutContent.RemoveAt(indexCss);
            layoutContent.Insert(indexCss, cssContent);

            // Insert JS from file
            //int indexJs = layoutContent.IndexOf("{{javascript}}");
            //layoutContent.RemoveAt(indexCss);
            //layoutContent.Insert(indexCss, cssContent);

            return layoutContent;

        }
    }
}
