﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Text;
using System.Windows.Forms;
// Copyright 2015 MS Almquist. 
// MIT Licence
namespace TimeTrack
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private readonly string _fileName = "TasksLog.csv";
        private readonly string _projectListFile = "projectlist.txt";
        private readonly string _lockFileName = "TasksLog.locked";

        private string GetTrackFilePath()
        {
            return Properties.Settings.Default.FilePath + Path.DirectorySeparatorChar + _fileName;
        } // end ()

        private string GetProjectListFilePath()
        {
            return Properties.Settings.Default.FilePath + Path.DirectorySeparatorChar + _projectListFile;
        }

        private string GetLockFilePath()
        {
            return Properties.Settings.Default.FilePath + Path.DirectorySeparatorChar + _lockFileName;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string path = GetTrackFilePath();
            bool gotlock = GetLock();
            if (gotlock)
            {
                bool fileExists = System.IO.File.Exists(path);
                //DateTime dt = this.dateTimePicker1.Value;
                //string dateStr = dt.Month.ToString() + "/" + dt.Day.ToString() + "/" + dt.Year.ToString();
                TaskRecord tr = new TaskRecord();
                tr.Date = TaskDate;
                tr.Hours = textBoxHours.Text;
                tr.TaskSummary = textBoxTaskName.Text;
                tr.Description = textBoxDescription.Text;
                tr.ForWho = listBox1.SelectedItem.ToString();
                WriteCvs wcvs = new WriteCvs();
                wcvs.Write(tr, path);

                this.Close();

                ReleaseLock();
            }
            else
            {
                MessageBox.Show("The task records are currently locked.", "Lock Detected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string _dateString = "";

        private void Form1_Load(object sender, EventArgs e)
        {
            var fver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            this.labelVersion.Text = string.Format("Version: {0}.{1}", fver.FileMajorPart, fver.FileMinorPart);

            string hostname = System.Windows.Forms.SystemInformation.ComputerName;

            string path = Properties.Settings.Default.FilePath;
            if (String.IsNullOrEmpty(Properties.Settings.Default.FilePath))
            {
                Properties.Settings.Default.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Properties.Settings.Default.Save();
            }

            LoadProjectList();
            listBox1.SelectedIndex = 0;
            GetDateStringFromPicker();
        }

        private void GetTaskFilePath()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult dr = fbd.ShowDialog();
            if (dr != DialogResult.Cancel)
            {
                Properties.Settings.Default.FilePath = fbd.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void DisplayTaskFile()
        {
            Process notepad = new Process();
            notepad.StartInfo.FileName   = "notepad.exe";
            notepad.StartInfo.Arguments = GetTrackFilePath();
            notepad.Start();
            notepad.WaitForExit();
        }

        private void LoadProjectList()
        {
            var projFile = GetProjectListFilePath();
            if(System.IO.File.Exists(projFile))
            {
                listBox1.Items.Clear();
                StreamReader sr = new StreamReader(projFile, Encoding.ASCII);
                string item = "";
                while(item != null)
                {
                    item = sr.ReadLine();
                    if(!string.IsNullOrEmpty(item)) listBox1.Items.Add(item);
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        } // end ()

        List<string> _dates = new List<string>();
        List<Single> _totaledHoursForDay = new List<float>();

        private void BuildHourTotals()
        {
            string path = GetTrackFilePath();
            bool fileExists = System.IO.File.Exists(path);

            StreamReader sr = new StreamReader(path, Encoding.ASCII);
            
            // write a header if the file has just been created
            if(!fileExists)
            {
                string record = sr.ReadLine();
                if(!String.IsNullOrEmpty(record) && record.IndexOf("who") > 0)
                {
                    string[] toks = null;
                    do
                    {
                        record = sr.ReadLine();
                        toks = record.Split('\t');
                        int i = 0;
                        foreach(var s in _dates)
                        {
                            if(s.CompareTo(_dateString) == 0)
                            {


                            }
                            i++;
                        }
                    } 
                    while (record != null);

                }

            }
            sr.Close();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            GetDateStringFromPicker();
        } // end ()

        private DateTime TaskDate = DateTime.Now;

        private void GetDateStringFromPicker()
        {
            TaskDate = this.dateTimePicker1.Value;
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DlgGenericTextEntry dgte = new DlgGenericTextEntry();
            dgte.Title = "Add Project Category";
            DialogResult dr = dgte.ShowDialog();
            if(dr != DialogResult.Cancel)
            {
                var projFile = GetProjectListFilePath();
                StreamWriter sw = new StreamWriter(projFile, true, Encoding.ASCII);
                sw.WriteLine(dgte.TextEntry);
                sw.Close();
                listBox1.Items.Add(dgte.TextEntry);
                listBox1.SelectedIndex = listBox1.FindString(dgte.TextEntry);
            }
        }// end ()

        private bool GetLock()
        {
            string lockfile = GetLockFilePath();
            bool gotLock = !System.IO.File.Exists(lockfile);
            if (gotLock)
            {
                StreamWriter sw = new StreamWriter(lockfile);
                sw.Close();
            }
            return gotLock;
        }

        private bool ReleaseLock()
        {
            string lockfile = GetLockFilePath();
            bool locked = System.IO.File.Exists(lockfile);
            if (locked)  { System.IO.File.Delete(lockfile);  }
            return locked;
        }

        private void setTaskFilePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetTaskFilePath();
        }

        private void generateReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime timestamp = DateTime.Now;
            string datesuffix = "_" + timestamp.Day.ToString() + "_" + timestamp.ToString("MMM") + "_" + timestamp.Year.ToString();

            ImportTaskList tlist = new ImportTaskList();
            string taskfile = GetTrackFilePath();

            string selectedForWho = (string)listBox1.SelectedItem;
            string reportfile = Path.GetDirectoryName(taskfile) + Path.DirectorySeparatorChar + "Reports" + Path.DirectorySeparatorChar + "TaskReport_" + selectedForWho + datesuffix + ".txt";

            if (!Directory.Exists(Path.GetDirectoryName(reportfile))) Directory.CreateDirectory(Path.GetDirectoryName(reportfile));

            var importedTasks = tlist.ImportTaskFile(taskfile, selectedForWho);
            List<string> monthsList = tlist.GetMonthsRepresented();

            if (monthsList.Count > 1)
            {
                ListBoxChooserDlg lbcd = new ListBoxChooserDlg();
                lbcd.Text = "Choose Month For Report";
                lbcd.InstructionText = "(Choose 'All' for all months.)";
                lbcd.LoadListboxItems(monthsList);
                if (DialogResult.Cancel == lbcd.ShowDialog()) return;

                if(!lbcd.SelectedItem.ToLower().Contains("all"))
                {
                    DateTime dt = DateTime.Now;
                    if(DateTime.TryParse(lbcd.SelectedItem, out dt))
                    {
                        importedTasks = tlist.FilterByTimePeriod(dt.Month, dt.Year);
                    }   
                }
            }

            decimal totalHours = tlist.GetTotalHours(importedTasks);

            MonthlyReport.Write(importedTasks, reportfile, totalHours);

            
        }

        private void showTaskFileContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayTaskFile();
        }
    }
}