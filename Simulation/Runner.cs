using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable 1998

namespace Simulation {
    public class Runner {
        public Runner(SimulationConfig config) {
            Config = config;
            Timer = new Stopwatch();
            Threads = new List<Task<ScResult>>();
            Random = new Random();
            Logger = new Logger();
        }

        public readonly object Lock = new object();
        public SimulationConfig Config { get; }
        private Stopwatch Timer { get; }
        private List<Task<ScResult>> Threads { get; }
        public int ThreadsDone => Threads.Count(x => x.IsCompleted);
        public double ElapsedMilliseconds => Timer.Elapsed.TotalMilliseconds;
        public bool IsSimulationCompleted => ThreadsDone == Config.SimulationCount;
        public int ActiveThreads { get; private set; }
        private Random Random { get; }
        public Logger Logger { get; }

        public void RunSimulations() {
            Logger.Log("Simulation started", LogType.Info);
            Timer.Start();

            LaunchThreads();

            WaitForLastThread();
        }

        private void LaunchThreads() {
            for (var i = 0; i < Config.SimulationCount; i++) {

                lock (Lock) ActiveThreads = i - ThreadsDone;

                lock (Lock) {
                    if (ActiveThreads < Config.CpuThreadCount) {
                        StartNewThread();
                        Logger.Log("Starting new thread", LogType.Debug);
                    }
                    else {
                        Task.Delay(50).Wait();
                        i--;
                    }
                }
            }
        }

        private void WaitForLastThread() {
            while (ThreadsDone != Config.SimulationCount) Task.Delay(50).Wait();
            Logger.Log("Simulation ended", LogType.Info);
        }

        private void StartNewThread() {
            Threads.Add(Task.Run(RunSimulation));
        }

        public IEnumerable<ScResult> GetResults() {
            return Threads.Select(x => x.Result);
        }

        private async Task<ScResult> RunSimulation() {
            var rand = new Random(Random.Next());
            var sc = new SubjectController(Config.SubjectCount, rand) {Action = Config.SubjectAction};
            var result = sc.Run();
            return result;
        }
    }
}