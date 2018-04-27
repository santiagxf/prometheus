#r "Twilio.Api"
#r "Microsoft.WindowsAzure.Storage"

using Microsoft.WindowsAzure.Storage.Table;

using System.Net;
using Twilio;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, IQueryable<AlertGroups> tableBinding, TraceWriter log)
{
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");    
 
    string accountSid = GetEnvironmentVariable("TwilioAccountSid");
    string authToken = GetEnvironmentVariable("TwilioAuthToken");
    string twilioVerifiedNumber = GetEnvironmentVariable("TwilioPhoneNumber");

    string partitionKey = GetEnvironmentVariable("AzureWebJobsAlertGroupsPartition");

    string googleMapsUrl = @"http://maps.google.com/maps?q={0}";
 
    var client = new TwilioRestClient(accountSid, authToken);
    string fireLocation = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "location", true) == 0)
        .Value;
    
    foreach (var person in tableBinding.Where(p => p.PartitionKey == partitionKey).ToList())
    {
         var result = client.SendMessage(
            twilioVerifiedNumber, // Insert your Twilio from SMS number here
            person.Phone, // Insert your verified (trial) to SMS number here
            "PrometheusAlert: A fire has been confirmed at " + string.Format(googleMapsUrl, fireLocation)         
        );
    
        if (result == null || result.RestException != null)
        {
            //an exception occurred making the REST call
            req.CreateResponse(HttpStatusCode.OK, result.RestException.Message);
        }
    }
    return req.CreateResponse(HttpStatusCode.OK, "Message sent");
}

public class AlertGroups : TableEntity
{
    public string Name { get; set; }
    public string Phone { get; set; }
}