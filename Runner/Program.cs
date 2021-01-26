using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Simulation;
using Simulation.Utils;

namespace Runner {
    internal class Program {
        private Simulation.Runner Runner { get; set; }
        private LogEntry _lastEntry;
        private int _cursorTop;
        private Stopwatch Timer { get; set; }
        private readonly string[] _text = {
            "Active threads: *   ",
            "Simulations done: *   ",
            "Progress: *%   ",
            "Time remaining: *   "
        };
        private static void Main() {
            new Program().InstancedMain();
        }

        private void InstancedMain() {
            Initialize();
            
            var config = new SimulationConfig(100, 5000, SubjectWinAction);
            Runner = new Simulation.Runner(config);
            
            Timer.Start();
            Runner.RunSimulations();
            
            Timer.Stop();
            ShowInfo();
            Console.WriteLine();
            Console.WriteLine("Total time: " + Timer.Elapsed.TotalSeconds.ToString("F") + "s   ");
            Console.WriteLine("Total CPU time: "
                              + Runner.GetResults()
                                      .Sum(x=>(double)x.TotalMilliseconds / 1000)
                                      .ToString("F") + "s   ");
            Console.ReadLine();
        }

        private void Initialize() {
            Timer = new Stopwatch();
            Task.Run(DisplayInfo);
        }

        private async Task DisplayInfo() {
            _cursorTop = Console.CursorTop;
            Console.CursorVisible = false;
            var isDone = false;
            while (!isDone) {
                
                ShowInfo();

                ShowLogs();

                await Task.Delay(250);
                lock (Runner.Lock)
                    isDone = Runner.IsSimulationCompleted;
            }
            Console.CursorVisible = true;
            var info = GetInfo();
                
            Console.SetCursorPosition(0, _cursorTop);

            for (var j = 0; j < _text.Length; j++) {
                    Console.WriteLine(_text[j].Replace("*", info[j]));
            }
        }

        private void ShowInfo() {
            var values = GetInfo();
                
            Console.SetCursorPosition(0, _cursorTop);

            for (var i = 0; i < _text.Length; i++) {
                Console.WriteLine(_text[i].Replace("*", values[i]));
            }
        }

        private void ShowLogs() {
            var lastLog = Runner.Logger.GetLastN(LogType.Info).Last();
            if (_lastEntry != null && lastLog != null && Equals(lastLog, _lastEntry)) return;
            _lastEntry = lastLog;
            Console.WriteLine(_lastEntry?.ShowFull());
        }

        private List<string> GetInfo() {
            lock (Runner.Lock) {
                var list = new List<string>();
                var ratio = (double) Runner.ThreadsDone / Runner.Config.SimulationCount;
                var remainingTime = CalculateRemainingTime(ratio);
                list.Add(Runner.ActiveThreads.ToString(),
                         Runner.ThreadsDone + "/" + Runner.Config.SimulationCount,
                         (ratio * 100).ToString("F", CultureInfo.CurrentCulture),
                         remainingTime.ToString(@"hh\:mm\:ss"));
                return list;
            }
        }

        private TimeSpan CalculateRemainingTime(double ratio) {
            if (ratio < 0.1)
                return TimeSpan.Zero;
            lock (Runner.Lock) {
                var remainingMs = (Runner.ElapsedMilliseconds / ratio) * (1 - ratio);
                return TimeSpan.FromMilliseconds(remainingMs);
            }
        }

        #region === Subject Actions ===

        /// <summary>
        ///     Choose boxes based on the solution
        ///     of the '100 Prisoners' problem
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="room"></param>
        private static void SubjectWinAction(Subject subject, Room room) {
            var num = subject.Number;
            for (var i = 0; i < room.Drawers.Length / 2; i++) {
                if (subject.TryFindNumber(room, num))
                    return;
                num = room.Drawers[num];
            }
        }

        // ReSharper disable once UnusedMember.Local
        /// <summary>
        ///     Randomly choose boxes to open
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="room"></param>
        private void SubjectAction(Subject subject, Room room) {
            var numbers = new List<int>();
            for (var i = 0; i < room.Drawers.Length; i++) numbers.Add(i);

            numbers.Shuffle();
            numbers.Shuffle();
            numbers.Shuffle();
            numbers.RemoveRange(0, room.Drawers.Length / 2);

            var _ = numbers.Select(t => subject.TryFindNumber(room, t));
        }

        #endregion
    }
}