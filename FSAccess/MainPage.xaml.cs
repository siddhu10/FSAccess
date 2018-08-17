using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FSAccess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string strStartPath = string.Empty;
        string strFolderProp = string.Empty;
        string strFileProp = string.Empty;

        StorageFolder strtFolder;
        StorageFolder newFolder;
        StorageFile newFile;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void accessBox_Click(object sender, RoutedEventArgs e)
        {
            progBar.Visibility = Visibility.Visible;
            fileListArea.IsReadOnly = false;

            if (!(string.IsNullOrEmpty(startPath.Text)))
            {
                strStartPath = startPath.Text;
                strtFolder = await StorageFolder.GetFolderFromPathAsync(strStartPath);
                txtBox.Text = strtFolder.Path;

                string strList = await GetFileList();
                fileListArea.Document.SetText(TextSetOptions.None, strList);
            }
            else
            {
                ContentDialog dialog = Helper.GetDialog();
                dialog.Content = Helper.GetResourceString("ID_SRCPATH_MSG");
                await dialog.ShowAsync();
            }
            
            fileListArea.IsReadOnly = true;
            progBar.Visibility = Visibility.Collapsed;
        }

        private async void folderBox_Click(object sender, RoutedEventArgs e)
        {
            if (strtFolder != null)
            {
                if (!(string.IsNullOrEmpty(folderName.Text)))
                {
                    newFolder = await strtFolder.CreateFolderAsync(folderName.Text, CreationCollisionOption.GenerateUniqueName);
                    txtBox2.Text = newFolder.Path;

                    refBtn_Click(sender, e);
                }
                else
                {
                    ContentDialog dialog = Helper.GetDialog();
                    dialog.Content = Helper.GetResourceString("ID_FOLDER_MSG");
                    await dialog.ShowAsync();
                }
            }
            else
            {
                ContentDialog dialog = Helper.GetDialog();
                dialog.Content = Helper.GetResourceString("ID_SRCPATH_MSG");
                await dialog.ShowAsync();
            }
        }

        private async void fileBox_Click(object sender, RoutedEventArgs e)
        {
            if (strtFolder != null)
            {
                if (!(string.IsNullOrEmpty(fileName.Text)))
                {
                    newFile = await strtFolder.CreateFileAsync(fileName.Text, CreationCollisionOption.GenerateUniqueName);
                    txtBox2.Text = newFile.Path;

                    refBtn_Click(sender, e);
                }
                else
                {
                    ContentDialog dialog = Helper.GetDialog();
                    dialog.Content = Helper.GetResourceString("ID_FILE_MSG");
                    await dialog.ShowAsync();
                }
            }
            else
            {
                ContentDialog dialog = Helper.GetDialog();
                dialog.Content = Helper.GetResourceString("ID_SRCPATH_MSG");
                await dialog.ShowAsync();
            }
        }

        private async void propBox_Click(object sender, RoutedEventArgs e)
        {
            strFolderProp = string.Empty;
            strFileProp = string.Empty;

            filePropArea.IsReadOnly = false;
            foldrPropArea.IsReadOnly = false;

            if (newFolder != null)
            {
                BasicProperties basicProperties = await newFolder.GetBasicPropertiesAsync();
                DocumentProperties docProp = await newFolder.Properties.GetDocumentPropertiesAsync();

                strFolderProp += Helper.GetResourceString("ID_FLDR_NAME") + " - " + newFolder.Name + "\r\n" + "\r\n" +
                                 Helper.GetResourceString("ID_DATE_CRT") + ": " + newFolder.DateCreated + "\r\n" +
                                 Helper.GetResourceString("ID_TYPE") + ": " + newFolder.DisplayType + "\r\n" +
                                 Helper.GetResourceString("ID_DATE_MDF") + ": " + basicProperties.DateModified + "\r\n" +
                                 Helper.GetResourceString("ID_SIZE") + ": " + basicProperties.Size;

                foldrPropArea.Document.SetText(TextSetOptions.None, strFolderProp);
            }
            else
            {
                ContentDialog dialog = Helper.GetDialog();
                dialog.Content = Helper.GetResourceString("ID_FLDR_PROP_MSG");
                await dialog.ShowAsync();
            }

            if (newFile != null)
            {
                BasicProperties basicProperties = await newFile.GetBasicPropertiesAsync();
                DocumentProperties docProp = await newFile.Properties.GetDocumentPropertiesAsync();

                strFileProp += Helper.GetResourceString("ID_FILE_NAME") + " - " + newFile.Name + "\r\n" + "\r\n" +
                               Helper.GetResourceString("ID_DATE_CRT") + ": " + newFile.DateCreated + "\r\n" +
                               Helper.GetResourceString("ID_TYPE") + ": " + newFile.DisplayType + "\r\n" +
                               Helper.GetResourceString("ID_FILE_TYPE") + ": " + newFile.FileType + "\r\n" +
                               Helper.GetResourceString("ID_CONTENT_TYPE") + ": " + newFile.ContentType + "\r\n" +
                               Helper.GetResourceString("ID_DATE_MDF") + ": " + basicProperties.DateModified + "\r\n" +
                               Helper.GetResourceString("ID_SIZE") + ": " + basicProperties.Size;

                filePropArea.Document.SetText(TextSetOptions.None, strFileProp);
            }
            else
            {
                ContentDialog dialog = Helper.GetDialog();
                dialog.Content = Helper.GetResourceString("ID_FILE_PROP_MSG");
                await dialog.ShowAsync();
            }

            filePropArea.IsReadOnly = true;
            foldrPropArea.IsReadOnly = true;
        }

        private async void browseBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderPicker folderPicker = new FolderPicker();
                folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
                folderPicker.ViewMode = PickerViewMode.Thumbnail;
                folderPicker.FileTypeFilter.Add(".txt");

                StorageFolder storageFolder = await folderPicker.PickSingleFolderAsync();
                strtFolder = await StorageFolder.GetFolderFromPathAsync(storageFolder.Path);
                startPath.Text = strtFolder.Path;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception Handled: " + ex.Message);
            }
        }

        private async Task<string> GetFileList()
        {
            string strList = string.Empty;
            var list = await strtFolder.GetItemsAsync();
            foreach (IStorageItem item in list)
            {
                strList += item.Name + "\r\n";
            }
            strList.Trim();
            return strList;
        }

        private async void refBtn_Click(object sender, RoutedEventArgs e)
        {
            if (strtFolder != null)
            {
                progBar.Visibility = Visibility.Visible;
                fileListArea.IsReadOnly = false;

                fileListArea.Document.SetText(TextSetOptions.None, string.Empty);
                string strList = await GetFileList();
                fileListArea.Document.SetText(TextSetOptions.None, strList);

                fileListArea.IsReadOnly = true;
                progBar.Visibility = Visibility.Collapsed;
            }
        }
    }
}
