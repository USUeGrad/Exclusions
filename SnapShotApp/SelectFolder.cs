using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Forms;
using System;
using System.IO;


/*
 * opens dialogue for where to save screenshots
 */

namespace SnapShotApp
{
    class SelectFolder
    {
        public string SelectedFolder;
        private static string currentDate;

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
            CreateFolderWithTodaysDate();
        }

        public void CreateFolderWithTodaysDate()
        {
            DateTime today = DateTime.Today; // As DateTime
            currentDate = today.ToString("MM-dd-yyyy");

            Directory.CreateDirectory(SelectedFolder + "\\" + currentDate);
        }

        public string FolderPath()
        {
            return SelectedFolder + "\\" + currentDate + "\\";
        }
    }
}