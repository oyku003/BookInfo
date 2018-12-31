using BookInfo.ML;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace BookInfo.Helper
{
    public class Jobclass : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            new AprioriProcess().CreateAprioriRules();
        }
    }
}