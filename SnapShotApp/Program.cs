/**   
*   Utah State University
*   The College of Education
*   and Human Resources
*   Authors: Joe Bainbridge  (joe.bainbridge@usu.edu)
*            Eric Rowles     (eric.rowles@usu.edu)
**/

using System;
using System.IO;
using System.Windows.Forms;

namespace SnapShotApp
{
	internal class Program
	{
		[STAThread]
		private static void Main()
		{
			//set working directory
			Directory.SetCurrentDirectory(Directory.GetParent(Directory.GetCurrentDirectory()) + "\\Reports");
			//create array to hold filenames
			string[] filePaths = new string[Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csv", SearchOption.TopDirectoryOnly).Length];

			//ask user to pick one or all files
			DialogResult userChoice;
			userChoice = MessageBox.Show("Run Exclusions on all csv files?", "Exclusions", MessageBoxButtons.YesNo);
			//populate array with files in directory
			if (userChoice == DialogResult.Yes)
			{
				filePaths = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csv", SearchOption.TopDirectoryOnly);
			}
			//Select file in directory
			if (userChoice == DialogResult.No)
			{
				SelectFile selectFile = new SelectFile();
				filePaths[0] = selectFile.SelectedFile;
			}
			foreach (string fileName in filePaths)
			{
				if (fileName != null)
				{
					SelectFolder selectFolder = new SelectFolder(fileName);
					//begin namesearch
					NameSearch nameSearch = new NameSearch(fileName, selectFolder.FolderPath());
					//count names in list
					nameSearch.CountNames();
					//check OIG exclusions
					nameSearch.SearchOig();
					//check SAM exclusions
					nameSearch.SearchSam();
					//output summary file and notify user process is complete
					nameSearch.WriteSummary(selectFolder.FolderPath());
				}
			}
			MessageBox.Show("Screenshots Complete!", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}
}