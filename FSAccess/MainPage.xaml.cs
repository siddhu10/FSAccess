using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            strStartPath = startPath.Text;
            strtFolder = await StorageFolder.GetFolderFromPathAsync(strStartPath);
            txtBox.Text = strtFolder.Path;
        }

        private async void folderBox_Click(object sender, RoutedEventArgs e)
        {
            newFolder = await strtFolder.CreateFolderAsync(folderName.Text, CreationCollisionOption.ReplaceExisting);
            txtBox2.Text = newFolder.Path;
        }

        private async void fileBox_Click(object sender, RoutedEventArgs e)
        {
            newFile = await strtFolder.CreateFileAsync(fileName.Text, CreationCollisionOption.ReplaceExisting);
            txtBox2.Text = newFile.Path;
        }

        private async void propBox_Click(object sender, RoutedEventArgs e)
        {
            strFolderProp = string.Empty;
            strFileProp = string.Empty;

            if (newFolder != null)
            {
                BasicProperties basicProperties = await newFolder.GetBasicPropertiesAsync();
                DocumentProperties docProp = await newFolder.Properties.GetDocumentPropertiesAsync();

                strFolderProp += "Date Created: " + newFolder.DateCreated + "\r\n" +
                                 "Type: " + newFolder.DisplayType + "\r\n" +                
                                 "Date Modified: " + basicProperties.DateModified + "\r\n" +
                                 "Size: " + basicProperties.Size;
            }

            if (newFile != null)
            {
                BasicProperties basicProperties = await newFile.GetBasicPropertiesAsync();
                DocumentProperties docProp = await newFile.Properties.GetDocumentPropertiesAsync();

                strFolderProp += "Date Created: " + newFile.DateCreated + "\r\n" +
                                 "Type: " + newFile.DisplayType + "\r\n" +
                                 "File Type: " + newFile.FileType + "\r\n" +
                                 "Content Type: " + newFile.ContentType + "\r\n" +
                                 "Date Modified: " + basicProperties.DateModified + "\r\n" +
                                 "Size: " + basicProperties.Size;
            }

            string strList = string.Empty;
            var list = await strtFolder.GetItemsAsync();
            foreach (IStorageItem item in list)
            {
                strList += item.Name + "\r\n";
            }

            propArea.Text = strFolderProp + "\r\n \r\n" + strFileProp + "\r\n" + strList;
        }

        private async void browseBox_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            folderPicker.ViewMode = PickerViewMode.Thumbnail;

            StorageFolder pickedFolder = await folderPicker.PickSingleFolderAsync();
            strtFolder = await StorageFolder.GetFolderFromPathAsync(pickedFolder.Path);
            startPath.Text = pickedFolder.Path;
        }
    }
}
