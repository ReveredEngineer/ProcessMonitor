# Install psutil by typing:
# pip install psutil colorama

import psutil
import time
from colorama import Fore, Style, init

# Initialize colorama for colored output
init()

# Thresholds for color coding
HIGH_CPU = 50
MEDIUM_CPU = 30
HIGH_MEM = 500  # MB
MEDIUM_MEM = 300  # MB

def get_color(value, high, medium):
    """Returns color based on thresholds."""
    if value >= high:
        return Fore.RED
    elif value >= medium:
        return Fore.YELLOW
    return Fore.GREEN

def monitor_single_process(process_name):
    """Monitor a single process by name."""
    while True:
        found = False
        print(f"\nMonitoring process: {process_name}")
        print("-" * 50)

        for proc in psutil.process_iter(attrs=['pid', 'name', 'cpu_percent', 'memory_info']):
            if proc.info['name'].lower() == process_name.lower():
                found = True
                cpu = proc.info['cpu_percent']
                mem = proc.info['memory_info'].rss / (1024 * 1024)  # Convert to MB

                print(f"PID: {proc.info['pid']} | {Fore.CYAN}{proc.info['name']}{Style.RESET_ALL}")
                print(f"CPU: {get_color(cpu, HIGH_CPU, MEDIUM_CPU)}{cpu:.2f}%{Style.RESET_ALL}")
                print(f"Memory: {get_color(mem, HIGH_MEM, MEDIUM_MEM)}{mem:.2f} MB{Style.RESET_ALL}")
                print("-" * 50)

        if not found:
            print(Fore.RED + f"Process {process_name} not found!" + Style.RESET_ALL)

        time.sleep(5)

def monitor_all_processes():
    """Monitor all running processes."""
    while True:
        print("\nMonitoring ALL processes...")
        print("-" * 70)
        
        processes = sorted(psutil.process_iter(attrs=['pid', 'name', 'cpu_percent', 'memory_info']), 
                           key=lambda p: p.info['cpu_percent'], reverse=True)

        for proc in processes:
            cpu = proc.info['cpu_percent']
            mem = proc.info['memory_info'].rss / (1024 * 1024)  # Convert to MB

            print(f"PID: {proc.info['pid']:5} | {proc.info['name'][:20]:20} | "
                  f"CPU: {get_color(cpu, HIGH_CPU, MEDIUM_CPU)}{cpu:.2f}%{Style.RESET_ALL} | "
                  f"Memory: {get_color(mem, HIGH_MEM, MEDIUM_MEM)}{mem:.2f} MB{Style.RESET_ALL}")

        time.sleep(5)

if __name__ == "__main__":
    process_name = input("Enter process name to monitor (or type 'all' for all processes): ")

    if process_name.lower() == "all":
        monitor_all_processes()
    else:
        monitor_single_process(process_name)
