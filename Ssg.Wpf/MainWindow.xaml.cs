using Ssg.Wpf.Controls;
using System.Windows;
using Microsoft.Win32;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Ssg.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IConfiguration _config;


        public MainWindow(IConfiguration config)
        {
            InitializeComponent();

            _config = config;

            content.Content = new FileViewerControl(_config);
            
        }

        private void menuMarkdownnToHtml_Click(object sender, RoutedEventArgs e)
        {
            content.Content = new MarkdownToHtmlControl(_config);
        }

        private void menuOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //if (openFileDialog.ShowDialog() == true)
            //    MessageBox.Show(openFileDialog.FileName);

            content.Content = new FileViewerControl(_config);

        }




    }
}
