using Application.Abstractions.UseCases;
using Contracts.Abstractions.Messages;
using MassTransit;

namespace Infrastructure.MessageBus.Abstractions;

public abstract class Consumer<TMessage> : IConsumer<TMessage>
    where TMessage : class, IMessage
{
    private readonly IInteractor<TMessage> _interactor;

    protected Consumer(IInteractor<TMessage> interactor)
    {
        _interactor = interactor;
    }
    
    public Task Consume(ConsumeContext<TMessage> context)
        => _interactor.InteractAsync(context.Message, context.CancellationToken);
}