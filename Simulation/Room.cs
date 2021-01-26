using System;
using Simulation.Utils;

namespace Simulation {
    public class Room {
        #region === Methods ===

        private void FillDrawers(int maxIterations) {
            var number = Drawers.Length;

            for (var i = 0; i < number; i++) Drawers[i] = i;

            for (var i = 0; i < _random.Next(maxIterations); i++) Drawers.Shuffle(_random);
        }

        #endregion

        #region === Constructors ===

        public Room(int number) {
            _random = new Random();
            Drawers = new int[number];
            FillDrawers(5);
        }

        public Room(int number, Random random) {
            _random = random;
            Drawers = new int[number];
            FillDrawers(100);
        }

        #endregion

        #region === Properties ===

        private readonly Random _random;

        public int[] Drawers { get; }

        #endregion
    }
}