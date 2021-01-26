using System;

namespace Simulation {
    public class SimulationConfig {
        public SimulationConfig(int simulationCount, int subjectCount, Action<Subject, Room> subjectAction) {
            SimulationCount = simulationCount;
            SubjectCount = subjectCount;
            SubjectAction = subjectAction;
            CpuThreadCount = Environment.ProcessorCount;
        }

        public SimulationConfig(int simulationCount, int subjectCount, Action<Subject, Room> subjectAction,
                                int cpuThreadCount) {
            SimulationCount = simulationCount;
            SubjectCount = subjectCount;
            SubjectAction = subjectAction;
            CpuThreadCount = cpuThreadCount;
        }

        public int SimulationCount { get; }
        public int SubjectCount { get; }
        public int CpuThreadCount { get; }
        public Action<Subject, Room> SubjectAction { get; }
    }
}