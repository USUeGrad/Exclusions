using System.Windows.Forms;

/*
 * Once all information has been gathered, sends contents of VerifyList to txt file. 
 */

namespace SnapShotApp
{
    class FileOutput
    {
        private readonly string _folderLocation;

        public FileOutput(string folderLocation, VerifyList verifyList)
        {
            _folderLocation = folderLocation;
            OutputPeopleToVerify(verifyList);
        }

        public void OutputPeopleToVerify(VerifyList verifyList)
        {
            using (var file = new System.IO.StreamWriter(_folderLocation + "@-SUMMARY-@.txt"))
            {
                foreach (var line in verifyList.ReturnVerifyList())
                {
                    file.WriteLine(line);
                }
            }
        }
        
        public void Completed() {
            MessageBox.Show("Screenshots Complete!", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}