using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Consts
{
    public class ServiceBusConsts
    {
        //<app.<eventname>.<queue-name>
        public const string ProductAddedEventQueueName= "clean.app.productadded.event.queue";
    }
}
