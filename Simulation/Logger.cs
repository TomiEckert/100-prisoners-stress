using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Simulation.Utils;

namespace Simulation {
    public class Logger {
        public Logger() {
            LogEntries = new List<LogEntry>();
            Timer = new Stopwatch();
        }

        private static readonly object _lock = new object();
        private List<LogEntry> LogEntries { get; }
        private Stopwatch Timer { get; }

        public void Log(string message, LogType logType) {
            lock (_lock) {
                LogEntries.Add(new LogEntry(Timer.Elapsed, GetSender(), message, logType));
            }
        }

        public void Log(string message) {
            lock (_lock) {
                LogEntries.Add(new LogEntry(Timer.Elapsed, GetSender(), message));
            }
        }

        public IReadOnlyCollection<LogEntry> GetLastN() {
            IEnumerable<LogEntry> lastNEntries;

            lock (_lock) {
                lastNEntries = new List<LogEntry> {LogEntries.Last()};
            }

            return lastNEntries.ToReadOnlyCollection();
        }

        public IReadOnlyCollection<LogEntry> GetLastN(int n) {
            IEnumerable<LogEntry> lastNEntries;

            lock (_lock) {
                lastNEntries = LogEntries.GetRange(LogEntries.Count - (n + 1), n);
            }

            return lastNEntries.ToReadOnlyCollection();
        }

        public IReadOnlyCollection<LogEntry> GetLastN(LogType logType) {
            IEnumerable<LogEntry> lastNEntries;

            lock (_lock) {
                lastNEntries = new List<LogEntry> {LogEntries.Last(x => x.LogType == logType)};
            }

            return lastNEntries.ToReadOnlyCollection();
        }

        public IReadOnlyCollection<LogEntry> GetLastN(int n, LogType logType) {
            IEnumerable<LogEntry> lastNEntries;

            lock (_lock) {
                lastNEntries = LogEntries.Where(x => x.LogType == logType)
                                         .Skip(LogEntries.Count - n);
            }

            return lastNEntries.ToReadOnlyCollection();
        }

        public IReadOnlyCollection<LogEntry> GetAllLogs() {
            IReadOnlyCollection<LogEntry> readOnlyCollection;

            lock (_lock) {
                readOnlyCollection = LogEntries.ToReadOnlyCollection();
            }

            return readOnlyCollection;
        }

        public IReadOnlyCollection<LogEntry> GetAllLogs(LogType logType) {
            IEnumerable<LogEntry> filteredEntries;

            lock (_lock) {
                filteredEntries = LogEntries.Where(x => x.LogType == logType);
            }

            return filteredEntries.ToReadOnlyCollection();
        }

        private MethodInfo GetSender() {
            return (MethodInfo)new StackTrace().GetFrame(2).GetMethod();
        }
    }

    public class LogEntry {
        public LogEntry(TimeSpan time, MethodInfo sender, string message) {
            Time = time;
            Sender = sender;
            Message = message;
            LogType = LogType.Info;
        }

        public LogEntry(TimeSpan time, MethodInfo sender, string message, LogType logType) {
            Time = time;
            Sender = sender;
            Message = message;
            LogType = logType;
        }

        public TimeSpan Time { get; }
        public MethodInfo Sender { get; }
        public string Message { get; }
        public LogType LogType { get; }

        public string ShowFull() {
            var time = Time.TotalMilliseconds < 10 ? "[00:00:00]" : Time.ToString(@"[hh\:mm\:ss] ");
            var type = "[" + LogType + "]";
            var sender = "[" + Sender.DeclaringType?.FullName + "." + Sender.Name + "]";
            return time + type + sender + Message;
        }
    }

    public enum LogType {
        Info,
        Debug
    }
}