using Chromely.Core;
using Chromely.Core.Configuration;

namespace ModularProject.Chromely {
    class Program {
        static void Main(string[] args) {
            // basic example of the application builder
            AppBuilder
            .Create()
            .UseApp<MyChromelyApp>()
            .UseConfiguration<IChromelyConfiguration>(new MyChromelyConfiguration())
            .Build()
            .Run(args);
        }
    }
}
