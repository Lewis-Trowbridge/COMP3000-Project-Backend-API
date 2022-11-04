using EphemeralMongo;
using MongoDB.Driver;
using System.Diagnostics;

namespace COMP3000_Project_Backend_API.IntegrationTests.Support
{
    public class MongoDBFixture : IDisposable
    {
        private bool disposedValue;
        public IMongoRunner runner;
        public MongoClient mongoClient;

        public MongoDBFixture()
        {
            var options = new MongoRunnerOptions()
            {
                StandardOuputLogger = line => Debug.WriteLine(line),
                StandardErrorLogger = line => Debug.WriteLine(line),
            };
            runner = MongoRunner.Run(options);
            mongoClient = new MongoClient(runner.ConnectionString);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    runner.Dispose();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
