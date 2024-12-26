using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PennyProject.Helper
{
    public enum LogLevel
    {
        /// <summary>紀錄</summary>
        Debug,
        /// <summary>警告</summary>
        Warn,
        /// <summary>錯誤</summary>
        Error
    }
    public class NlogHelper
    {
        private static Logger logRec = LogManager.GetCurrentClassLogger();
        public static void SetLogger(string targetName)
        {
            logRec = LogManager.GetLogger(targetName);
        }

        public static void WriteLog(
            LogLevel level,
            string content,
            Exception ex = null,
            [CallerLineNumber] int lineNumber = 0,
            string className = null)
        {
            string line = ex != null ? GetExceptionLineNumber(ex).ToString() : lineNumber.ToString();
            string exMsg = ex != null ? string.Format("{0}ExMsg:{1}", !string.IsNullOrEmpty(content) ? "> " : string.Empty, GetExceptionMessage(ex)) : string.Empty;
            string method = GetSourceMethod(ex);
            string classMsg = className == null ? method.Split('.').Last() : className;
            string logMsg = string.Format("{0} | Line:{1} | {2} | {3} {4}", method, line, classMsg, content, exMsg);
            switch (level)
            {
                case LogLevel.Debug:
                    logRec.Debug("{0} | Line:{1} | {2} | {3} {4}", method, line, classMsg, content, exMsg);
                    break;

                case LogLevel.Warn:
                    logRec.Warn("{0} | Line:{1} | {2} | {3} {4}", method, line, classMsg, content, exMsg);
                    break;

                case LogLevel.Error:
                    logRec.Error("{0} | Line:{1} | {2} | {3} {4}", method, line, classMsg, content, exMsg);
                    break;
            }
        }

        private static int GetExceptionLineNumber(Exception ex)
        {
            var st = new StackTrace(ex, true);
            var firstFrame = st.GetFrame(0);
            if (firstFrame == null)
            {
                return 0;
            }

            if (firstFrame.GetFileLineNumber() > 0)
            {
                return firstFrame.GetFileLineNumber();
            }

            for (int i = st.GetFrames().Length - 1; i > 0; i--)
            {
                if (st.GetFrame(i).GetFileLineNumber() > 0)
                {
                    return st.GetFrame(i).GetFileLineNumber();
                }
            }

            return 0;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetSourceMethod(Exception ex = null)
        {
            if (ex == null)
            {
                StackFrame frame = new StackTrace().GetFrame(2);
                return string.Format("{0}.{1}", frame.GetMethod().DeclaringType.FullName, frame.GetMethod().Name);
            }

            var st = new StackTrace(ex, true);
            var firstFrame = st.GetFrame(0);

            if (firstFrame == null)
            {
                return string.Empty;
            }

            if (firstFrame.GetFileLineNumber() > 0)
            {
                return string.Format(
                    "{0}.{1}",
                    firstFrame.GetMethod().DeclaringType.FullName,
                    firstFrame.GetMethod().Name);
            }


            for (int i = st.GetFrames().Length - 1; i > 0; i--)
            {
                if (st.GetFrame(i).GetFileLineNumber() > 0)
                {
                    return string.Format(
                        "{0}.{1}",
                        st.GetFrame(i).GetMethod().DeclaringType.FullName,
                        st.GetFrame(i).GetMethod().Name);
                }
            }

            return string.Empty;
        }

        public static string GetExceptionMessage(Exception ex)
        {
            string trace = ex.ToString();
            int idx = trace.IndexOf("於lambda_method(Closure , Object , Object[] )");
            if (idx > -1)
            {
                trace = trace.Substring(0, idx);
            }

            return trace;
        }
    }
}
