using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Net;
using System.Net.NetworkInformation;
using RegistryStrings = System.Collections.Generic.Dictionary<string, string>;

namespace Umbriel.GIS.ArcGISVersion
{
    public class Program
    {
        static void Main(string[] args)
        {
            string machine = string.Empty;

            if (args.Length.Equals(0))
            {
                machine = System.Environment.MachineName;
            }
            else
            {
                machine = args[0];
            }

            char delimiter = '-';
            bool appendDelimiter = false;

            if (Ping(machine) == IPStatus.Success)
            {
                StringBuilder sb = new StringBuilder();
                RegistryStrings vals = OpenKey(machine);

                if (vals.ContainsKey("SPBuild"))
                {
                    sb.Append(vals["SPBuild"]);
                    appendDelimiter = true;
                }

                if (System.Environment.CommandLine.Contains("-SPDisplayName") || System.Environment.CommandLine.Contains("-d"))
                {
                    if (appendDelimiter)
                        sb.Append(delimiter);

                    if (vals.ContainsKey("SPDisplayName"))
                    {
                        sb.Append(vals["SPDisplayName"]);
                    }
                }

                Console.WriteLine(sb.ToString());
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Machine Not Found.");
            }
        }

        private static RegistryStrings OpenKey(string remoteName)
        {
            RegistryStrings regValues = new RegistryStrings();

            try
            {
                RegistryKey registryKey = RegistryKey.OpenRemoteBaseKey(
       RegistryHive.LocalMachine, remoteName).OpenSubKey(@"Software\ESRI\ArcInfo\Desktop\8.0");

                string[] valuenames = registryKey.GetValueNames();


                foreach (string regvalue in valuenames)
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("{0}-{1}", regvalue, registryKey.GetValue(regvalue)));
                    regValues.Add(regvalue, registryKey.GetValue(regvalue).ToString());
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Could not connect to {0} registry", remoteName);
                System.Diagnostics.Trace.WriteLine(message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                Console.WriteLine(message);
            }
            return regValues;
        }
        
        private static IPStatus Ping(string hostname)
        {
            try
            {
                Ping pingSender = new Ping();
                PingOptions options = new PingOptions();

                // Use the default Ttl value which is 128,
                // but change the fragmentation behavior.
                options.DontFragment = true;

                // Create a buffer of 32 bytes of data to be transmitted.
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 120;
                PingReply reply = pingSender.Send(hostname, timeout, buffer, options);
                return reply.Status;
                //if (reply.Status == IPStatus.Success)
                //{
                //    Console.WriteLine("Address: {0}", reply.Address.ToString());
                //    Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                //    Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                //    Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                //    Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);

                //}
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                Console.WriteLine("Cannot find " + hostname);
                return IPStatus.Unknown;
            }
        }
    }
}
