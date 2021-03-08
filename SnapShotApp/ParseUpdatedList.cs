using System;
using System.Collections.Generic;
using System.IO;

/**
 * This class pulls names from an updated CSV file (Updated LEIE Database) from
 * https://oig.hhs.gov/exclusions/exclusions_list.asp
 * This list is updated each month and should be placed in the application folder
 */

namespace SnapShotApp
{
    public class ParseUpdatedList
    {
        private StreamReader _streamReader;
        public List<ExcludedPerson> ExcludedPeople;
        private const string FileLocation = "UPDATED.csv"; //this file should be downloaded from the above link each month
        private string _line;
        private int _counter;

        public void ParseExcludedPeople()
        {
            try
            {
                ExcludedPeople = new List<ExcludedPerson>();
                _streamReader = new StreamReader(FileLocation);

                while ((_line = _streamReader.ReadLine()) != null)
                {
                    if (_counter > 0) //skip header names
                    {
                        CleanString();
                        var excludedPerson = AssignDetailsToExcludedPerson();
                        if (excludedPerson != null)
                        {
                            ExcludedPeople.Add(excludedPerson);
                        }
                    }
                    _counter++;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error Parsing Updated LEIE Database File");
                Environment.Exit(0);
            }
        }

        private void CleanString()
        {
            //removes the " from the string
            _line = _line.Replace("\"", "");
        }

        private ExcludedPerson AssignDetailsToExcludedPerson()
        {
            var nameString = _line.Split(',');
            //companies and industries have null first, last, middle, and DOB field data
            //we're looking for individuals, not companies or industries
            if (nameString[0] != "" &&
                nameString[1] != "" &&
                nameString[2] != "" &&
                nameString[8] != "")
            {
                return new ExcludedPerson
                {
                    LastName = nameString[0],
                    FirstName = nameString[1],
                    MidName = nameString[2],
                    DateOfBirth = nameString[8]
                };
            }
            return null;
        }
    }
}