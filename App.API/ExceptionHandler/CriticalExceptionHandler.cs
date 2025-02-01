using Microsoft.AspNetCore.Diagnostics;

namespace App.API.ExceptionHandler
{
    public class CriticalExceptionHandler(): IExceptionHandler
    {
        public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {

            //busines logic(spesific error)
            //if (exception is CriticalException)
            //{
            //    Console.WriteLine("Hata ile ilgili sms gönderildi");
            //}
            return ValueTask.FromResult(false);  //true dönersek global exceptiona gitmez.
        }
    }
}
