using FitnessSolution.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessSolution.Controllers
{
    public class ServicebusController : Controller
    {
        const string ServiceBusConnectionString = "Endpoint=sb://fitnesssolutionbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ObecfBtRDSL1AB7ioq1LMCf0SrvaEXP0/DK5v6Dv9Xw="; 
        const string QueueName = "notification";

        public IActionResult Index()
        {
            return View();
        }
        private static async Task CreateQueueFunctionAsync()
        {
            var managementClient = new ManagementClient(ServiceBusConnectionString);
            bool queueExists = await managementClient.QueueExistsAsync(QueueName);
            if (!queueExists)
            {
                QueueDescription qd = new QueueDescription(QueueName)
                {
                    MaxSizeInMB = 1024,
                    MaxDeliveryCount = 3
                };
                await managementClient.CreateQueueAsync(qd);
            }
        }

        [HttpPost] // after fill in the form 
        [ValidateAntiForgeryToken]
        public async Task AddNotification(Notification notification)
        {
            QueueClient queue = new QueueClient(ServiceBusConnectionString, QueueName);
            if (ModelState.IsValid)
            {
                var orderJSON = JsonConvert.SerializeObject(notification);
                var message = new Message(Encoding.UTF8.GetBytes(orderJSON))
                {
                    MessageId = Guid.NewGuid().ToString(),
                    ContentType = "application/json"
                };
                await queue.SendAsync(message);
            }
        }

        public async Task<ActionResult> ReceivedMessage()
        {
            var managementClient = new ManagementClient(ServiceBusConnectionString);
            var queue = await managementClient.GetQueueRuntimeInfoAsync(QueueName);
            System.Diagnostics.Debug.WriteLine(queue.MessageCount);
            ViewBag.MessageCount = queue.MessageCount;
            List<Notification> messages = new List<Notification>();
            List<long> sequence = new List<long>();
            MessageReceiver messageReceiver = new MessageReceiver(ServiceBusConnectionString,QueueName);
            for (int i = 0; i < queue.MessageCount; i++)
            {
                Message message = await messageReceiver.PeekAsync();
                Notification result = JsonConvert.DeserializeObject<Notification>(Encoding.UTF8.GetString(message.Body));
                sequence.Add(message.SystemProperties.SequenceNumber);
                messages.Add(result);
            }
            ViewBag.sequence = sequence;
            ViewBag.messages = messages;
            return View();
        }

        public async Task<ActionResult> Approve(long sequence)
        {
            //connect to the same queue 
            var managementClient = new ManagementClient(ServiceBusConnectionString);
            var queue = await managementClient.GetQueueRuntimeInfoAsync(QueueName);
            //receive the selected message 
            MessageReceiver messageReceiver = new MessageReceiver(ServiceBusConnectionString, QueueName);
            Notification result = null;
            for (int i = 0; i < queue.MessageCount; i++)
            {
                Message message = await messageReceiver.ReceiveAsync();
                string token = message.SystemProperties.LockToken;
                //to find the selected message - read and remove from the queue 
                if (message.SystemProperties.SequenceNumber == sequence)
                {
                    result = JsonConvert.DeserializeObject<Notification>(Encoding.UTF8.GetString(message.Body));
                    await messageReceiver.CompleteAsync(token);
                    break;
                }
            }
            return RedirectToAction("ReceiveMessaged", "Servicebus");
        }

        public static void Initialize()
        {
            CreateQueueFunctionAsync().GetAwaiter().GetResult();
        }

    }
}
