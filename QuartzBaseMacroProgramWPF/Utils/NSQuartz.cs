using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using WindowsInput.Native;
using QuartzBaseMacroProgramWPF.Model;

namespace QuartzBasedMacroProgram.Utils
{
    public class NSQuartz : IDisposable
    {
        public readonly string jobGroup = "NSJobGroup";
        private ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;

        private static NSQuartz _NSQuartzScheduler;

        public static NSQuartz Instance
        {
            get
            {
                if (_NSQuartzScheduler == null)
                {
                    _NSQuartzScheduler = new NSQuartz();
                }

                return _NSQuartzScheduler;
            }
        }

        public bool IsShutDown
        {
            get
            {
                return _scheduler.IsShutdown;
            }
        }

        public bool IsStarted
        {
            get
            {
                return _scheduler.IsStarted;
            }
        }

        public bool InStandbyMode
        {
            get
            {
                return _scheduler.InStandbyMode;
            }
        }

        private NSQuartz()
        {
            _schedulerFactory = new StdSchedulerFactory();
            _scheduler = _schedulerFactory.GetScheduler().Result;
        }

        ~NSQuartz()
        {
            if (!_scheduler.IsShutdown)
            {
                _scheduler.PauseAll();
                _scheduler.Clear();
                _scheduler.Shutdown();
            }
        }

        public void Dispose()
        {
            if (_scheduler != null && _scheduler.IsStarted)
            {
                //_schedulerFactory.AllSchedulers.Clear();
                //Clear();
                Shutdown();
            }
        }

        public void Start()
        {
            if (!_scheduler.IsStarted)
            {
                _scheduler.ResumeAll();

                _scheduler.Start();
            }
        }

        public void Shutdown()
        {
            if (!_scheduler.IsShutdown)
            {
                _scheduler.Shutdown();
            }
        }

        public void PauseandResume()
        {
            if (!_scheduler.InStandbyMode)
            {
                _scheduler.Standby();
            }
            else
            {
                _scheduler.ResumeAll();

                _scheduler.Start();
            }
        }

        public void Clear()
        {
            if (_scheduler.IsStarted)
            {
                _scheduler.Standby();
                //_scheduler.PauseAll();
                _scheduler.Clear();
                //_scheduler.Shutdown();
                _scheduler.Start();
            }
        }

        public void TestRun()
        {
            Console.WriteLine("-Scheduling Jobs-");
            //DateTimeOffset startTime = DateBuilder.NextGivenSecondDate(null, 15);
            IJobDetail jobDetail = JobBuilder.Create<SimpleJob>().WithIdentity("job1", jobGroup).Build();
            ICronTrigger cronTrigger = (ICronTrigger)TriggerBuilder.Create().WithIdentity("trigger1", jobGroup).WithCronSchedule("0/2 * * * * ?").StartNow().Build();
            DateTimeOffset? ft = _scheduler.ScheduleJob(jobDetail, cronTrigger).Result;
            Console.WriteLine($"{jobDetail.Key} has been scheduled to run at {ft} and repeat based on expression: {cronTrigger.CronExpressionString}");
            this.Start();
        }

        public void AddJobRow(string cronExpression, IJob job, object param)
        {
            StringBuilder jobName = new StringBuilder();
            jobName.AppendFormat("job@{0}", Guid.NewGuid().ToString());
            string jobGroup = NSQuartz.Instance.jobGroup;
            JobDetailImpl jobDetail = new JobDetailImpl(jobName.ToString(), jobGroup, job.GetType());
            jobDetail.JobDataMap["param"] = param;
            CronTriggerImpl cronTrigger = new CronTriggerImpl
            (
                "Trigger" + jobName,
                jobGroup,
                cronExpression
            );
            if(job is PressKey)
            {
                cronTrigger.Description = $"작업 분류: 키보드 입력, 패러미터: {(VirtualKeyCode)int.Parse(param.ToString())}";
            }
            else if(job is PressKeyMulti)
            {
                cronTrigger.Description = $"작업 분류: 키보드 다중 입력, 패러미터: {string.Join(", ", ((List<int>)param).Cast<VirtualKeyCode>().ToArray())}";
            }
            else if(job is SequenceKey)
            {
                cronTrigger.Description = $"작업 분류: 키보드 연속 입력, 패러미터: {(VirtualKeyCode)((SequenceKeyData)param).holdkey} and {string.Join(", ", ((SequenceKeyData)param).sendkeys.Cast<VirtualKeyCode>().ToArray())}";
            }
            else
            {
                cronTrigger.Description = $"작업 분류: 미확인 작업, 패러미터: {param.ToString()}";
            }
            _scheduler.ScheduleJob(jobDetail, cronTrigger);
        }

        public void AddJob(string jobName, string jobGroup, string cronExpression, IJob job)
        {
            JobDetailImpl jobDetail = new JobDetailImpl(jobName, jobGroup, job.GetType());

            CronTriggerImpl cronTrigger = new CronTriggerImpl
            (
                "Trigger" + jobName,
                jobGroup,
                cronExpression
            );

            _scheduler.ScheduleJob(jobDetail, cronTrigger);
        }

        /// <summary>
        /// 일반적으로 AddJob(string, TimeJobModel)에서 호출하는 직접적 작업 추가 메서드.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroup"></param>
        /// <param name="jobTime"></param>
        /// <param name="job"></param>
        public void AddJob(string jobName, string jobGroup, DateTime jobTime, IJob job)
        {
            //DateTime targetTime = DateTime.Now;
            int jobTimeCompare = DateTime.Compare(jobTime, DateTime.Now);

            if (job != null)
            {
                JobDetailImpl jobDetail = new JobDetailImpl(jobName, jobGroup, job.GetType());

                CronTriggerImpl cronTrigger = new CronTriggerImpl
                (
                    "Trigger" + jobName,
                    jobGroup,
                    string.Format
                    (
                        "{0} {1} {2} {3} {4} ? {5}",
                        jobTime.Second,
                        jobTime.Minute,
                        jobTime.Hour,
                        jobTime.Day,
                        jobTime.Month,
                        jobTime.Year
                    )
                );
                _scheduler.ScheduleJob(jobDetail, cronTrigger);
            }
        }

        /// <summary>
        /// <para>
        /// 외부에서 대부분의 경우 호출하도록 되어있는 간접적 작업 추가 메서드. 
        /// </para>
        /// timeJob에서의 시작시간에 시각 작업을 하는 것과 끝 시간에 끝 작업을 하는 두가지 작업을 동시에 추가한다.
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="timeJob"></param>
        public void AddJob(string jobGroup, TimeJobModel timeJob)
        {
            DateTime nowTime = DateTime.Now;
            int startTimeCompare = DateTime.Compare(timeJob.StartTime, nowTime);
            int endTimeCompare = DateTime.Compare(timeJob.StartTime, nowTime);
            if (startTimeCompare > 0 && endTimeCompare > 0)
            {
                StringBuilder startJobName = new StringBuilder();
                StringBuilder endJobName = new StringBuilder();

                startJobName.AppendFormat("J_S@{0}_{1}_{2}", timeJob.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.Ticks, Guid.NewGuid().ToString());
                endJobName.AppendFormat("J_E@{0}_{1}_{2}", timeJob.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.Ticks, Guid.NewGuid().ToString());

                AddJob(startJobName.ToString(), jobGroup, timeJob.StartTime, timeJob.StartJob);
                AddJob(endJobName.ToString(), jobGroup, timeJob.EndTime, timeJob.EndJob);
            }
        }

        public List<ITrigger> GetTriggers()
        {
            List<ITrigger> result = new List<ITrigger>();
            IList<string> jobGroupList = _scheduler.GetJobGroupNames().Result.ToList();

            foreach (string group in jobGroupList)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var jobKeys = _scheduler.GetJobKeys(groupMatcher).Result;
                foreach (var jobKey in jobKeys)
                {
                    var detail = _scheduler.GetJobDetail(jobKey).Result;
                    var triggers = _scheduler.GetTriggersOfJob(jobKey).Result;
                    foreach (ITrigger trigger in triggers)
                    {
                        result.Add(trigger);
                    }
                }
            }
            return result;
        }

        public string GetAllGroupList()
        {
            string result = "==== Schedule List ====\r\n";
            IList<string> jobGroupList = _scheduler.GetJobGroupNames().Result.ToList();

            foreach (string group in jobGroupList)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var jobKeys = _scheduler.GetJobKeys(groupMatcher).Result;
                foreach (var jobKey in jobKeys)
                {
                    var detail = _scheduler.GetJobDetail(jobKey).Result;
                    var triggers = _scheduler.GetTriggersOfJob(jobKey).Result;
                    foreach (ITrigger trigger in triggers)
                    {
                        result += "Group: " + group;
                        result += ", JobKey: " + jobKey.Name;
                        result += ", Description: " + detail.Description;
                        result += ", Trigger Name: " + trigger.Key.Name;
                        result += ", Trigger Group: " + trigger.Key.Group;
                        result += ", Trigger Type: " + trigger.GetType().Name;
                        result += ", Trigger State: " + _scheduler.GetTriggerState(trigger.Key);

                        Debug.Write("Group: " + group);
                        Debug.Write(", JobKey: " + jobKey.Name);
                        Debug.Write(", Description: " + detail.Description);
                        Debug.Write(", Trigger Name: " + trigger.Key.Name);
                        Debug.Write(", Trigger Group: " + trigger.Key.Group);
                        Debug.Write(", Trigger Type: " + trigger.GetType().Name);

                        DateTimeOffset? previousFireTime = trigger.GetPreviousFireTimeUtc();
                        if (previousFireTime.HasValue)
                        {
                            Debug.Write(", PreviousFireTime: " + previousFireTime.Value.LocalDateTime.ToString());
                            result += " " + previousFireTime.Value.LocalDateTime.ToString();
                        }

                        DateTimeOffset? nextFireTime = trigger.GetNextFireTimeUtc();
                        if (nextFireTime.HasValue)
                        {
                            Debug.Write(", NextFireTime: " + nextFireTime.Value.LocalDateTime.ToString());
                            result += " " + nextFireTime.Value.LocalDateTime.ToString();
                        }

                        Debug.WriteLine(", Trigger State: " + _scheduler.GetTriggerState(trigger.Key));
                        result += ", Trigger State: " + _scheduler.GetTriggerState(trigger.Key);
                        result += "\r\n";
                    }
                }
            }
            return result;
        }

        public void GetState()
        {
            Debug.WriteLine($"in standbymode? : {_scheduler.InStandbyMode}");
            Debug.WriteLine($"is started? : {_scheduler.IsStarted}");
        }

        public List<TimeJobModel> GetReverseTimeList(DateTime startTime, DateTime endTime)
        {
            List<TimeJobModel> result = new List<TimeJobModel>();

            DateTime targetTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
            int startTimeCompare = DateTime.Compare(targetTime, startTime);

            targetTime = new DateTime(endTime.Year, endTime.Month, endTime.Day, 23, 59, 0);
            int endTimeCompare = DateTime.Compare(targetTime, endTime);

            if (startTimeCompare == 0 && endTimeCompare == 1) //시작시간 00:00:00, 종료시간 23:59:00 이전
            {
                TimeJobModel TimeJobModel = new TimeJobModel();
                TimeJobModel.StartTime = endTime;
                TimeJobModel.EndTime = new DateTime(endTime.Year, endTime.Month, endTime.Day, 23, 59, 59);
                result.Add(TimeJobModel);
            }
            else if (startTimeCompare == 0 && endTimeCompare <= 0) //시작시간 00:00:00, 종료시간 23:59:00 이후
            {
                TimeJobModel TimeJobModel = new TimeJobModel();
                TimeJobModel.StartTime = startTime;
                TimeJobModel.EndTime = endTime;
                result.Add(TimeJobModel);
            }
            else if (startTimeCompare == -1 && endTimeCompare <= 0) //시작시간 00:00:00 이후, 종료시간 23:59:00 이후
            {
                TimeJobModel TimeJobModel = new TimeJobModel();
                TimeJobModel.StartTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
                TimeJobModel.EndTime = startTime;
                result.Add(TimeJobModel);
            }
            else if (startTimeCompare == -1 && endTimeCompare == 1) //시작시간 00:00:00 이후, 종료시간 23:59:00 이전
            {
                TimeJobModel TimeJobModel1 = new TimeJobModel();
                TimeJobModel TimeJobModel2 = new TimeJobModel();

                TimeJobModel1.StartTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
                TimeJobModel1.EndTime = startTime;

                TimeJobModel2.StartTime = endTime;
                TimeJobModel2.EndTime = new DateTime(endTime.Year, endTime.Month, endTime.Day, 23, 59, 59);

                result.Add(TimeJobModel1);
                result.Add(TimeJobModel2);
            }

            return result;
        }
    }

    /// <summary>
    /// 어떠한 작업의 타입, 시작 시간, 끝 시간, 시작시 작업, 끝시 작업을 명시한 클래스.
    /// </summary>
    public class TimeJobModel
    {
        public string type { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public IJob StartJob { get; set; }
        public IJob EndJob { get; set; }
    }
}
