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
					AssignInfoToPerson();
					PeopleList.Add(_person);
				}
				counter++;
			}
		}


		private void AssignInfoToPerson()
		{
			var nameString = _line.Split(',');
			_person.LastName = nameString[0];
			_person.FirstName = nameString[1];
			//_person.MiddleName = nameString[2]; //not currently used
			_person.DateOfBirth = ConvertDoB(nameString[4]);
		}

		private static string ConvertDoB(string DoB)
		{
			//conform the birthdate to be the same as the OIG LEIE updated database format
			if (DoB != "" && DoB != null)
			{
				DateTime birthDate = Convert.ToDateTime(DoB);
				return birthDate.ToString("MM/dd/yyyy");
			}
			return null;
		}
	}
}