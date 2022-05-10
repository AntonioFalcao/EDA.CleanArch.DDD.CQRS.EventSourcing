﻿using Contracts.DataTransferObjects;
using Domain.Abstractions.ValueObjects;

namespace Domain.ValueObjects.PaymentOptions.CreditCards;

public record CreditCard(DateOnly Expiration, string Number, string HolderName, string SecurityNumber) : ValueObject<CreditCardValidator>, IPaymentOption
{
    public static implicit operator CreditCard(Dto.CreditCard creditCard)
        => new(creditCard.Expiration, creditCard.Number, creditCard.HolderName, creditCard.SecurityNumber);

    public static implicit operator Dto.CreditCard(CreditCard creditCard)
        => new(creditCard.Expiration, creditCard.Number, creditCard.HolderName, creditCard.SecurityNumber);
}