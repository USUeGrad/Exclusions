/*
 * opens dialogue for csv file to be selected
 */

namespace SnapShotApp
{
    class FileOpen
    {
        private string _fileName;

        public string SelectedFile()
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.ShowDialog();
            _fileName = dialog.FileName;
            return _fileName;
        }
    }
}
