using System;
using System.IO;

/*
 *  Creates a folder with the current date. Used to store verification and snapshots. 
 */

namespace SnapShotApp
{
    class CreateFolder
    {
        private static string _parentdirectory;
        private static string _todaysDate;

        public CreateFolder(string parentDirectory)
        {
            _parentdirectory = parentDirectory;
            CreateFolderWithTodaysDate();
        }

        public void CreateFolderWithTodaysDate()
        {
            DateTime today = DateTime.Today; // As DateTime
            _todaysDate = today.ToString("MM-dd-yyyy");

            Directory.CreateDirectory(_parentdirectory + "\\" + _todaysDate);
        }

        public string ReturnCreatedFolderPath()
        {
            return _parentdirectory + "\\" + _todaysDate + "\\";
        }

        public string ReturnParentDirectory()
        {
            return _parentdirectory;
        }
    }
}
