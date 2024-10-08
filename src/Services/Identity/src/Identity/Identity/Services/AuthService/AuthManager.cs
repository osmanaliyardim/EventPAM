﻿using EventPAM.BuildingBlocks;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.JWT;
using EventPAM.Identity.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using User = EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities.User;

namespace EventPAM.Identity.Identity.Services.AuthService;

public class AuthManager : IAuthService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenHelper _tokenHelper;
    private readonly TokenOptions? _tokenOptions;
    private readonly IUserOperationClaimRepository _userOperationClaimRepository;

    public AuthManager(
        IUserOperationClaimRepository userOperationClaimRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenHelper tokenHelper,
        IConfiguration configuration
    )
    {
        _userOperationClaimRepository = userOperationClaimRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenHelper = tokenHelper;
        _tokenOptions = configuration.GetSection(EventPAMBase.Configs.TOKEN_OPTIONS).Get<TokenOptions>();
    }

    public async Task<AccessToken> CreateAccessToken(User user)
    {
        var operationClaims = await _userOperationClaimRepository
            .Query()
            .AsNoTracking()
            .Where(p => p.UserId == user.Id)
            .Select(p => new OperationClaim { Id = p.OperationClaimId, Name = p.OperationClaim.Name })
            .ToListAsync();

        var accessToken = _tokenHelper.CreateToken(user, operationClaims);

        return accessToken;
    }

    public async Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken)
    {
        var addedRefreshToken = await _refreshTokenRepository.AddAsync(refreshToken);

        return addedRefreshToken;
    }

    public async Task DeleteOldRefreshTokens(Guid userId)
    {
        var refreshTokens = await _refreshTokenRepository
            .Query()
            .AsNoTracking()
            .Where(
                r =>
                    r.UserId == userId
                    && r.Revoked == null
                    && r.Expires >= DateTime.UtcNow
                    && r.Created.AddDays(_tokenOptions!.RefreshTokenTTL) <= DateTime.UtcNow
            )
            .ToListAsync();

        await _refreshTokenRepository.DeleteRangeAsync(refreshTokens);
    }

    public async Task<RefreshToken?> GetRefreshTokenByToken(string token)
    {
        var refreshToken = await _refreshTokenRepository.GetAsync(r => r.Token == token);

        return refreshToken;
    }

    public async Task RevokeRefreshToken(RefreshToken refreshToken, string ipAddress, string? reason = null, string? replacedByToken = null)
    {
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReasonRevoked = reason;
        refreshToken.ReplacedByToken = replacedByToken;
        await _refreshTokenRepository.UpdateAsync(refreshToken);
    }

    public async Task<RefreshToken> RotateRefreshToken(User user, RefreshToken refreshToken, string ipAddress)
    {
        var newRefreshToken = _tokenHelper.CreateRefreshToken(user, ipAddress);
        await RevokeRefreshToken(refreshToken, ipAddress, reason: "Replaced by new token", newRefreshToken.Token);

        return newRefreshToken;
    }

    public async Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason)
    {
        var childToken = await _refreshTokenRepository.GetAsync(r => r.Token == refreshToken.ReplacedByToken);

        if (childToken is not null && childToken.Revoked is not null && childToken.Expires <= DateTime.UtcNow)
            await RevokeRefreshToken(childToken, ipAddress, reason);
        else
            await RevokeDescendantRefreshTokens(childToken!, ipAddress, reason);
    }

    public Task<RefreshToken> CreateRefreshToken(User user, string ipAddress)
    {
        var refreshToken = _tokenHelper.CreateRefreshToken(user, ipAddress);

        return Task.FromResult(refreshToken);
    }
}
