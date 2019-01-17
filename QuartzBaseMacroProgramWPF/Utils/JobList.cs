using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Quartz;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using QuartzBaseMacroProgramWPF.Utils;

namespace QuartzBasedMacroProgram.Utils
{
    public class ExecuteProcess : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            FileInfo targetFile = dataMap.Get("param") as FileInfo;
            try
            {
                await Task.Delay(500);

                Task task = Task.Run(() => {
                    if (File.Exists(targetFile.FullName))
                    {
                        try
                        {
                            Process.Start(targetFile.FullName);
                        }
                        catch
                        {
                            Logger.LogWriterMessage.Info($"{targetFile.Name} fails to execute");
                        }
                    }
                });

                try
                {
                    Task.WaitAll(task, Task.Delay(3000));
                }
                catch (AggregateException exception)
                {
                    exception.Handle(ex =>
                    {
                        return true;
                    });
                    return;
                }

                await Task.Delay(100);

            }
            catch
            {

            }
        }
    }

    public class CloseProcess : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            FileInfo targetFile = dataMap.Get("param") as FileInfo;
            try
            {
                await Task.Delay(500);

                Task task = Task.Run(() => {
                    if (File.Exists(targetFile.FullName))
                    {
                        try
                        {
                            Process[] pllist = Process.GetProcessesByName(targetFile.Name);
                            foreach (var x in pllist) x.Kill();
                        }
                        catch
                        {
                            Logger.LogWriterMessage.Info($"{targetFile.Name} not founds so cannot close");
                        }
                    }
                });

                try
                {
                    Task.WaitAll(task, Task.Delay(3000));
                }
                catch (AggregateException exception)
                {
                    exception.Handle(ex =>
                    {
                        return true;
                    });
                    return;
                }

                await Task.Delay(100);

            }
            catch
            {

            }
        }
    }

    public class PressKey : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            int targetkey = dataMap.GetInt("param");
            try
            {
                await Task.Delay(500);

                Task task = Task.Run(() => {
                    KeyboardInputs.PressKey(targetkey);
                });

                try
                {
                    Task.WaitAll(task, Task.Delay(3000));
                }
                catch (AggregateException exception)
                {
                    exception.Handle(ex =>
                    {
                        return true;
                    });
                    return;
                }

                await Task.Delay(100);

            }
            catch
            {

            }
        }
    }

    public class PressKeyMulti : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            List<int> targetkey = dataMap.Get("param") as List<int>;
            try
            {
                await Task.Delay(500);

                Task task = Task.Run(() => {
                    KeyboardInputs.PressKeyMulti(targetkey);
                });

                try
                {
                    Task.WaitAll(task, Task.Delay(3000));
                }
                catch (AggregateException exception)
                {
                    exception.Handle(ex =>
                    {
                        return true;
                    });
                    return;
                }

                await Task.Delay(100);

            }
            catch
            {

            }
        }
    }

    public class SimpleJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Delay(1);
            JobKey jobKey = context.JobDetail.Key;
            Console.WriteLine($"SimpleJob says: {jobKey} executing at {DateTime.Now.ToString("r")}");
        }
    }
}
