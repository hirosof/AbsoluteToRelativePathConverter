using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AbsoluteToRelativePathConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

            this.AdditionalBaseFolderPath(Environment.CurrentDirectory);

        }

        private void AdditionalBaseFolderPath(string path) {

            int index = BaseFolderPathComboBox.Items.IndexOf(path);

            if (index == -1) {
                BaseFolderPathComboBox.Items.Insert(0, path);
                BaseFolderPathComboBox.SelectedIndex = 0;
            } else {
                BaseFolderPathComboBox.SelectedIndex = index;
            }

            UpdateRelativePath();
            ImmediatelyAfterBaseFolderChanged = true;

        }


        private void BaseFolderReferenceButton_Click(object sender, RoutedEventArgs e) {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();

            if (Directory.Exists(BaseFolderPathComboBox.Text)) {
                openFolderDialog.DefaultDirectory = BaseFolderPathComboBox.Text;
            } else {
                openFolderDialog.DefaultDirectory = Environment.CurrentDirectory;
            }

            openFolderDialog.InitialDirectory = openFolderDialog.DefaultDirectory;

            if (openFolderDialog.ShowDialog() == true) {
                AdditionalBaseFolderPath(openFolderDialog.FolderName);
            }

        }

        private void TargetPathInputTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            UpdateRelativePath();
        }

        private void UpdateRelativePath() {

            string? basePath = BaseFolderPathComboBox.SelectedValue.ToString();

            if ((TargetPathInputTextBox.Text.Length == 0)||(basePath ==null)) {
                RelativePathOutputTextBox.Text = "";
                return;
            }

            StringReader reader = new StringReader(TargetPathInputTextBox.Text);

            StringBuilder sb = new StringBuilder();

            string? item;

            while ((item = reader.ReadLine()) != null) {
                item = item.Trim().Trim('\"').Trim();

                if (item.Length == 0) {
                    sb.AppendLine();
                    continue;
                }

                if (System.IO.Path.IsPathRooted(item)) {
                    sb.AppendLine(System.IO.Path.GetRelativePath(basePath, item));
                } else {
                    sb.AppendLine(item);
                }
            }

            RelativePathOutputTextBox.Text = sb.ToString();
        }

        private void BaseFolderPathComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            UpdateRelativePath();
            ImmediatelyAfterBaseFolderChanged = true;
        }


        private string finalDirectoryForTargetPathSelectDialog = string.Empty;
        private bool ImmediatelyAfterBaseFolderChanged = false;

        private void TargetPathInputAdditionalByFiles_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if((!ImmediatelyAfterBaseFolderChanged) &&(finalDirectoryForTargetPathSelectDialog.Length>0) && Directory.Exists(finalDirectoryForTargetPathSelectDialog)) {
                openFileDialog.DefaultDirectory = finalDirectoryForTargetPathSelectDialog;
            } else if (Directory.Exists(BaseFolderPathComboBox.Text)) {
                openFileDialog.DefaultDirectory = BaseFolderPathComboBox.Text;
            } else {
                openFileDialog.DefaultDirectory = Environment.CurrentDirectory;
            }

            openFileDialog.InitialDirectory = openFileDialog.DefaultDirectory;

            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true) {

                if (TargetPathInputTextBox.Text.Length > 0) {
                    if (TargetPathInputTextBox.GetLineLength(TargetPathInputTextBox.LineCount - 1) != 0) {
                        TargetPathInputTextBox.AppendText(Environment.NewLine);
                    }
                }

                TargetPathInputTextBox.AppendText(string.Join(Environment.NewLine, openFileDialog.FileNames));
                finalDirectoryForTargetPathSelectDialog = GetParentDirectory(openFileDialog.FileName);
                ImmediatelyAfterBaseFolderChanged = false;
            }

        }

        private void TargetPathInputAdditionalByFolders_Click(object sender, RoutedEventArgs e) {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();

            if ((!ImmediatelyAfterBaseFolderChanged) &&(finalDirectoryForTargetPathSelectDialog.Length > 0) && Directory.Exists(finalDirectoryForTargetPathSelectDialog)) {
                openFolderDialog.DefaultDirectory = finalDirectoryForTargetPathSelectDialog;
            } else if (Directory.Exists(BaseFolderPathComboBox.Text)) {
                openFolderDialog.DefaultDirectory = BaseFolderPathComboBox.Text;
            } else {
                openFolderDialog.DefaultDirectory = Environment.CurrentDirectory;
            }

            openFolderDialog.InitialDirectory = openFolderDialog.DefaultDirectory;

            openFolderDialog.Multiselect = true;

            if (openFolderDialog.ShowDialog() == true) {


                if (TargetPathInputTextBox.Text.Length > 0) {
                    if (TargetPathInputTextBox.GetLineLength(TargetPathInputTextBox.LineCount - 1) != 0) {
                        TargetPathInputTextBox.AppendText(Environment.NewLine);
                    }
                }

                TargetPathInputTextBox.AppendText(string.Join(Environment.NewLine, openFolderDialog.FolderNames));

                finalDirectoryForTargetPathSelectDialog = GetParentDirectory(openFolderDialog.FolderName);
                ImmediatelyAfterBaseFolderChanged = false;

            }
        }


        private string GetParentDirectory(string path) {

            var parentDirectory = System.IO.Path.GetDirectoryName(path);

            if (parentDirectory == null) {
                return string.Empty;
            }

            return parentDirectory;
        }

        private void TargetPathInputClear_Click(object sender, RoutedEventArgs e) {
            TargetPathInputTextBox.Clear();
        }
    }
}