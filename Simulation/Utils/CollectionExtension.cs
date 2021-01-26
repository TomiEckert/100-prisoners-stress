using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Simulation.Utils {
    public static class CollectionExtension {
        private static readonly Random Rng = new Random();

        public static void Shuffle<T>(this List<T> list) {
            var n = list.Count;
            while (n > 1) {
                n--;
                var k = Rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Shuffle<T>(this List<T> list, Random random) {
            var n = list.Count;
            while (n > 1) {
                n--;
                var k = random.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Shuffle<T>(this T[] array) {
            var n = array.Length;
            while (n > 1) {
                n--;
                var k = Rng.Next(n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }

        public static void Shuffle<T>(this T[] array, Random random) {
            var n = array.Length;
            while (n > 1) {
                n--;
                var k = random.Next(n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }

        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> collection) {
            var list = collection.ToList();
            return new ReadOnlyCollection<T>(list);
        }

        public static void Add<T>(this List<T> list, params T[] items) {
            list.AddRange(items);
        }
    }
}