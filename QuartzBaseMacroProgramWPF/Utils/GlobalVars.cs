using Newtonsoft.Json;
using QuartzBaseMacroProgramWPF.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace QuartzBaseMacroProgramWPF.Utils
{
    public static class GlobalVars
    {
        public static NSQuartz scheduler = NSQuartz.Instance;
        public static ScheduleModel mainSchedule;
        public static SettingModel currentSetting;
        public static List<string> nextApps = new List<string>();
        public static int curIndex;
        public static bool isworking;
        public static bool istimerticking;

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

        public static void SetSetting()
        {
            using (StreamReader sr = new StreamReader("settings/setting.json"))
            {
                string json = sr.ReadToEnd();
                GlobalVars.currentSetting = JsonConvert.DeserializeObject<SettingModel>(json, settings);
            }
        }

        public static void SetQuartzJobs()
        {
            GlobalVars.scheduler.Clear();
            using (StreamReader sr = new StreamReader("settings/data.json"))
            {
                string json = sr.ReadToEnd();
                GlobalVars.mainSchedule = JsonConvert.DeserializeObject<ScheduleModel>(json, settings);
            }

            if (currentSetting.parsemode == "string")
            {
                foreach (var x in GlobalVars.mainSchedule.presskey)
                {
                    try
                    {
                        PressKey newjob = new PressKey();
                        int param = KeyboardInputs.VKStringtoInt(x.sendkey);
                        GlobalVars.scheduler.AddJobRow(x.cronexpression, newjob, param);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            else
            {
                foreach (var x in GlobalVars.mainSchedule.presskey)
                {
                    try
                    {
                        PressKey newjob = new PressKey();
                        GlobalVars.scheduler.AddJobRow(x.cronexpression, newjob, int.Parse(x.sendkey));
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            if (currentSetting.parsemode == "string")
            {
                foreach (var x in mainSchedule.presskeymulti)
                {
                    try
                    {
                        PressKeyMulti newjob = new PressKeyMulti();
                        List<int> param = x.sendkeys.ConvertAll(KeyboardInputs.VKStringtoInt);
                        GlobalVars.scheduler.AddJobRow(x.cronexpression, newjob, param);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            else
            {
                foreach (var x in mainSchedule.presskeymulti)
                {
                    try
                    {
                        PressKeyMulti newjob = new PressKeyMulti();
                        GlobalVars.scheduler.AddJobRow(x.cronexpression, newjob, x.sendkeys.ConvertAll(int.Parse));
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            if (currentSetting.parsemode == "string")
            {
                foreach (var x in mainSchedule.sequencekey)
                {
                    SequenceKeyData param = new SequenceKeyData();
                    if (x.holdkey != string.Empty)
                    {
                        param.holdkey = KeyboardInputs.VKStringtoInt(x.holdkey);
                        param.iscombomode = true;
                    }
                    else
                    {
                        param.iscombomode = false;
                        param.holdkey = 0;
                    }
                    param.interval = TimeSpan.FromSeconds(x.interval);
                    param.unholdafter = x.unholdafter;
                    param.reholdafter = x.reholdafter;
                    param.sendkeys = x.sendkeys.ConvertAll(KeyboardInputs.VKStringtoInt);
                    param.sendkeys.Insert(0, 0);
                    param.isholding = true;
                    GlobalVars.scheduler.AddJobRow(x.cronexpression, new SequenceKey(), param);
                }
            }
            else
            {
                foreach (var x in mainSchedule.sequencekey)
                {
                    SequenceKeyData param = new SequenceKeyData();
                    if (x.holdkey != string.Empty)
                    {
                        param.holdkey = int.Parse(x.holdkey);
                        param.iscombomode = true;
                    }
                    else
                    {
                        param.iscombomode = false;
                        param.holdkey = 0;
                    }
                    param.interval = TimeSpan.FromSeconds(x.interval);
                    param.unholdafter = x.unholdafter;
                    param.reholdafter = x.reholdafter;
                    param.sendkeys = x.sendkeys.ConvertAll(int.Parse);
                    param.sendkeys.Insert(0, 0);
                    param.isholding = true;
                    GlobalVars.scheduler.AddJobRow(x.cronexpression, new SequenceKey(), param);
                }
            }
            App.Logger.Info(GlobalVars.scheduler.GetAllGroupList());
        }
    }
}