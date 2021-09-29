﻿using System;
using Messages.Abstractions.Queries;

namespace Messages.Accounts
{
    public static class Queries
    {
        public record GetAccountDetails(Guid Id) : Query;

        public record GetAccountsDetailsWithPagination(int Limit, int Offset) : QueryPaging(Limit, Offset);
    }
}