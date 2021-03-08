using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Forms;


/*
 * opens dialogue for where to save screenshots
 */

namespace SnapShotApp
{
    class SelectFolder
    {
        public string SelectedFolder;

        public SelectFolder()
        {
            var chooseFolder = new CommonOpenFileDialog();
            chooseFolder.IsFolderPicker = true;
            chooseFolder.Title = "Choose output folder";
            
            if (chooseFolder.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SelectedFolder = chooseFolder.FileName;
            }
            if (SelectedFolder == null) //invalid CSV
            {
                MessageBox.Show("Please Select a Valid Folder");
                System.Environment.Exit(0);
            }
        }

        public void SelectDirectoryToPutSnapshotsIn()
        {
            
        }
    }
}