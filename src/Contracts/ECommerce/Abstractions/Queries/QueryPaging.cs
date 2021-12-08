﻿using ECommerce.Abstractions.Queries.Paging;
using MassTransit.Topology;

namespace ECommerce.Abstractions.Queries;

[ExcludeFromTopology]
public record QueryPaging(int Limit, int Offset) : Query, IPaging;