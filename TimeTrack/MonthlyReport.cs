using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TimeTrack
{
    public class MonthlyReport
    {
        static public void Write(List<TaskRecord> tasklist, string savePath, decimal totalHours)
        {
            TaskRecord tr = new TaskRecord();

            string lineFormat = "{0}, {1} hours {2}: {3}. {4}";
            StreamWriter sw = new StreamWriter(savePath);
            foreach (var t in tasklist)
            {
                // skip over time records (records containing time remaining information)
                if (t.TaskSummary.ToLower().IndexOf(TaskRecord.TimeRecordText) == 0) continue;

               string datestring = t.GetDateString();
               sw.WriteLine(string.Format(lineFormat,
                            datestring,
                            t.Hours,
                            t.ForWho,
                            t.TaskSummary,
                            t.Description));
                sw.WriteLine("");
            }
            if(totalHours > 0.0m)
            {
                sw.WriteLine("Total hours: " + totalHours.ToString("0.##\n\n"));
            }
            sw.Close();
        }
    }
}
