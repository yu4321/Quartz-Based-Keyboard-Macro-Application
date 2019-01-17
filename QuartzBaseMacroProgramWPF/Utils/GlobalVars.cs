using Newtonsoft.Json;
using QuartzBaseMacroProgramWPF;
using QuartzBaseMacroProgramWPF.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzBasedMacroProgram.Utils
{
    public static class GlobalVars
    {
        public static NSQuartz scheduler = NSQuartz.Instance;
        public static ScheduleModel mainSchedule;
        public static List<string> nextApps = new List<string>();
        public static int curIndex;
        private static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        public static string curAppName
        {
            get
            {
                try
                {
                    return nextApps[curIndex];
                }
                catch
                {
                    return null;
                }
            }
        }
        public static void SetQuartzJobs()
        {
            GlobalVars.scheduler.Clear();
            using (StreamReader sr = new StreamReader("settings/setting.json"))
            {
                string json = sr.ReadToEnd();
                GlobalVars.mainSchedule = JsonConvert.DeserializeObject<ScheduleModel>(json, settings);
            }

            foreach (var x in GlobalVars.mainSchedule.presskey)
            {
                try
                {
                    PressKey newjob = new PressKey();
                    GlobalVars.scheduler.AddJobRow(x.cronexpression, newjob, x.sendkey);
                }
                catch
                {
                    continue;
                }
            }

            //foreach (var x in GlobalVars.mainSchedule.executeprocess)
            //{
            //    try
            //    {
            //        FileInfo info = new FileInfo(x.fullpath);
            //        ExecuteProcess newjob = new ExecuteProcess();
            //        GlobalVars.scheduler.AddJobRow(x.cronexpression, newjob, info);
            //    }
            //    catch
            //    {
            //        continue;
            //    }
            //}

            foreach(var x in mainSchedule.presskeymulti)
            {
                try
                {
                    PressKeyMulti newjob = new PressKeyMulti();
                    GlobalVars.scheduler.AddJobRow(x.cronexpression, newjob, x.sendkeys);
                }
                catch
                {
                    continue;
                }
            }
            App.Logger.Info(GlobalVars.scheduler.GetAllGroupList());
            //GlobalVars.scheduler.GetState();
        }
    }

}
