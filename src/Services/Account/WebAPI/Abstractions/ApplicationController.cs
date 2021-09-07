using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Abstractions
{
    [ApiController, Route("api/[controller]/[action]")]
    public abstract class ApplicationController : ControllerBase
    {
        private readonly IBus _bus;

        protected ApplicationController(IBus bus)
        {
            _bus = bus;
        }

        protected async Task<IActionResult> SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        {
            await SendMessage(command, cancellationToken);
            return Accepted();
        }

        protected async Task<IActionResult> GetQueryResponseAsync<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken)
            where TQuery : class
            where TResponse : class
        {
            var response = await GetResponseAsync<TQuery, TResponse>(query, cancellationToken);
            return Ok(response.Message);
        }

        private Task<Response<TResponse>> GetResponseAsync<TMessage, TResponse>(TMessage message, CancellationToken cancellationToken)
            where TMessage : class
            where TResponse : class 
            => _bus.CreateRequestClient<TMessage>().GetResponse<TResponse>(message, cancellationToken);

        private Task SendMessage<TMessage>(TMessage message, CancellationToken cancellationToken)
            => _bus.Send(message, cancellationToken);
    }
}