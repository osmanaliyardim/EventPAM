﻿using EventPAM.Ticketing.GrpcClient.Protos;

namespace EventPAM.IntegrationTest.Ticketing.Fakes;

public static class FakeGetAvailableSeatsResponse
{
    public static GetAvailableSeatsResult Generate()
    {
        var result = new GetAvailableSeatsResult();
        result.SeatDtos.AddRange(new List<SeatDtoResponse>
        {
            new SeatDtoResponse()
            {
                EventId = new Guid("3c5c0000-97c6-fc34-2eb9-08db322230c9").ToString(),
                Class = SeatClass.Standard,
                SeatNumber = "2F",
                Id = NewId.NextGuid().ToString()
            },
            new SeatDtoResponse()
            {
                EventId = new Guid("3c5c0000-97c6-fc34-2eb9-08db322230c9").ToString(),
                Class = SeatClass.Premium,
                SeatNumber = "3D",
                Id = NewId.NextGuid().ToString()
            }
        });

        return result;
    }
}
