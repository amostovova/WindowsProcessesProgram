using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;


namespace WindowsProcesses
{
    public class Program
    {
        public static List<Process> killLog = new List<Process>();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello! I need some information from you.");

            string myProcessName = InputProcess();

            double processLifetime = InputMinutes("process maximum lifetime");
            double processMonFrequency = InputMinutes("monitoring frequency");

            MonitoringProcess(myProcessName, processLifetime, processMonFrequency);

            if (killLog.Count != 0)
            {
                Console.WriteLine("List of killed processes:");
                foreach (Process p in killLog)
                    Console.WriteLine(p);
            }
        }


        static string InputProcess() 
        {
            Console.WriteLine("Write a process name or 'help' to see the full list of processes.");
            string myProcessName = null;

            while (myProcessName is null)
            {
                myProcessName = CheckInputProcess(Console.ReadLine().ToLower());
            }
            return myProcessName;
        }

        public static string CheckInputProcess(string input)
        {
            if (input == "help")
            {
                Console.WriteLine("Choose a process name from the list below:");
                foreach (string process in PrintProcessesNames())
                    Console.WriteLine(process);
            }
            else if (input.Length < 2)
            {
                Console.WriteLine("You wrote incorrect value! Write a process name or 'help':");
            }
            else
            {
                return input;
            }

            return null;

            //вернуть присваивание процесса?
            //добавить поиск имени по части?
        }


        static IEnumerable<string> PrintProcessesNames()
        {
            List<string> unicProcesses = new List<string>();
            foreach (Process process in Process.GetProcesses())
                unicProcesses.Add(process.ProcessName);

            unicProcesses.Sort();
            return unicProcesses.Distinct();
        }


        static double InputMinutes(string valueName) 
        {
            double minutes = 0;
            while (minutes <= 0)
            {
                Console.WriteLine($"\nWrite a \"{valueName}\" (in minutes):");
                minutes = CheckInputMinutes(Console.ReadLine());
            }
            return minutes;
        }

        public static double CheckInputMinutes(string input)
        {
            if (double.TryParse(input, out double time))
            {
                if (time <= 0)
                    Console.Write($"You wrote a number less then or equal to zero! ");
                return time;
            }
            Console.Write($"You wrote incorrect value! ");
            return 0;
        }


        static void MonitoringProcess(String processName, double lifetime, double monFrequency)
        {
            Console.WriteLine("\nStart monitoring. For exit press 'q'");

            double workTime = 0;
            Process process = null;
            
            while (!IsExit())
            {
                if (process is null)
                {
                    workTime = 0;
                    process = FindProcessByName(processName);
                }
                if (!(process is null))
                {
                    if (workTime == 0)
                    {
                        DateTime startTime = process.StartTime;
                        workTime = Math.Round((DateTime.Now - startTime).TotalSeconds / 60, 3);
                    }
                    CheckProcessTime(ref process, workTime, lifetime);
                    workTime = Math.Round(workTime + monFrequency, 3);
                }
                Thread.Sleep((int)(monFrequency * 1000 * 60));
            }

            Console.WriteLine("\nThank you for choosing our service :)");
        }

        public static Process FindProcessByName(String processName)
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (processName == process.ProcessName.ToLower())
                {
                    Console.WriteLine("The process was found!");
                    return process;
                }
            }
            Console.WriteLine("Process " + processName + " not found.");
            return null;
        }

        static bool IsExit()
        {
            if (Console.KeyAvailable)
                return (Console.ReadKey(true).Key == ConsoleKey.Q);
            return false;
        }


        public static void CheckProcessTime(ref Process process, double workTime, double lifetime)
        {
            if (IsProcessStillExist(ref process))
                if (workTime >= lifetime)
                    Console.WriteLine(KillProcess(ref process));
                else
                    Console.WriteLine("Process work time = " + workTime + " min.");
            else
                Console.WriteLine($"The process was closed.");
        }

        public static bool IsProcessStillExist(ref Process process)
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (process.ToString() == p.ToString())
                {
                    process = p;
                    return true;
                }
            }
            process = null;
            return false;
        }

        public static string KillProcess(ref Process process)
        {
            process.Kill();
            process.WaitForExit();
            killLog.Add(process);

            string ret = $"Process: '{process}' - killed.";
            process = null;
            return ret;
        }
    }
}
