using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace TimeTrack
{
    class WriteCvs
    {
        public bool WriteHeader(string taskFilePath)
        {
            bool fileExists = File.Exists(taskFilePath);

            if (!fileExists)
            {
                TaskRecord tr = new TaskRecord();
                StreamWriter sw = new StreamWriter(taskFilePath, false, Encoding.ASCII);

                string header = string.Format("{0}\t{1}\t{2}\t{3}\t{4}",
                                nameof(tr.Date),
                                nameof(tr.ForWho),
                                nameof(tr.Hours),
                                nameof(tr.TaskSummary),
                                nameof(tr.Description));

                sw.WriteLine(header);
                sw.Close();

                fileExists = true;

            }
            return fileExists;
        }

        public bool Write(TaskRecord taskrecord, string recordFilePath)
        {
            bool succeeded = WriteHeader(recordFilePath);

            if(succeeded)
            {
                StreamWriter sw = new StreamWriter(recordFilePath, true, Encoding.ASCII);
                string record = taskrecord.ToCvsString();
                sw.WriteLine(record);
                sw.Close();
            }
            return succeeded;
        }

    }
}
