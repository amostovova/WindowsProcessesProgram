using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using WindowsProcessesProgram;


namespace Tests
{
    [TestFixture]
    public class Tests
    {
        #region ¬вод данных

        [Order(1)]
        [TestCase(Description = "Entering a 'help' on 'choose process name' level.")]
        public void InputHelp()
        {
            string message = Program.CheckInputProcess(input: "help", out Process testProcess);

            Assert.Multiple(() =>
            {
                Assert.True(message == "Choose a process name from the list:", 
                    "Wrong message after 'help list': " + message);
                Assert.True(testProcess is null, "A process name is exist without choosing.");
            });
        }

        [Order(1)]
        [TestCase(Description = "Entering a correct value on 'choose process name' level.")]
        public void InputCorrectProcess()
        {
            string testName = Process.GetProcesses()[0].ProcessName;

            string message = Program.CheckInputProcess(testName, out Process process);

            Assert.Multiple(() =>
            {
                Assert.True(message == "\nThanks for process name!", "Wrong message after correct name: " + message);
                Assert.True(process.ProcessName == testName, "A process name wasn't found.");
            });
        }

        [Order(1)]
        [TestCase(Description = "Entering an incorrect value on 'choose process name' level.")]
        public void InputWrongProcess()
        {
            string testName = "thisName definitely doesNotExist";

            string message = Program.CheckInputProcess(testName, out Process testProcess);

            Assert.Multiple(() =>
            {
                Assert.True(message == "You wrote incorrect value! Write a process name or 'help':", 
                    "Wrong message after incorrect name: " + message);
                Assert.True(testProcess is null, "A process name was found after incorrect input.");
            });
        }


        [Order(1)]
        [TestCase("1", "process maximum lifetime", Description = "Entering a number on 'choose process lifetime' level")]
        [TestCase("17", "monitoring frequency", Description = "Entering a number on 'choose monitoring frequency' level")]
        [TestCase("0,1", "process maximum lifetime", Description = "Entering a double number on 'choose process lifetime' level")]
        [TestCase("999,77", "monitoring frequency", Description = "Entering a double number on 'choose monitoring frequency' level")]
        public void InputCorrectNumber(string testValue, string valueName)
        {
            double minutes = 0;

            string message = Program.CheckInputMinutes(testValue, valueName, ref minutes);

            Assert.Multiple(() =>
            {
                Assert.True(message == "\nThanks for time!", "Wrong message after correct value: " + message);
                Assert.True(minutes == double.Parse(testValue), $"A {valueName} without value.");
            });
        }

        [Order(1)]
        [TestCase("0", "process maximum lifetime", Description = "Entering a zero number on 'choose process lifetime' level")]
        [TestCase("-10", "monitoring frequency", Description = "Entering a negative number on 'choose monitoring frequency' level")]
        [TestCase("-455,78", "process maximum lifetime", Description = "Entering a negative double number on 'choose process lifetime' level")]
        [TestCase("0,0", "monitoring frequency", Description = "Entering a zero double number on 'choose monitoring frequency' level")]
        public void InputIncorrectNumber(string testValue, string valueName)
        {
            double minutes = 0;

            string message = Program.CheckInputMinutes(testValue, valueName, ref minutes);

            Assert.Multiple(() =>
            {
                Assert.True(message == $"You wrote a number less than zero! Write a {valueName} (in minutes):", 
                    "Wrong message after incorrect value: " + message);
                Assert.True(minutes == 0, $"An incorrect {valueName} with value.");
            });
        }


        [Order(1)]
        [TestCase("process maximum lifetime", Description = "Entering not a number on 'choose process lifetime' level")]
        [TestCase("monitoring frequency", Description = "Entering not a number on 'choose monitoring frequency' level")]
        public void InputNotANumber(string valueName)
        {
            double minutes = 0;

            string message = Program.CheckInputMinutes(input: "notNumber", valueName, ref minutes);

            Assert.Multiple(() =>
            {
                Assert.True(message == $"You wrote incorrect value! Write a {valueName} (in minutes):", 
                    "Wrong message after incorrect value: " + message);
                Assert.True(minutes == 0, $"A {valueName} is exist after incorrect input.");
            });
        }

        #endregion


        [Order(2)]
        [TestCase(-1, 1, Description = "Monitoring a process that hasn't been found yet.")]
        [TestCase(0, 8.3, Description = "Monitoring a process that just start running.")]
        [TestCase(2.5, 2.5, Description = "Monitoring a process that has been running for less than the lifetime.")]
        [TestCase(9.9, 2, Description = "Monitoring a process that has been running for a little less than the lifetime.")]
        public void MonitoringCorrectProcess(double testWorkTime, double frequency)
        {
            Process testProcess = Process.GetProcesses()[0];

            double nextWorkTime = Program.FindingProcessInMonitoring(testProcess, testWorkTime, lifetime: 10, frequency);

            Assert.Multiple(() =>
            {
                Assert.True(nextWorkTime != testWorkTime, "The process wasn't found.");

                if (testWorkTime == -1)
                    Assert.True(nextWorkTime == 0, $"Incorrect work time. Expected: 0. Received: {nextWorkTime}");
                else
                    Assert.True(nextWorkTime == testWorkTime + frequency, $"Incorrect work time. " +
                        $"Expected: {testWorkTime + frequency}. Received: {nextWorkTime}");
            });
        }

        [Order(2)]
        [TestCase(-1, Description = "Monitoring a non-existent process that hasn't been found yet.")]
        [TestCase(3, Description = "Monitoring a non-existent process that has been running for less than the lifetime.")]
        public void MonitoringNonExistentProcess(double testWorkTime)
        {
            Process emptyProcess = new Process();

            double nextWorkTime = Program.FindingProcessInMonitoring(emptyProcess, testWorkTime, lifetime: 100, frequency: 2);

            Assert.True(nextWorkTime == testWorkTime, "Non-existent process was found.");
        }


        [Order(3)]
        // Idk how to make this test universal so as not to accidentally kill an important process...
        // So, let's kill Telegram and Excel!
        [TestCase("Telegram", 10, Description = "Monitoring and kill a process that has been running equal to the lifetime.")]
        [TestCase("EXCEL", 11, Description = "Monitoring and kill a process that has been running more than the lifetime.")]
        public void MonAndKillCorrectProcess(string testProcName, double testWorkTime)
        {
            Program.CheckInputProcess(testProcName, out Process testProcess);

            double workTime = Program.FindingProcessInMonitoring(testProcess, testWorkTime, lifetime: 10, frequency: 2);

            bool isExist = false;
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ToString() == testProcName.ToString())
                {
                    isExist = true;
                    break;
                }
            }

            Assert.Multiple(() =>
            {
                Assert.True(workTime == -1, "The process wasn't killed.");
                Assert.True(!isExist, "The process wasn't killed!"); 
            });
        }


        [Order(4)]
        [TestCase(Description = "Kill a process.")]
        public void KillCorrectProcess()
        {
            Program.CheckInputProcess(input: "notepad", out Process testProcess);

            string message = Program.KillProcess(testProcess);

            Assert.Multiple(() =>
            {
                Assert.True(message == $"Process: '{testProcess}' - killed.", 
                    "Wrong message after killing a process: " + message);
                Assert.True(Program.killLog[0] == testProcess, "Log list is empty after killing a process.");
            });
        }
        
        [Order(4)]
        [TestCase(Description = "Try to kill a non-existent process.")]
        public void KillNonExistentProcess()
        {
            Process emptyProcess = new Process();

            string message = Program.KillProcess(emptyProcess);

            Assert.Multiple(() =>
            {
                Assert.True(message == $"Process: '{emptyProcess}' - doesn't exist!",
                    "Wrong message after killing a non-existent process: " + message);
                Assert.True(Program.killLog.Count == 0, "Log list isn't empty after killing a non-existent process.");
            });
        }



        // double kill?
        // имитаци€ полного мониторинга
    }
}
