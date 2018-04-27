#r "Microsoft.WindowsAzure.Storage"
using Microsoft.WindowsAzure.Storage.Table;
using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, ICollector<AlertGroups> tableBinding, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    string name = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
        .Value;

    string phone = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "phone", true) == 0)
        .Value;
    
    string partition =  GetEnvironmentVariable("AzureWebJobsAlertGroupsPartition");

    if (!string.IsNullOrEmpty(name) && string.IsNullOrEmpty(phone))
        tableBinding.Add(
                    new AlertGroups() {
                        PartitionKey = partition,
                        RowKey = phone,
                        Name = name,
                        Phone = phone }
                    );
    else
        return req.CreateResponse(HttpStatusCode.OK, "No record was changed");
    return req.CreateResponse(HttpStatusCode.OK, "Message sent");
}

public class AlertGroups : TableEntity
{
    public string Name { get; set; }
    public string Phone { get; set; }
}