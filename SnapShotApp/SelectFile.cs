using System.Windows.Forms;

/*
 * opens file dialogue for where to save screenshots
 */

namespace SnapShotApp
{
	class SelectFile
	{
		public string SelectedFile;

		public SelectFile()
		{
			OpenFileDialog chooseFile = new OpenFileDialog();
			chooseFile.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
			chooseFile.Title = "Choose input file";
			if (chooseFile.ShowDialog() == DialogResult.OK)
			{
				SelectedFile = chooseFile.FileName;
			}
			if (SelectedFile == null) //invalid CSV
			{
				MessageBox.Show("Please Select a Valid CSV File.");
				System.Environment.Exit(0);
			}
		}

	}
}