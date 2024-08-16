namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}