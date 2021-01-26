namespace Simulation {
    public class ScResult {
        public ScResult(int subjectCount, bool isDone, bool isSuccessful, long totalMilliseconds) {
            SubjectCount = subjectCount;
            IsDone = isDone;
            IsSuccessful = isSuccessful;
            TotalMilliseconds = totalMilliseconds;
        }

        public int SubjectCount { get; }
        public bool IsDone { get; }
        public bool IsSuccessful { get; }
        public long TotalMilliseconds { get; }
    }
}