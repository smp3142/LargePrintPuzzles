using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace LargePrintPuzzles.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int[] numberOfPages;

        public MainWindow()
        {
            InitializeComponent();
            numberOfPages = new int[20];
            for (int i = 0; i < numberOfPages.Length; i++)
            {
                numberOfPages[i] = i + 1;
            }
            NumberOfPages.ItemsSource = numberOfPages;
            NumberOfPages.SelectedIndex = 19;
        }

        private async void MakePuzzle_Click(object sender, RoutedEventArgs e)
        {
            btnMakePuzzle.IsEnabled = false;
            txtFileStatus.Text = "Making File";
            fileUrlDisplay.Visibility = Visibility.Hidden;
            progressBar.Visibility = Visibility.Visible;

            byte[] byteArray = null;
            string pdfFileName = null;
            string pdfFullPath = null;
            string gameType = GameType.Text;
            string difficulty = Difficulty.Text;
            int numberOfPages = int.Parse(NumberOfPages.Text);

            await Task.Factory.StartNew(() =>
            {
                switch (gameType)
                {
                    case "Clueless Crosswords":
                        pdfFileName = "LPP-CluelessCrosswords.pdf";
                        pdfFullPath = System.IO.Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            pdfFileName);
                        CluelessCrossword.Difficulty cluelessDifficulty = difficulty switch
                        {
                            "Easy" => CluelessCrossword.Difficulty.Easy,
                            "Normal" => CluelessCrossword.Difficulty.Normal,
                            "Hard" => CluelessCrossword.Difficulty.Hard,
                            _ => throw new NotImplementedException(),
                        };
                        var clueless = new CluelessCrossword.CluelessCrosswords(
                            numberOfPages,
                            cluelessDifficulty);

                        byteArray = CluelessCrossword.PDF.Produce.PDF(clueless);

                        try
                        {
                            File.WriteAllBytes(pdfFullPath, byteArray);
                        }
                        catch (Exception ex) when (ex is ArgumentException ||
                                                   ex is PathTooLongException ||
                                                   ex is DirectoryNotFoundException ||
                                                   ex is IOException ||
                                                   ex is UnauthorizedAccessException)
                        {
                            MessageBox.Show($"Error Creating {pdfFullPath}\n{ex.Message}", "Error");
                            pdfFileName = null;
                            return;
                        }

                        break;

                    case "Clueless Word List":
                        pdfFileName = "LPP-ShortWords.pdf";
                        pdfFullPath = System.IO.Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            pdfFileName);
                        byteArray = CluelessCrossword.PDF.Produce.ShortWords();
                        try
                        {
                            File.WriteAllBytes(pdfFullPath, byteArray);
                        }
                        catch (Exception ex) when (ex is ArgumentException ||
                                                   ex is PathTooLongException ||
                                                   ex is DirectoryNotFoundException ||
                                                   ex is IOException ||
                                                   ex is UnauthorizedAccessException)
                        {
                            MessageBox.Show($"Error Creating {pdfFullPath}\n{ex.Message}", "Error");
                            pdfFileName = null;
                            return;
                        }
                        break;

                    default:
                        break;
                }
            }
                );

            btnMakePuzzle.IsEnabled = true;
            txtFileStatus.Text = "Ready";
            progressBar.Visibility = Visibility.Hidden;

            if (pdfFileName != null)
            {
                txtFileStatus.Text = "PDF File";
                UriBuilder uriBuilder = new UriBuilder(pdfFullPath);
                fileUrl.NavigateUri = uriBuilder.Uri;
                fileUrlDisplay.Visibility = Visibility.Visible;
                if (AutoOpenPDF.IsChecked.GetValueOrDefault()) { OpenPdf(); }
            }

            e.Handled = true;
        }

        private void FileUrl_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            OpenPdf();
            e.Handled = true;
        }

        private void OpenPdf()
        {
            string localFile = fileUrl.NavigateUri.LocalPath;
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(localFile)))
            {
                MessageBox.Show($"Unable to access {System.IO.Path.GetDirectoryName(localFile)}",
                                "Directory Access Error");
                return;
            }
            if (!File.Exists(localFile))
            {
                MessageBox.Show($"{localFile} has been moved, renamed, or deleted.",
                                "File Access Error");
                return;
            }
            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = fileUrl.NavigateUri.AbsoluteUri;
            process.Start();
        }
    }
}
