using EventPAM.BuildingBlocks.Mongo;
using EventPAM.Customer.Customers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EventPAM.Customer.Data;

public class CustomerReadDbContext : MongoDbContext
{
    public CustomerReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Customers = GetCollection<CustomerReadModel>("customerreadmodel");
    }

    public IMongoCollection<CustomerReadModel> Customers { get; }
}
