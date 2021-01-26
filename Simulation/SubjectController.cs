using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Simulation {
    public class SubjectController {
        #region === Properties ===

        public Action<Subject, Room> Action { get; set; }

        private bool IsDone => Subjects.All(x => x.IsDone);

        private bool IsSuccessful => Subjects.All(x => x.IsSuccessful);

        private Room Room { get; }

        private int SubjectCount { get; }

        private int TryPerSubject { get; }

        private List<Subject> Subjects { get; }

        #endregion

        #region === Methods ===

        public ScResult Run() {
            var sw = new Stopwatch();
            sw.Start();
            while (!IsDone)
                foreach (var subject in Subjects)
                    Action?.Invoke(subject, Room);

            sw.Stop();
            var result = new ScResult(SubjectCount, IsDone, IsSuccessful, sw.ElapsedMilliseconds);
            return result;
        }

        private void FillSubjects() {
            for (var i = 0; i < SubjectCount; i++) {
                var subject = new Subject(i, TryPerSubject);
                Subjects.Add(subject);
            }
        }

        #endregion

        #region === Constructors ===

        public SubjectController() {
            SubjectCount = 100;
            TryPerSubject = 50;
            Room = new Room(SubjectCount);
            Subjects = new List<Subject>();
            FillSubjects();
        }

        public SubjectController(int subjectCount) {
            SubjectCount = subjectCount;
            TryPerSubject = subjectCount / 2;
            Room = new Room(SubjectCount);
            Subjects = new List<Subject>();
            FillSubjects();
        }

        public SubjectController(int subjectCount, Random random) {
            SubjectCount = subjectCount;
            TryPerSubject = subjectCount / 2;
            Room = new Room(SubjectCount, random);
            Subjects = new List<Subject>();
            FillSubjects();
        }

        #endregion
    }
}