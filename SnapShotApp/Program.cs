/**   
*   Utah State University
*   The College of Education
*   and Human Resources
*   Authors: Joe Bainbridge  (joe.bainbridge@usu.edu)
*            Eric Rowles     (eric.rowles@usu.edu)
**/

using System;
using System.Windows.Forms;

namespace SnapShotApp
{
    internal class Program
    {
        [STAThread]
        private static void Main()
        {
            //parse the updated exclusions list
            ParseUpdatedList parseUpdatedList = new ParseUpdatedList();
            parseUpdatedList.ParseExcludedPeople();

            //open csv file
            SelectFile selectFile = new SelectFile();
            //select directory to put snapshots in
            SelectFolder selectFolder = new SelectFolder();

            //create a folder with today's date inside the folder selected
            CreateFolder folderToPutSnapShotsIn = new CreateFolder(selectFolder.SelectedFolder);
            //initialize verify list
            VerifyList verifyList = new VerifyList();

            //count number of names in the list
            int numNames = -1;  //header is not a name
            System.IO.StreamReader streamCounter = new System.IO.StreamReader(selectFile.SelectedFile);
            while (streamCounter.ReadLine() != null) { numNames++; }
            verifyList.AddToVerifyList("Total number of names checked: " + numNames);

            //run snapshots
            Oig oig = new Oig(selectFile.SelectedFile, folderToPutSnapShotsIn.ReturnCreatedFolderPath(), verifyList, parseUpdatedList.ExcludedPeople);
            Sam sam = new Sam(selectFile.SelectedFile, folderToPutSnapShotsIn.ReturnCreatedFolderPath(), verifyList, parseUpdatedList.ExcludedPeople);

            //output verify list file
            FileOutput verifyFileOutput = new FileOutput(folderToPutSnapShotsIn.ReturnCreatedFolderPath(),
                verifyList);
            verifyFileOutput.OutputPeopleToVerify(verifyList);
                
            //snapshot process is complete
            //Pops up a message box comfirming that process is complete.
            verifyFileOutput.Completed();
            Console.WriteLine("Process complete!");
        }
    }
}