using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;


namespace WindowsProcessesProgram
{
    public class Program
    {
        public static List<Process> killLog = new List<Process>();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello! I need some information from you.");

            Process myProcess = InputProcess();

            double processLifetime = InputMinutes("process maximum lifetime");
            double processMonFrequency = InputMinutes("monitoring frequency");

            string finalMessage = MonitoringProcess(myProcess, processLifetime, processMonFrequency);
            Console.WriteLine(finalMessage);

            if (killLog.Count != 0)
            {
                Console.WriteLine("List of killed processes:");
                foreach (Process p in killLog)
                    Console.WriteLine(p);
            }
        }


        static Process InputProcess() 
        {
            Console.WriteLine("Write a process name or 'help' to see the full list of processes:");
            Process myProcess = null;

            while (myProcess is null)
            {
                string message = CheckInputProcess(Console.ReadLine(), out myProcess);

                Console.WriteLine(message);
            }
            return myProcess;
        }

        public static string CheckInputProcess(string input, out Process myProcess)
        {
            myProcess = null;

            if (input == "help")
            {
                foreach (string process in PrintProcessesNames())
                    Console.WriteLine(process);

                return "Choose a process name from the list:";
            }

            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName == input)
                {
                    myProcess = process;
                    return "\nThanks for process name!";
                }
            }

            return "You wrote incorrect value! Write a process name or 'help':";
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
            Console.WriteLine($"\nWrite a {valueName} (in minutes):");
            double minutes = 0;

            while (minutes == 0)
            {
                string message = CheckInputMinutes(Console.ReadLine(), valueName, ref minutes);

                Console.WriteLine(message);
            }
            return minutes;
        }

        public static string CheckInputMinutes(string input, string valueName, ref double minutes)
        {
            if (double.TryParse(input, out double j))
            {
                if (j < 0)
                    return $"You wrote a number less than zero! Write a {valueName} (in minutes):";
                minutes = j;
                return "\nThanks for time!";
            }
            return $"You wrote incorrect value! Write a {valueName} (in minutes):";
        }


        static string MonitoringProcess(Process process, double lifetime, double frequency)
        {
            Console.WriteLine("\nStart monitoring. For exit press 'q'");
            double workTime = -1;

            while (!IsExit())
            {
                workTime = FindingProcessInMonitoring(process, workTime, lifetime, frequency);

                Thread.Sleep((int)(frequency * 1000)); //* 60 
            }
            return "\nThank you for choosing our service :)";
        }

        static bool IsExit()
        {
            if (Console.KeyAvailable)
                return (Console.ReadKey(true).Key == ConsoleKey.Q);
            return false;
        }


        public static double FindingProcessInMonitoring(Process process, double workedTime, double lifetime, double frequency)
        {
            if (workedTime >= lifetime)
            {
                string message = KillProcess(process);
                Console.WriteLine(message);
            }
            else if (FindingProcess(ref process))
            {
                Console.WriteLine("Process worked: " + workedTime);
                return (workedTime < 0) ? 0 : workedTime + frequency;
            }
            return -1;
        }


        public static string KillProcess(Process process)
        {
            if (FindingProcess(ref process))
            {
                process.Kill();
                process.WaitForExit();
                killLog.Add(process);

                return $"Process: '{process}' - killed.";
            }
            return $"Process: '{process}' - doesn't exist!";
        }


        public static bool FindingProcess(ref Process process)
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (process.ToString() == p.ToString())
                {
                    process = p;
                    return true;
                }
            }
            return false;
        }
    }
}
