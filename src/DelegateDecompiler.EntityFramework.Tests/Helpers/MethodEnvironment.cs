// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DelegateDecompiler.EntityFramework.Tests.EfItems;

namespace DelegateDecompiler.EntityFramework.Tests.Helpers
{
    class MethodEnvironment : IDisposable
    {
        private readonly string memberName;
        private int lineNumberToUseIfException;
        private readonly ClassEnvironment classEnv;

        private bool finishedLinqPart;
        private bool hasLoggedError;

        private List<string> loggedLinqSql;
        private List<string> loggedDelegateDecompilerSql;

        public EfTestDbContext Db { get; private set; }

        public MethodEnvironment(ClassEnvironment classEnv,
                                  [CallerMemberName] string memberName = "",
                                  [CallerLineNumber] int envLineNumber = 0)
        {
            this.classEnv = classEnv;
            this.memberName = memberName;
            lineNumberToUseIfException = envLineNumber;

            loggedLinqSql = new List<string>();
            loggedDelegateDecompilerSql = new List<string>();

            Db = new EfTestDbContext();
#if !EF_CORE
            Db.Database.Log = LogEfSql;               //capture the sql for the linq part
#else
            //TODO: Caputre log?
#endif
        }

        public void AboutToUseDelegateDecompiler([CallerLineNumber] int lineNumber = 0)
        {
            finishedLinqPart = true;
            lineNumberToUseIfException = lineNumber;
        }

        public void LogFailer(int sourceLineNumber)
        {
            classEnv.AddMethodlog( new MethodLog(LogStates.NotSupported, memberName, sourceLineNumber, loggedLinqSql, loggedDelegateDecompilerSql)) ;
            hasLoggedError = true;
        }

        public void LogSuccess(int sourceLineNumber)
        {
            classEnv.AddMethodlog(new MethodLog(LogStates.Supported, memberName, sourceLineNumber, loggedLinqSql, loggedDelegateDecompilerSql));
            hasLoggedError = true;
        }

        private void LogExceptionHappended(int sourceLineNumber)
        {
            classEnv.AddMethodlog( new MethodLog( finishedLinqPart ? LogStates.NotSupported : LogStates.EvenLinqDidNotWork,
                memberName, sourceLineNumber, loggedLinqSql, loggedDelegateDecompilerSql));
        }

        public void Dispose()
        {

            if (!hasLoggedError)
                LogExceptionHappended(lineNumberToUseIfException);

            if (Db != null)
                Db.Dispose();
        }

        //----------------------------------------------------------
        //private helpers

        private void LogEfSql(string loggedData)
        {
            if (finishedLinqPart)
                loggedDelegateDecompilerSql.Add(loggedData);
            else
                loggedLinqSql.Add(loggedData);
        }
    }
}
