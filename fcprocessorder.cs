using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace functionchaining
{
    public static class fcprocessorder
    {
        [FunctionName("fcprocessorder_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter, ILogger log)
        {
            string instanceId = await starter.StartNewAsync("fcprocessorder", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("fcprocessorder")]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            bool productAvailable = await context.CallActivityAsync<bool>("CheckProductAvailability", "Mi Band 4, Optical Mouse - red");
            if(productAvailable)
                outputs.Add("Products are available");
            double totalAmount = await context.CallActivityAsync<double>("CreateInvoice", productAvailable);
            outputs.Add(string.Format("Total Invoice Amount {0}", totalAmount));
            await context.CallActivityAsync("ShipProducts", totalAmount);
            outputs.Add("Products are shipped.");
            return outputs;
        }

        [FunctionName("CheckProductAvailability")]
        public static bool ProductAvailability([ActivityTrigger] string productList, ILogger log)
        {
            log.LogInformation($"Product List - {productList}.");
            bool productsAvailable = true;
            return productsAvailable;
        }

        [FunctionName("CreateInvoice")]
        public static double CreateInvoice([ActivityTrigger] bool productAvailable, ILogger log)
        {
            if (productAvailable)
            {
                double invoiceAmount = 20000;
                log.LogInformation("Products are available");
                return invoiceAmount;
            }
            else
                return 0;
        }

        [FunctionName("ShipProducts")]
        public static void ShipProducts([ActivityTrigger] double totalAmount, ILogger log)
        {
            log.LogInformation($"Invoice Amount {totalAmount}.");
        }


    }
}