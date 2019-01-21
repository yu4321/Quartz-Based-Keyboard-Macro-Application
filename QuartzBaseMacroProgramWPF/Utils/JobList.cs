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
using QuartzBaseMacroProgramWPF.Model;
using System.Threading;
using WindowsInput.Native;

namespace QuartzBasedMacroProgram.Utils
{
    public class PressKey : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            int targetkey = dataMap.GetInt("param");
            try
            {
                Task task = Task.Run(() => {
                    KeyboardInputs.PressKey(targetkey);
                });

                try
                {
                    Task.WaitAll(task, Task.Delay(1000));
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
                Task task = Task.Run(() => {
                    KeyboardInputs.PressKeyMulti(targetkey);
                });

                try
                {
                    Task.WaitAll(task, Task.Delay(1000));
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

    public class SequenceKey : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            SequenceKeyData model = dataMap.Get("param") as SequenceKeyData;
            while (GlobalVars.isworking)
            {
                Thread.Sleep(10);
            }
            try
            {
                GlobalVars.isworking = true;
                Task task = Task.Run(() => {
                    if (model.iscombomode)
                    {
                        Console.WriteLine($"-----연속 키 입력 시작: 동시입력 모드 ----- {context.JobDetail.Key}");
                        for (int i = 0; i < model.sendkeys.Count; i++)
                        {
                            if (model.isholding)
                            {
                                if (model.sendkeys[i] != 0)
                                {
                                    KeyboardInputs.PressKeyMulti(model.holdkey, model.sendkeys[i]);
                                    Console.WriteLine($"연속 키 입력: 동시입력 모드 동시입력 {(VirtualKeyCode)model.holdkey}, {(VirtualKeyCode)model.sendkeys[i]}");
                                    Thread.Sleep(model.interval);
                                }
                            }
                            else
                            {
                                if (model.sendkeys[i] != 0)
                                {
                                    KeyboardInputs.PressKey(model.sendkeys[i]);
                                    Console.WriteLine($"연속 키 입력: 동시입력 모드 단독입력 {(VirtualKeyCode)model.sendkeys[i]}");
                                    Thread.Sleep(model.interval);
                                }
                            }

                            if (model.unholdafter.Contains(i))
                            {
                                model.isholding = false;
                            }
                            else if (model.reholdafter.Contains(i))
                            {
                                model.isholding = true;
                            }
                        }
                        Console.WriteLine($"-----연속 키 입력 종료: 동시입력 모드----- {context.JobDetail.Key}");
                    }
                    else
                    {
                        Console.WriteLine($"-----연속 키 입력 시작: 단독입력 모드----- {context.JobDetail.Key}");
                        for (int i = 0; i < model.sendkeys.Count; i++)
                        {
                            if (model.sendkeys[i] != 0)
                            {
                                KeyboardInputs.PressKey(model.sendkeys[i]);
                                Console.WriteLine($"연속 키 입력: 단독입력 모드 단독입력 {(VirtualKeyCode)model.sendkeys[i]}");
                                Thread.Sleep(model.interval);
                            }
                        }
                        Console.WriteLine($"-----연속 키 입력 종료: 단독입력 모드----- {context.JobDetail.Key}");
                    }

                    model.isholding = true;
                });

                try
                {
                    Task.WaitAll(task);
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
            finally
            {
                GlobalVars.isworking = false;
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
