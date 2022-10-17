// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using System.Runtime.CompilerServices;
using DelegateDecompiler.EntityFramework.Tests.EfItems;

namespace DelegateDecompiler.EntityFramework.Tests.Helpers
{
    class ClassEnvironment
    {
        public ClassLog ThisClassesLog { get; private set; }

        public ClassEnvironment(string alternativeSourceFilePath = null, [CallerFilePath] string sourceFilePath = "")
        {
            ThisClassesLog = new ClassLog(alternativeSourceFilePath ?? sourceFilePath);
            MasterEnvironment.AddClassLog(ThisClassesLog);

            //ensure the database is set up
            using (var db = new EfTestDbContext())
            {
                if (!db.EfParents.Any()) db.ResetDatabaseContent();     //make sure database is loaded after change to model
            }
        }

        public void AddMethodlog(MethodLog log)
        {
            ThisClassesLog.MethodLogs.Add(log);
            //Console.WriteLine(log);
        }

        public MethodLog GetLastMethodLog()
        {
            return ThisClassesLog.MethodLogs.Last();
        }

    }
}
