using Docker.DotNet;

namespace ewu.dockerstats;

internal class Program
{
    private static async Task Main(string[] args)
    {
        DockerClient client = new DockerClientConfiguration().CreateClient();

        DockerProjectMonitor monitor = new DockerProjectMonitor(client);

        while (true)
        {
            string info = await monitor.GetProjectInfo();
            Console.Clear();
            Console.WriteLine(info);
            await Task.Delay(1000);
        }
    }
}