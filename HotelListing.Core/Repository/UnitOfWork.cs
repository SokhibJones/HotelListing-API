using HotelListing.Core.IRepository;
using HotelListing.Data;

namespace HotelListing.Core.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;
        private IGenericRepository<Country> counries;
        private IGenericRepository<Hotel> hotels;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IGenericRepository<Country> Countries => counries ??= new GenericRepository<Country>(dbContext);

        public IGenericRepository<Hotel> Hotels => hotels ??= new GenericRepository<Hotel>(dbContext);

        public void Dispose()
        {
            dbContext.Dispose();
        }

        public async Task SaveAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
