using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace TimeTrack
{
    class ImportTaskList
    {
        List<TaskRecord> _tasks = null;

        private Dictionary<string, string> _monthsRepresented = new Dictionary<string, string>();
        private List<string> _listOfMonthsRepresented = new List<string>();

        public List<TaskRecord> ImportTaskFile(string fileName, string forwhoFilter)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }
            _tasks = new List<TaskRecord>();
            string monthDictKey = "";
            string monthDictValue = "";

            StreamReader sr = new StreamReader(fileName);
            string line = sr.ReadLine(); // header--toss
            do
            {
                line = sr.ReadLine();
                if(line != null)
                {
                    TaskRecord tr = new TaskRecord();
                    if (tr.FillRecordFromCvs(line, forwhoFilter))
                    {
                        _tasks.Add(tr);
                        monthDictKey = GetKeyString(tr.Date);
                        monthDictValue = GetKeyValue(tr.Date);

                        try { _monthsRepresented.Add(monthDictKey, monthDictValue); }
                        catch (Exception) { ; } // just to catch exception if key added twice
                    }
                    // sort the tasks earliest->latest 
                    _tasks.Sort((ps1, ps2) => DateTime.Compare(ps1.Date, ps2.Date));
                }
            }
            while (line != null);

            foreach (var m in _monthsRepresented)
            {
                _listOfMonthsRepresented.Add(m.Value);
            }

            sr.Close();

            return _tasks;
        }

        public List<string> GetMonthsRepresented()
        {
            return _listOfMonthsRepresented;
        }

        public decimal GetTotalHours (List<TaskRecord> tasks)
        {

            decimal totalHours = 0.0m;
            decimal tempHours = 0.0m;
            // walk through and total hours
            foreach (var t in tasks)
            {
                if (string.IsNullOrEmpty(t.TaskSummary) && string.IsNullOrEmpty(t.Description)) continue; // essentially an empty record

                if (decimal.TryParse(t.Hours, out tempHours))
                {
                    totalHours += tempHours;
                }
                else
                {
                    throw new Exception("empty hours in record--need at least a zero");
                }
            }

            return totalHours;
        }
        private string GetKeyString(DateTime date)
        {
            return date.Month.ToString() + "-" + date.Year.ToString();
        }


        private string GetKeyValue(DateTime date)
        {
            return date.ToString("MMMM") + " " + date.Year.ToString();
        }

        public List<TaskRecord> FilterByTimePeriod(int month, int year)
        {
            if(_tasks == null) return null;
            List<TaskRecord> filtered = new List<TaskRecord>();

            foreach(var t in _tasks)
            {
                if (t.Date.Month == month && t.Date.Year == year)
                {
                    filtered.Add(t);
                }
            }

            return filtered;
        }
    }
}
