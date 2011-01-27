using System;

namespace ThreatAwarePathfinder {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args) {
            using (AbstractDemo game = new AbstractDemo()) {
                game.Run();
            }
        }
    }
}

