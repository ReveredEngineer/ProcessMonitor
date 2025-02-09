using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

class ProcessMonitor
{
    // Thresholds for color coding
    static int HIGH_CPU = 50;
    static int MEDIUM_CPU = 30;
    static int HIGH_MEM = 500;  // MB
    static int MEDIUM_MEM = 300;  // MB

    static ConsoleColor GetColor(double value, int high, int medium)
    {
        if (value >= high) return ConsoleColor.Red;
        if (value >= medium) return ConsoleColor.Yellow;
        return ConsoleColor.Green;
    }

    static void MonitorSingleProcess(string processName)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Monitoring process: {processName}");
            Console.WriteLine(new string('-', 50));

            var processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Process {processName} not found!");
                Console.ResetColor();
            }

            foreach (var process in processes)
            {
                double cpuUsage = GetCpuUsage(process);
                double memoryUsage = process.WorkingSet64 / (1024.0 * 1024.0); // Convert to MB

                Console.WriteLine($"PID: {process.Id} | {process.ProcessName}");
                Console.ForegroundColor = GetColor(cpuUsage, HIGH_CPU, MEDIUM_CPU);
                Console.WriteLine($"CPU: {cpuUsage:F2}%");
                Console.ForegroundColor = GetColor(memoryUsage, HIGH_MEM, MEDIUM_MEM);
                Console.WriteLine($"Memory: {memoryUsage:F2} MB");
                Console.ResetColor();
                Console.WriteLine(new string('-', 50));
            }

            Thread.Sleep(5000);
        }
    }

    static void MonitorAllProcesses()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Monitoring ALL processes...");
            Console.WriteLine(new string('-', 70));

            var processes = Process.GetProcesses()
                .OrderByDescending(p => GetCpuUsage(p))
                .ToList();

            foreach (var process in processes)
            {
                double cpuUsage = GetCpuUsage(process);
                double memoryUsage = process.WorkingSet64 / (1024.0 * 1024.0); // Convert to MB

                Console.Write($"PID: {process.Id,-5} | {process.ProcessName,-20} | ");
                Console.ForegroundColor = GetColor(cpuUsage, HIGH_CPU, MEDIUM_CPU);
                Console.Write($"CPU: {cpuUsage:F2}% | ");
                Console.ForegroundColor = GetColor(memoryUsage, HIGH_MEM, MEDIUM_MEM);
                Console.WriteLine($"Memory: {memoryUsage:F2} MB");
                Console.ResetColor();
            }

            Thread.Sleep(5000);
        }
    }

    static double GetCpuUsage(Process process)
    {
        try
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = process.TotalProcessorTime;
            Thread.Sleep(500);
            var endTime = DateTime.UtcNow;
            var endCpuUsage = process.TotalProcessorTime;
            double cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            double totalMsPassed = (endTime - startTime).TotalMilliseconds;
            return (cpuUsedMs / totalMsPassed) * 100.0;
        }
        catch
        {
            return 0;
        }
    }

    static void Main()
    {
        Console.Write("Enter process name to monitor (or type 'all' for all processes): ");
        string processName = Console.ReadLine();

        if (processName.ToLower() == "all")
        {
            MonitorAllProcesses();
        }
        else
        {
            MonitorSingleProcess(processName);
        }
    }
}
