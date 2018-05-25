#r "Microsoft.WindowsAzure.Storage"
using Microsoft.WindowsAzure.Storage.Table;
using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, CloudTable tableBinding, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    string id = req.GetQueryNameValuePairs()
        .First(q => string.Compare(q.Key, "id", true) == 0)
        .Value;

    string partition  = req.GetQueryNameValuePairs()
        .First(q => string.Compare(q.Key, "group", true) == 0)
        .Value;

    log.Info("Removing entity with id " + id);

    var item = new TableEntity(id, id) { ETag = "*", PartitionKey = partition };
    var operation = TableOperation.Delete(item);
    await tableBinding.ExecuteAsync(operation);

    return req.CreateResponse(HttpStatusCode.OK, "Done");
}