using System.Collections.Generic;

/*
 * Manages verifyList (txt file in folder that shows users to be verified)
 * */

namespace SnapShotApp
{
    class VerifyList
    {
        private readonly List<string> _verifyList = new List<string>();

        public void AddToVerifyList(string personName)
        {
            _verifyList.Add(personName);
        }

        public void RemoveFromVerifyList(string personName)
        {
            _verifyList.Remove(personName);
        }

        public List<string> ReturnVerifyList()
        {
            return _verifyList;
        }
    }
}