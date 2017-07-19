using System;
using System.Collections.Generic;

namespace TimeTrack
{
    public class TaskRecord
    {
        public DateTime Date = DateTime.Now;
        public string Hours = "";
        public string TaskSummary = "";
        public string Description = "";
        public string ForWho = "";

        public string GetDateString()
        {
            return Date.Month.ToString() + "/" + Date.Day.ToString() + "/" + Date.Year.ToString();
        }

        public string ToCvsString()
        {
            string dateString = GetDateString();
            string record = string.Format("{0}\t{1}\t{2}\t{3}\t{4}",
                            dateString,
                            ForWho,
                            Hours,
                            TaskSummary,
                            Description);
            return record;
        }

        // forWho arg can be a string from the forwho column, or '*' -- if
        // the for, returns only the record latter, any record is returned.
        public bool FillRecordFromCvs(string cvsLine, string forWho)
        {
            bool retVal = false;

            if (string.IsNullOrEmpty(cvsLine)) throw new ArgumentException();

            char[] delim = new char[] { '\t' };
            string[] toks = cvsLine.Split(delim);
            string forWhoIn = toks[1].ToLower();
            if (forWho.ToLower() == forWhoIn || forWho == null)
            {

                ForWho = toks[1];
                Hours = toks[2];
                TaskSummary = toks[3];
                Description = toks[4];
                retVal = DateTime.TryParse(toks[0], out Date);
            }

            return retVal;
        }
    }
}
