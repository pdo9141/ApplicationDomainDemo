using System;
using System.IO;
using System.Security;
using System.Security.Policy;
using System.Reflection;
using System.Diagnostics;
using DemoLibrary;

namespace ApplicationDomainApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessRequestBatch();
            // Only added to force a collection for demonstration purposes, please do not do this in production
            // inducing a GC is typically not good a good practice
            GC.Collect();
            Console.WriteLine("Completed the batch press enter to continue");
            Console.ReadLine();
        }

        ///************************************
        // * Normal 
        // *************************************/
        //private static void ProcessRequestBatch()
        //{            
        //    Stopwatch watch = new Stopwatch();
        //    Processor demoProcessor = new Processor();
        //    watch.Start();
        //    for (int index = 0; index < 10000; index++)
        //    {
        //        int id = index % 5000;
        //        Console.WriteLine(demoProcessor.ProcessRequest(id));
        //    }
        //    watch.Stop();
        //    Console.WriteLine("The processing took {0} ms to complete.", watch.ElapsedMilliseconds);
        //}

        ///************************************
        // * With App Domain for memory unload
        // *************************************/
        //private static void ProcessRequestBatch()
        //{
        //    Stopwatch watch = new Stopwatch();
        //    watch.Start();
        //    AppDomain domain = AppDomain.CreateDomain("ProcessingDomain");
        //    ProcessingWrapper demoProcessor = (ProcessingWrapper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ProcessingWrapper).FullName);

        //    demoProcessor.ProcessRequestBatch();
        //    AppDomain.Unload(domain);
        //    watch.Stop();
        //    Console.WriteLine("The processing took {0} ms to complete.", watch.ElapsedMilliseconds);
        //}

        ///************************************
        // * With App Domain for security
        // *************************************/
        //private static void ProcessRequestBatch()
        //{
        //    const string DirectoryName = @"C:\Test";
        //    Stopwatch watch = new Stopwatch();
        //    watch.Start();
        //    // Create the permission set found in the Trusted zone
        //    Evidence domainEvidence = new Evidence();
        //    //domainEvidence.AddHostEvidence(new Zone(SecurityZone.Trusted));
        //    domainEvidence.AddHostEvidence(new Zone(SecurityZone.Untrusted));

        //    PermissionSet permissions = SecurityManager.GetStandardSandbox(domainEvidence);
        //    // Create the directory to be used as the sandbox and copy the soon to be untrusted assembly to it
        //    if (!Directory.Exists(DirectoryName)) Directory.CreateDirectory(DirectoryName);
        //    string untrustedAssemblyPath = Assembly.GetAssembly(typeof(Processor)).Location;
        //    File.Copy(untrustedAssemblyPath, Path.Combine(DirectoryName, Path.GetFileName(untrustedAssemblyPath)), true);
        //    // Copy the one dependency assembly
        //    string trustedAssembly = Assembly.GetExecutingAssembly().Location;
        //    File.Copy(trustedAssembly, Path.Combine(DirectoryName, Path.GetFileName(trustedAssembly)), true);
        //    // Create the AppDomain configuration object 
        //    AppDomainSetup domainSetup = new AppDomainSetup() { ApplicationBase = DirectoryName };
        //    // Create the AppDomain with the full trust permissions in the sandbox defined
        //    AppDomain domain = AppDomain.CreateDomain("ProcessingDomain", domainEvidence, domainSetup, permissions, null);
        //    ProcessingWrapper demoProcessor = (ProcessingWrapper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ProcessingWrapper).FullName);
        //    demoProcessor.ProcessRequestBatch();
        //    AppDomain.Unload(domain);
        //    watch.Stop();

        //    Console.WriteLine("The processing took {0} ms to complete.", watch.ElapsedMilliseconds);
        //}

        private static void ProcessRequestBatch()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            AppDomain domain = AppDomain.CreateDomain("ProcessingDomain");
            domain.FirstChanceException += (sender, args) => Console.WriteLine("First chance exception happened and the processing domain recorded it.");
            domain.UnhandledException += (sender, args) => Console.WriteLine("Unhandled exception happened and the processing domain recorded it.");
            AppDomain.CurrentDomain.FirstChanceException += (sender, args) => Console.WriteLine("First chance exception happened and the default domain recorded it.");
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => Console.WriteLine("Unhandled exception happened and the default domain recorded it.");
            ProcessingWrapper demoProcessor = (ProcessingWrapper)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ProcessingWrapper).FullName);
            demoProcessor.ProcessRequestBatch();
            AppDomain.Unload(domain);
            watch.Stop();
            Console.WriteLine("The processing took {0} ms to complete.", watch.ElapsedMilliseconds);
        }
    }

    public sealed class ProcessingWrapper : MarshalByRefObject
    {
        public void ProcessRequestBatch()
        {
            Processor demoProcessor = new Processor();
            for (int index = 0; index < 10000; index++)
            {
                int id = index % 5000;
                Console.WriteLine(demoProcessor.ProcessRequestThrowEx(id));
            }
        }
    }

    //public sealed class ProcessingWrapper : MarshalByRefObject
    //{
    //    public void ProcessRequestBatch()
    //    {
    //        Processor demoProcessor = new Processor();
    //        for (int index = 0; index < 10000; index++)
    //        {
    //            int id = index % 5000;
    //            Console.WriteLine(demoProcessor.ProcessRequest(id));
    //        }
    //    }
    //}
}
