using System.CodeDom.Compiler;

using Docker.DotNet;
using Docker.DotNet.Models;

namespace ewu.dockerstats;

internal class DockerProjectMonitor
{
    private readonly DockerClient m_Client;

    public DockerProjectMonitor(DockerClient client)
    {
        m_Client = client;
    }

    public async Task<string> GetProjectInfo()
    {
        IList<ContainerListResponse>? containers = await m_Client.Containers.ListContainersAsync(
            new ContainersListParameters
            {
                All = true,
            }
        );

        IEnumerable<IGrouping<string, ContainerListResponse>> projects = containers
            .Where(x => x.Labels.ContainsKey("com.docker.compose.project"))
            .GroupBy(x => x.Labels["com.docker.compose.project"]);

        StringWriter sw = new StringWriter();
        IndentedTextWriter tw = new IndentedTextWriter(sw);

        foreach (IGrouping<string, ContainerListResponse> project in projects)
        {
            await tw.WriteLineAsync(project.Key);
            tw.Indent++;
            foreach (ContainerListResponse container in project)
            {
                await tw.WriteLineAsync(container.Names.First() + " " + container.State + " " + " " + container.Status);
                tw.Indent++;
                foreach (KeyValuePair<string, string> label in container.Labels)
                {
                    await tw.WriteLineAsync(label.Key + ": " + label.Value);
                }

                tw.Indent--;
            }

            tw.Indent--;
        }

        await tw.FlushAsync();

        return sw.ToString();
    }
}