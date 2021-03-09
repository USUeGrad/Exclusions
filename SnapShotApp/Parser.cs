using System;
using System.Collections.Generic;
using System.IO;

/**
*   This class pulls names from a CSV file
*   located in the directory of this program.
*   From there the names can be searched on https://exclusions.oig.hhs.gov.
**/

namespace SnapShotApp
{
    class Parser
    {
        private StreamReader _streamReader;
        public List<Person> PeopleList = new List<Person>();
        private string _line;
        public int counter;
        private Person _person;

        public void Parse(string fileLocation)
        {
            //Console.WriteLine(fileLocation);
            _streamReader = new StreamReader(fileLocation);
            ParseName();
        }

        private void ParseName()
        {
            while ((_line = _streamReader.ReadLine()) != null)
            {
                if (counter > 0) //skip header names
                {
                    //removes the " from the string
                    _line = _line.Replace("\"", "");
                    AssignNameToPerson();
                    PeopleList.Add(_person);
                }
                counter++;
            }
        }


        private void AssignNameToPerson()
        {
            var nameString = _line.Split(',');
            _person.LastName = nameString[0];
            _person.FirstName = nameString[1];
            //not currently used
            _person.MiddleName = nameString[2];
            _person.DateOfBirth = AssignDateOfBirthToPerson(nameString);
        }

        private static string AssignDateOfBirthToPerson(string[] nameString)
        {
            //conform the birthdate to be the same as the OIG LEIE udpated database format
            if (nameString[4] != "" && nameString[4] != null)
            {
                var monthIndex = nameString[4].IndexOf("/", StringComparison.Ordinal);
                var month = nameString[4].Substring(0, monthIndex);
                var remDayYear = nameString[4].Substring(monthIndex + 1, nameString[4].Length - monthIndex - 1);
                var dayIndex = remDayYear.IndexOf("/", StringComparison.Ordinal);
                var day = remDayYear.Substring(0, dayIndex);
                var year = remDayYear.Substring(dayIndex + 1, remDayYear.Length - dayIndex - 1);

                //make numbers to be double digits
                if (day.Length == 1)
                {
                    day = "0" + day;
                }
                if (month.Length == 1)
                {
                    month = "0" + month;
                }
                return  month + "/" + day + "/" + year;
            }
            //return null;
            return null;
        }
    }
}