using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DurableFuncsWithTimer
{
    public static class TimerTriggeredFunc
    {

        [FunctionName(nameof(SendNotification))]
        public static void SendNotification([ActivityTrigger] DateTime datetime, ILogger log)
        {
            Console.WriteLine($"Event triggered at {datetime}.");
            Thread.Sleep(120000); //do nothing for 2 mins

        }

        [FunctionName(nameof(ProcessOutagesOrchestrator))]
        public static async Task ProcessOutagesOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            await context.CallActivityAsync("SendNotification", DateTime.Now);
        }

        [FunctionName("TimerTrigger")]
        public static async Task Run(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("ProcessOutagesOrchestrator", null);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        }
    }
}