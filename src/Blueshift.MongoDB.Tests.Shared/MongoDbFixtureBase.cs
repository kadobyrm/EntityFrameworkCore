using System;
using System.Diagnostics;
using System.IO;

namespace Blueshift.MongoDB.Tests.Shared
{
    public abstract class MongoDbFixtureBase : IDisposable
    {
        private static readonly bool IsCiBuild;
        private Process _mongodProcess;

        static MongoDbFixtureBase()
        {
            IsCiBuild =
                (Boolean.TryParse(Environment.ExpandEnvironmentVariables("%APPVEYOR%"), out bool isAppVeyor) && isAppVeyor) ||
                (Boolean.TryParse(Environment.ExpandEnvironmentVariables("%TRAVIS%"), out bool isTravisCI) && isTravisCI);
        }

        public MongoDbFixtureBase()
        {
            if (!IsCiBuild)
            {
                Directory.CreateDirectory(MongoDbConstants.DataFolder);
                _mongodProcess = Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = MongoDbConstants.MongodExe,
                        Arguments = $@"-vvvvv --port {MongoDbConstants.MongodPort} --logpath "".data\{MongoDbConstants.MongodPort}.log"" --dbpath ""{MongoDbConstants.DataFolder}""",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
            }
        }

        public void Dispose()
        {
            if (!IsCiBuild && _mongodProcess != null && !_mongodProcess.HasExited)
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = MongoDbConstants.MongoExe,
                        Arguments = $@"""{MongoDbConstants.MongoUrl}/admin"" --eval ""db.shutdownServer();""",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                _mongodProcess.WaitForExit(milliseconds: 5000);
                _mongodProcess.Dispose();
                _mongodProcess = null;
            }
        }
    }
}
