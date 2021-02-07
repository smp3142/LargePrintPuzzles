using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

// IMPORTANT NOTE: I recommend against using this file as any sort of reference. This is just a copy
// of a WPF GUI rather than using an idiomatic Avalonia design.

namespace LPPava
{
    public class MainWindow : Window
    {
        private readonly CheckBox chkAutoOpenPDF;
        private readonly TextBlock txtFileStatus;
        private readonly TextBox txtPathAndFile;
        private readonly ProgressBar progressBar;
        private readonly ComboBox cmbGameType;
        private readonly ComboBox cmbDifficulty;
        private readonly ComboBox cmbNumberOfPages;
        private readonly Button btnMakePuzzle;

        private readonly string basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            txtPathAndFile = this.Find<TextBox>("TxtPathAndFile");
            progressBar = this.Find<ProgressBar>("IndProgressBar");
            txtFileStatus = this.Find<TextBlock>("TxtFileStatus");
            chkAutoOpenPDF = this.Find<CheckBox>("AutoOpenPDF");
            cmbGameType = this.Find<ComboBox>("GameType");
            cmbDifficulty = this.Find<ComboBox>("Difficulty");
            cmbNumberOfPages = this.Find<ComboBox>("NumberOfPages");
            btnMakePuzzle = this.Find<Button>("BtnMakePuzzle");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void MakePuzzle_Click(object sender, RoutedEventArgs e)
        {
            Exception? fileCreationException = null;

            btnMakePuzzle.IsEnabled = false;
            txtFileStatus.Text = "Making File";
            txtPathAndFile.IsVisible = false;
            progressBar.IsVisible = true;

            byte[]? byteArray = null;
            string? pdfFileName = null;
            string? pdfFullPath = null;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
            ComboBoxItem cmbItem;
            cmbItem = (ComboBoxItem)cmbGameType.SelectedItem;
            string gameType = cmbItem.Content.ToString();

            cmbItem = (ComboBoxItem)cmbDifficulty.SelectedItem;
            string difficulty = cmbItem.Content.ToString();

            cmbItem = (ComboBoxItem)cmbNumberOfPages.SelectedItem;
            int numberOfPages = int.Parse(cmbItem.Content.ToString());
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            await Task.Factory.StartNew(() =>
            {
                switch (gameType)
                {
                    case "Clueless Crosswords":
                        pdfFileName = "LPP-CluelessCrosswords.pdf";
                        pdfFullPath = System.IO.Path.Combine(basePath, pdfFileName);
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

                        try { File.WriteAllBytes(pdfFullPath, byteArray); }
                        catch (Exception ex) when (ex is ArgumentException or PathTooLongException or DirectoryNotFoundException or IOException or UnauthorizedAccessException)
                        {
                            fileCreationException = ex;
                            pdfFileName = null;
                        }
                        break;

                    case "Clueless Word List":
                        pdfFileName = "LPP-ShortWords.pdf"; pdfFullPath = System.IO.Path.Combine(basePath, pdfFileName);
                        byteArray = CluelessCrossword.PDF.Produce.ShortWords(); try
                        {
                            File.WriteAllBytes(pdfFullPath, byteArray);
                        }
                        catch (Exception ex) when (ex is ArgumentException or PathTooLongException or DirectoryNotFoundException or IOException or UnauthorizedAccessException)
                        {
                            fileCreationException = ex;
                            pdfFileName = null;
                        }
                        break;

                    default:
                        break;
                }
            }
                );

            btnMakePuzzle.IsEnabled = true;
            txtFileStatus.Text = "Ready";
            progressBar.IsVisible = false;

            if (fileCreationException != null)
            {
                txtPathAndFile.Text = fileCreationException.Message;
                txtPathAndFile.IsVisible = true;
            }
            else if (pdfFileName != null)
            {
                txtFileStatus.Text = "PDF File";
                txtPathAndFile.Text = basePath + '\n' + pdfFileName;
                txtPathAndFile.IsVisible = true;
                if (chkAutoOpenPDF.IsChecked.GetValueOrDefault()) { OpenPdf(pdfFileName); }
            }

            e.Handled = true;
        }

        private void OpenPdf(string pdfFileName)
        {
            string? pdfFullPath = System.IO.Path.Combine(basePath, pdfFileName);

            if (!Directory.Exists(Path.GetDirectoryName(pdfFullPath)))
            {
                txtPathAndFile.Text = $"Unable to access\n{Path.GetDirectoryName(pdfFullPath)}";
                return;
            }
            if (!File.Exists(Path.GetFullPath(pdfFullPath)))
            {
                txtPathAndFile.Text = $"Unable to access\n{Path.GetFullPath(pdfFullPath)}";
                return;
            }
            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            string temp = "file:///" + pdfFullPath.Replace(@"\\", "/");
            process.StartInfo.FileName = temp;
            process.Start();
        }
    }
}
