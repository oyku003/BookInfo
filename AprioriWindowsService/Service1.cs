using BookInfo.ML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AprioriWindowsService
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer;

        public Service1()
        {
            InitializeComponent();

            timer = new Timer();
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(timer_Tick);

            timer.Interval = 60000;
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
        }

        public void timer_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                timer.Stop();

                CreateAprioriRules();
            }
            catch (Exception ex)
            {
                log($"{ex.StackTrace} \n {ex.Message} \n {ex.InnerException}");
            }
            finally
            {
                timer.Start();
            }
        }

        private void CreateAprioriRules()
        {
            new AprioriProcess().CreateAprioriRules(); 
        }

        private void log(string message)
        {
            string filePath = @"C:\Users\YAĞIZ\Desktop\metinbelgesi.txt";

            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);

            StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine(message);
            sw.WriteLine(DateTime.Now.ToString());

            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
}
