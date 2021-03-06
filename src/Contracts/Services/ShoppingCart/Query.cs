using Contracts.Abstractions.Messages;

namespace Contracts.Services.ShoppingCart;

public static class Query
{
    public record GetShoppingCart(Guid CartId) : Message(CorrelationId: CartId), IQuery;

    public record GetCustomerShoppingCart(Guid CustomerId) : Message(CorrelationId: CustomerId), IQuery;

    public record GetShoppingCartItem(Guid CartId, Guid ItemId) : Message(CorrelationId: CartId), IQuery;

    public record GetShoppingCartItems(Guid CartId, ushort Limit, ushort Offset) : Message(CorrelationId: CartId), IQuery;

    public record GetCartPaymentMethods(Guid CartId, ushort Limit, ushort Offset) : Message(CorrelationId: CartId), IQuery;

    public record GetCartPaymentMethod(Guid CartId, Guid PaymentMethodId) : Message(CorrelationId: CartId), IQuery;
}