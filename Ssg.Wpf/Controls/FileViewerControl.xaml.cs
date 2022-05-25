using Microsoft.Extensions.Configuration;
using Ssg.Wpf.Data;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;


namespace Ssg.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for FileViewerControl.xaml
    /// </summary>
    public partial class FileViewerControl : UserControl
    {
        private readonly IConfiguration _config;

        private string inputDirectory;
        private string outputDirectory;
        private string _selectedFilename;

        public List<string> Filenames { get; set; } = new List<string>();
        private string selectedFilename 
        { 
            get => _selectedFilename;
            set 
            { 
                _selectedFilename = value; 
                _config.GetSection("folders").GetSection("lastSelectedFile").Value = value;
                DbFile.SaveSelectedFile(value);
            }
        }

        public FileViewerControl(IConfiguration config)
        {
            _config = config;

            InitializeComponent();


            outputDirectory = _config.GetSection("folders").GetSection("outputHtmlFiles").Value;

            string[] inputDirectoryFiles = Directory.GetFiles(outputDirectory);

            foreach (string inputFilePath in inputDirectoryFiles)
            {
                Filenames.Add(Path.GetFileName(inputFilePath));
            }

            //txtFileNames.Text = Filenames.ToString();
            fileNames.ItemsSource = Filenames;

            string selectedFilename = DbFile.LoadSelectedFile();

            if (!string.IsNullOrEmpty(selectedFilename))
            {
                LoadHtmlFile(selectedFilename);
                HideSidebar();
            }
            else
            {
                btnToggleSidebar.Visibility = Visibility.Collapsed;
            }


        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var item = fileNames.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(item))
            {
                string fileName = Path.Combine(outputDirectory, item);
                if (fileName != null)
                {
                    selectedFilename = item;
                    LoadHtmlFile(selectedFilename);
                    HideSidebar();
                }

            }
        }

        private void LoadHtmlFile(string fileName)
        {
            string filePath = Path.Combine(outputDirectory, fileName);
            string content = File.ReadAllText(filePath);

            htmlContent.NavigateToString(content);

            txtFileName.Text = fileName;

        }

        private void HideSidebar()
        {
            fileNames.Visibility = Visibility.Collapsed;
            btnToggleSidebar.Visibility = Visibility.Visible;
            btnToggleSidebar.Content = "Change File";
            btnOpenFile.Content = "Reload";
        }

        private void btnToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            if (fileNames.Visibility == Visibility.Visible)
            {
                HideSidebar();
            }
            else
            {
                fileNames.Visibility = Visibility.Visible;
                btnToggleSidebar.Content = "Hide";
                btnOpenFile.Content = "Open File";
            }
        }
    }
}
