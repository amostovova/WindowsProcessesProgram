This is a tiny C# utility to monitor Windows processes and kill the processes that work longer than the threshold specified.

The command line utility expects three arguments: 
1. A process name (you also can input "help" to see the full list of processes);
2. A process maximum lifetime (in minutes); 
3. A monitoring frequency (in minutes). 

When you run the program, it starts monitoring processes with the frequency specified. 
If a process of interest lives longer than the allowed duration, 
the utility kills the process and adds the corresponding record to the log. 
When no process exists at any given moment, the utility continues monitoring (new processes might appear later). 
The utility stops when a special keyboard button "Q" is pressed.


Also the code covered with NUnit tests.
