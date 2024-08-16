﻿namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

public class AggregateNotFoundException : System.Exception
{
    public AggregateNotFoundException(string typeName, Guid id) : base($"{typeName} with id '{id}' was not found")
    {

    }

    public static AggregateNotFoundException For<T>(Guid id)
    {
        return new AggregateNotFoundException(typeof(T).Name, id);
    }
}
