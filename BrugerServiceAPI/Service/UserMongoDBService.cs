using BrugerServiceAPI.Service;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;


namespace BrugerServiceAPI.Models
{
    public interface IBrugerInterface
    {
        Task<Bruger?> GetBruger(Guid brugerID);
        Task<IEnumerable<Bruger>?> GetBrugerList();
        Task<Guid> AddBruger(Bruger bruger);
        Task<long> UpdateBruger(Bruger bruger);
        Task<long> DeleteBruger(Guid brugerID);
    }
    public class UserMongoDBService : IBrugerInterface
    {
        private readonly ILogger<UserMongoDBService> _logger;
        private readonly IMongoCollection<Bruger> _userCollection;

        public UserMongoDBService(ILogger<UserMongoDBService> logger, MongoDBContext dbContext, IConfiguration configuration)
        {
            var collectionName = configuration["collectionName"];
            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ApplicationException("UserCollectionName is not configured.");
            }

            _logger = logger;
            _userCollection = dbContext.GetCollection<Bruger>(collectionName);  
            _logger.LogInformation($"Collection name: {collectionName}");
        }

        public async Task<Bruger?> GetBruger(Guid bruger_id)
        {
            var filter = Builders<Bruger>.Filter.Eq(x => x.brugerID, bruger_id);
            return await _userCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Bruger>?> GetBrugerList()
        {
            return await _userCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Guid> AddBruger(Bruger bruger)
        {
            bruger.brugerID = Guid.NewGuid();
            await _userCollection.InsertOneAsync(bruger);
            return bruger.brugerID;
        }

        public async Task<long> UpdateBruger(Bruger bruger)
        {
            var filter = Builders<Bruger>.Filter.Eq(x => x.brugerID, bruger.brugerID);
            var result = await _userCollection.ReplaceOneAsync(filter, bruger);
            return result.ModifiedCount;
        }

        public async Task<long> DeleteBruger(Guid bruger_id)
        {
            var filter = Builders<Bruger>.Filter.Eq(x => x.brugerID, bruger_id);
            var result = await _userCollection.DeleteOneAsync(filter);
            return result.DeletedCount;
        }
    }
}
