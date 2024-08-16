﻿namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.OtpAuthenticator;

public interface IOtpAuthenticatorHelper
{
    public Task<byte[]> GenerateSecretKey();

    public Task<string> ConvertSecretKeyToString(byte[] secretKey);

    public Task<bool> VerifyCode(byte[] secretKey, string code);
}
