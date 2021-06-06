using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AppKiller
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Process name: {args[0]}\nLifeTimeMinutes: {args[1]}\nCheckFrequency: {args[2]}\n");
            
            KillProcessOverTimeCycle(args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));

            Console.ReadLine();
        }

        static Process GetProcessToKillIfLifeTimeExpires(string ProcessNameToKill, int LifeTimeMinutes)
        {
            Process[] allProcesses = Process.GetProcesses();
            
            foreach(var process in allProcesses)
            {
                DateTime CurrentTime = DateTime.Now;
                var ProcName = process.ProcessName;
                try
                {
                    if (process.ProcessName.ToLower() == ProcessNameToKill.ToLower() && CurrentTime.Subtract(process.StartTime).TotalMinutes > LifeTimeMinutes)
                    {
                        Console.WriteLine(process.ProcessName);
                        return process;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
                
            }
            return null;
        }

        static void KillProcessOverTime(string ProcessName, int LifeTimeMinutes)
        {
            Process processToKill = GetProcessToKillIfLifeTimeExpires(ProcessName, LifeTimeMinutes);
                        
            processToKill?.Kill();
            if (processToKill == null)
            {
                Console.WriteLine("Process is not ready to death");
            }
            else
            {
                Console.WriteLine($"I killed {processToKill.ProcessName}");
            }            
        }

        public static async Task SetInterval(Action<string, int> action, string ProcessName, int LifeTimeMinutes, TimeSpan timeout)
        {
            action(ProcessName, LifeTimeMinutes);

            await Task.Delay(timeout).ConfigureAwait(false);

            SetInterval(action, ProcessName, LifeTimeMinutes, timeout);
        }

        public static void KillProcessOverTimeCycle(string ProcessName, int LifeTimeMinutes, int CheckFrequency)
        {
            TimeSpan FrequenceInterval = TimeSpan.FromMinutes(CheckFrequency);

            SetInterval(KillProcessOverTime, ProcessName, LifeTimeMinutes, FrequenceInterval);
        }
    }
}
