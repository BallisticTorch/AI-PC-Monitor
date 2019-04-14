using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Speech.Synthesis;

namespace PC_Monitor
{
    class Program
    {
        private static SpeechSynthesizer synth = new SpeechSynthesizer();
     
        static void Main(string[] args)
        {     
            // This will greet the user in the default voice
            synth.Speak("Welcome to PC Monitor version one point oh!");

            #region My Performance Counters
            // This will pull the current CPU load in percentage
            PerformanceCounter perfCpuCount = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            perfCpuCount.NextValue();

            // This will pull the current available memory in MegaBytes
            PerformanceCounter perfMemCount = new PerformanceCounter("Memory", "Available MBytes");
            perfMemCount.NextValue();

            // This will pull the Up Time value of the current session
            PerformanceCounter perfUpTimeCount = new PerformanceCounter("System", "System Up Time");
            perfUpTimeCount.NextValue();
            #endregion

            TimeSpan uptimeSpan = TimeSpan.FromSeconds(perfUpTimeCount.NextValue());
            string systemUptimeMessage = string.Format("The current system up time is {0} days {1} hours {2} minutes {3} seconds",
                (int)uptimeSpan.TotalDays,
                (int)uptimeSpan.Hours,
                (int)uptimeSpan.Minutes,
                (int)uptimeSpan.Seconds);

            // Tell the user what the current System Uptime is
            BryanSpeak(systemUptimeMessage, VoiceGender.Female, 2);

            // Infinite While loop
            while (true)
            {
                // Get current performance counter values
                int currentCpuPercentage = (int)perfCpuCount.NextValue();
                int currentAvailableMemory = (int)perfMemCount.NextValue();
                
                // Every 1 second, print CPU load percentage to screen
                Console.WriteLine("CPU Load        : {0}%", currentCpuPercentage);
                Console.WriteLine("Available Memory: {0}MB", currentAvailableMemory);

                // Only tell user when CPU usage is over 80%
                if (currentCpuPercentage > 80)
                {
                    if(currentCpuPercentage == 100)
                    {
                        string cpuLoadVocalMessage = String.Format("WARNING: Holy shit, shut down your porn before your computer explodes!", currentCpuPercentage);
                        BryanSpeak(cpuLoadVocalMessage, VoiceGender.Male, 2);
                    }
                    else
                    {
                        string cpuLoadVocalMessage = String.Format("The current CPU load is {0} percent", currentCpuPercentage);
                        BryanSpeak(cpuLoadVocalMessage, VoiceGender.Female, 2);
                    }
                    
                }

                // Only tell user when available Memory is less than 1GB
                if (currentAvailableMemory < 1024)
                {
                    string memAvailableVocalMessage = String.Format("You have {0} megabyte of memory available", currentAvailableMemory);
                    BryanSpeak(memAvailableVocalMessage, VoiceGender.Male, 2);
                }
                
                Thread.Sleep(1000);
            } // End of loop
        }

        /// <summary>
        /// Speaks with a selected voice and rate
        /// </summary>
        /// <param name="message"></param>
        /// <param name="voiceGender"></param>
        /// <param name="rate"></param>
        public static void BryanSpeak(string message, VoiceGender voiceGender, int rate)
        {
            synth.SelectVoiceByHints(voiceGender);
            synth.Speak(message);
            synth.Rate = rate;
        }
    }
}
