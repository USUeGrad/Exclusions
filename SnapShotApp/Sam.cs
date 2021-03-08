
using System.Collections.Generic;

namespace SnapShotApp
{
    class Sam
    {
        public Sam(string file, string folderDestination, VerifyList verifyList, List<ExcludedPerson> excludedPersons)
        {
            var parse = new Parser();
            parse.Parse(file);
            var screenShot = new ScreenShot(folderDestination);
            var nameSearcher = new NameSearcherSam();
            nameSearcher.SearchName(screenShot, parse, "https://www.sam.gov", verifyList, excludedPersons);
        }
    }
}