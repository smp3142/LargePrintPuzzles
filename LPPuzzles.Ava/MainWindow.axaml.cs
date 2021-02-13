using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

// Please note, this is just a copy of the WPF app and is not idiomatic Avalonia code.

namespace LPPuzzles.Ava
{
    public class MainWindow : Window
    {
        private readonly int[] numberOfPages;
        private readonly ComboBox NumberOfPages;
        private readonly ComboBox GameType;
        private readonly ComboBox Difficulty;
        private readonly CheckBox AutoOpenPDF;
        private readonly Button btnMakePuzzle;
        private readonly TextBlock txtAppStatus;
        private readonly TextBox txtFileCreation;
        private readonly ProgressBar progressBar;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            NumberOfPages = this.Find<ComboBox>("NumberOfPages");
            GameType = this.Find<ComboBox>("GameType");
            Difficulty = this.Find<ComboBox>("Difficulty");
            AutoOpenPDF = this.Find<CheckBox>("AutoOpenPDF");
            btnMakePuzzle = this.Find<Button>("btnMakePuzzle");
            txtAppStatus = this.Find<TextBlock>("txtAppStatus");
            txtFileCreation = this.Find<TextBox>("txtFileCreation");
            progressBar = this.Find<ProgressBar>("progressBar");

            numberOfPages = new int[20];
            for (int i = 0; i < numberOfPages.Length; i++)
            {
                numberOfPages[i] = i + 1;
            }
            NumberOfPages.Items = numberOfPages;
            NumberOfPages.SelectedIndex = 19;

            GameType.SelectedIndex = 0;
            Difficulty.SelectedIndex = 1;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void MakePuzzle_Click(object sender, RoutedEventArgs e)
        {
            btnMakePuzzle.IsEnabled = false;
            txtAppStatus.Text = "Making File";
            txtFileCreation.IsVisible = false;
            progressBar.IsVisible = true;

            byte[]? byteArray = null;
            string? pdfFileName = null;
            string? pdfFullPath = null;

            string? gameType = ComboBoxContent(GameType);
            string? difficulty = ComboBoxContent(Difficulty);
            int numberOfPages = NumberOfPages.SelectedIndex + 1;

            string? result = null;
            await Task.Factory.StartNew(() =>
            {
                switch (gameType)
                {
                    case "Clueless Crosswords":
                        pdfFileName = "LPP-CluelessCrosswords.pdf";
                        pdfFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                                   pdfFileName);
                        CluelessCrosswords.Difficulty cluelessDifficulty = difficulty switch
                        {
                            "Easy" => CluelessCrosswords.Difficulty.Easy,
                            "Normal" => CluelessCrosswords.Difficulty.Normal,
                            "Hard" => CluelessCrosswords.Difficulty.Hard,
                            _ => throw new NotImplementedException(),
                        };
                        var clueless = new CluelessCrosswords.Games(
                            numberOfPages,
                            cluelessDifficulty);

                        byteArray = CluelessCrosswords.PDF.Produce.GamesPDF(clueless, Properties.Resources.notomono);

                        try { File.WriteAllBytes(pdfFullPath, byteArray); }
                        catch (Exception ex) when (ex is ArgumentException or PathTooLongException
                                                   or DirectoryNotFoundException or IOException or UnauthorizedAccessException)
                        {
                            result = ex.Message;
                            pdfFileName = null;
                            return;
                        }
                        break;

                    case "Clueless Word List":
                        pdfFileName = "LPP-ShortWords.pdf";
                        pdfFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                                   pdfFileName);
                        byteArray = CluelessCrosswords.PDF.Produce.ShortWords(Properties.Resources.notomono);
                        try { File.WriteAllBytes(pdfFullPath, byteArray); }
                        catch (Exception ex) when (ex is ArgumentException or PathTooLongException
                                                   or DirectoryNotFoundException or IOException or UnauthorizedAccessException)
                        {
                            result = $"{ex.Message}";
                            pdfFileName = null;
                            return;
                        }
                        break;
                }
                result = null;
            }
                );

            btnMakePuzzle.IsEnabled = true;
            txtAppStatus.Text = "Ready";
            progressBar.IsVisible = false;

            if (result == null)
            {
                txtFileCreation.Text = $"File location\n{pdfFullPath}";
                txtFileCreation.IsVisible = true;
                if (AutoOpenPDF.IsChecked.GetValueOrDefault() && pdfFullPath != null) { OpenPdf(pdfFullPath); }
            }
            else
            {
                txtFileCreation.Text = result;
                txtFileCreation.IsVisible = true;
            }

            e.Handled = true;
        }

        private string? ComboBoxContent(ComboBox? combobox)
        {
            string? result = null;
            if (combobox == null) { return result; }
            ComboBoxItem? selected = (ComboBoxItem?)combobox.SelectedItem;
            if (selected == null) { return result; }
            result = (string?)selected.Content;
            return result;
        }

        private void OpenPdf(string pdfFullPath)
        {
            //string localFile = fileUrl.NavigateUri.LocalPath;
            //if (!Directory.Exists(System.IO.Path.GetDirectoryName(localFile)))
            //{
            //    MessageBox.Show($"Unable to access {System.IO.Path.GetDirectoryName(localFile)}",
            //                    "Directory Access Error");
            //    return;
            //}
            //if (!File.Exists(localFile))
            //{
            //    MessageBox.Show($"{localFile} has been moved, renamed, or deleted.",
            //                    "File Access Error");
            //    return;
            //}
            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = pdfFullPath;
            process.Start();
        }
    }
}
