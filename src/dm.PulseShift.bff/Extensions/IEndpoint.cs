﻿namespace dm.PulseShift.bff.Extensions;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}