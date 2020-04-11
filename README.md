# Function chaining pattern in durable function 
This sample code has used function chaining pattern in durable function framework. The code is developed on .NET Core 3.1 and function v3 in Visual Studio 2019

In function chaining, sequence of functions are executed in a given order. The output of one function is going to be the input for the next function. We can use Durable Functions to implement the function chaining pattern.

## Installed Packages
Microsoft.NET.Sdk.Functions version 3 (3.0.5) and Microsoft.Azure.WebJobs.Extensions.DurableTask 2 (2.2.0)

## Code snippets
### Http trigger function
This is a http trigger function and the entry point for the application
```
        [FunctionName("fcprocessorder_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get","post")]HttpRequestMessage req, 
          [DurableClient] IDurableOrchestrationClient starter, ILogger log)
        {
            string instanceId = await starter.StartNewAsync("fcprocessorder", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
```
