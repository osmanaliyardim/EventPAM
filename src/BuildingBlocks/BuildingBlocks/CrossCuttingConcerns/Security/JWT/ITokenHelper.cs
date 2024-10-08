﻿using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.JWT;

public interface ITokenHelper
{
    AccessToken CreateToken(User user, IList<OperationClaim> operationClaims);

    RefreshToken CreateRefreshToken(User user, string ipAddress);
}
