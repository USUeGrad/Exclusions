/**   
*   Utah State University
*   The College of Education
*   and Human Resources
*   Authors: Joe Bainbridge  (joe.bainbridge@usu.edu)
*            Eric Rowles     (eric.rowles@usu.edu)
**/

using System;

namespace SnapShotApp
{
    internal class Program
    {
        [STAThread]
        private static void Main()
        {

            //open csv file
            SelectFile selectFile = new SelectFile();
            //select directory to put snapshots in
            SelectFolder selectFolder = new SelectFolder();

            NameSearch nameSearch = new NameSearch(selectFile.SelectedFile, selectFolder.FolderPath());
            //count number of names in the list
            nameSearch.CountNames(selectFile.SelectedFile);
            //check OIG exclusions
            nameSearch.SearchOig();
            //check SAM exclusions
            nameSearch.SearchSam();
            //output summary file and notify user process is complete
            nameSearch.WriteSummary(selectFolder.FolderPath());
            
        }
    }
}