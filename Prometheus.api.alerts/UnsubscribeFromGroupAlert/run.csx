#r "Microsoft.WindowsAzure.Storage"
using Microsoft.WindowsAzure.Storage.Table;
using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, ICollector<AlertGroups> tableBinding, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    string id = req.GetQueryNameValuePairs()
        .First(q => string.Compare(q.Key, "id", true) == 0)
        .Value;

    var item = new AlertGroups(id, id);
    var operation = TableOperation.Delete(item);
    await tableBinding.ExecuteAsync(operation);

    return req.CreateResponse(HttpStatusCode.OK, "Done");
}

public class AlertGroups : TableEntity
{
    public string Name { get; set; }
    public string Phone { get; set; }
}