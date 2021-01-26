using System;

namespace Simulation {
    public class Subject {
        #region === Constructors ===

        public Subject(int number, int tries) {
            Number = number;
            TriesLeft = tries;
        }

        #endregion

        #region === Methods ===

        public bool TryFindNumber(Room room, int drawerNumber) {
            if (TriesLeft == 0)
                throw new Exception("No more tries left.");
            TriesLeft--;

            if (room.Drawers[drawerNumber] == Number) {
                IsDone = IsSuccessful = true;
                return true;
            }

            if (TriesLeft == 0)
                IsDone = true;
            return false;
        }

        #endregion

        #region === Properties ===

        public int Number { get; }
        private int TriesLeft { get; set; }
        public bool IsDone { get; private set; }
        public bool IsSuccessful { get; private set; }

        #endregion
    }
}