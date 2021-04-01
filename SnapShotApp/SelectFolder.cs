using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows.Forms;


/*
 * opens dialogue for where to save screenshots
 */

namespace SnapShotApp
{
	class SelectFolder
	{
		public string SelectedFolder;
		private static string currentDate;


		public SelectFolder(string fileName)
		{
			DateTime today = DateTime.Today; // As DateTime
			currentDate = today.ToString("MM-dd-yyyy");
			if (fileName.EndsWith("BiomedicalClinic.csv")) {
				SelectedFolder = (Directory.GetCurrentDirectory() + "\\" + "Biomedical Clinic");

			}
			else if (fileName.EndsWith("Clinical.csv")) {
				SelectedFolder = (Directory.GetCurrentDirectory() + "\\" + "Clinical");

			}
			else if (fileName.EndsWith("OtherUsers.csv")) {
				SelectedFolder = (Directory.GetCurrentDirectory() + "\\" + "OtherUsers");

			}
			else if (fileName.EndsWith("ProviderExclusionReport.csv")) {
				SelectedFolder = (Directory.GetCurrentDirectory() + "\\" + "Provider");

			}
			else if (fileName.EndsWith("UserExclusionReport.csv")) {
				SelectedFolder = (Directory.GetCurrentDirectory() + "\\" + "User");

			}
			else if (fileName.EndsWith("Sample.csv"))
			{
				SelectedFolder = (Directory.GetCurrentDirectory() + "\\" + "Sample");

			}
			else
			{
				var chooseFolder = new CommonOpenFileDialog();
				chooseFolder.IsFolderPicker = true;
				chooseFolder.Title = "Choose output folder for " + fileName;
				if (chooseFolder.ShowDialog() == CommonFileDialogResult.Ok)
				{
					SelectedFolder = chooseFolder.FileName;
				}
				if (SelectedFolder == null) //invalid folder
				{
					MessageBox.Show("Please Select a Valid Folder");
					System.Environment.Exit(0);
				}
			}
			Directory.CreateDirectory(SelectedFolder + "\\" + currentDate);
		}

		public string FolderPath()
		{
			return SelectedFolder + "\\" + currentDate + "\\";
		}
	}
}