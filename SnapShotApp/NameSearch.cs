using System.Windows.Forms;

namespace SnapShotApp
{
	class NameSearch
	{
		private Parser _parse;
		private ScreenShot _screenShot;
		private VerifyList _verifyList;

		public NameSearch(string file, string folderDestination)
		{
			_parse = new Parser();
			_parse.Parse(file);
			_screenShot = new ScreenShot(folderDestination);
			_verifyList = new VerifyList();
		}

		public void CountNames()
		{
			_verifyList.AddToVerifyList("Total number of names checked: " + (_parse.counter - 1));
		}

		public void SearchOig()
		{
			new Oig(_parse, _screenShot, _verifyList);
		}

		public void SearchSam()
		{
			new Sam(_parse, _screenShot, _verifyList);
		}

		public void WriteSummary(string folderLocation)
		{
			using (var file = new System.IO.StreamWriter(folderLocation + "!!SUMMARY!!.txt"))
			{
				foreach (var line in _verifyList.ReturnVerifyList())
				{
					file.WriteLine(line);
				}
			}
			//MessageBox.Show("Screenshots Complete!", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}
}
