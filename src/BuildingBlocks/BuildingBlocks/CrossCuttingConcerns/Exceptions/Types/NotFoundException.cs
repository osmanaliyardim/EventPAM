﻿namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}