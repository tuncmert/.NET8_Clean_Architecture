using App.Domain.Events;

namespace App.Application.Contracts.ServiceBus
{
    public interface IServiceBus
    {
        //T için her interface ait class gönderilmemesi için IEventOrMessage oluşturup kısınlandı
        Task PublishAsync<T>(T message, CancellationToken cancellation = default) where T : IEventOrMessage;

        Task SendAsync<T>(T message,string queueName, CancellationToken cancellation = default) where T : IEventOrMessage;
    }
}
