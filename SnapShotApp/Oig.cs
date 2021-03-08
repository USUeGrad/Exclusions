
using System.Collections.Generic;

namespace SnapShotApp
{
    class Oig
    {
        public Oig(string file, string folderDestination, VerifyList verifyList, List<ExcludedPerson> excludedPersons)
        {
            var parse = new Parser();
            parse.Parse(file);
            var screenShot = new ScreenShot(folderDestination);
            var nameSearcher = new NameSearchOig();
            nameSearcher.SearchName(screenShot, parse, "https://exclusions.oig.hhs.gov", verifyList, excludedPersons);
        }
    }
}