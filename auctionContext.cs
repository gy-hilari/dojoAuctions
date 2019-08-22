using Microsoft.EntityFrameworkCore;

namespace Auctions.Models
{
    public class auctionContext : DbContext
    {
        public auctionContext(DbContextOptions options) : base(options) { }
        public DbSet<User> users { get; set; }
        public DbSet<Login> logins { get; set; }
        public DbSet<Auction> auctions { get; set; }
        public DbSet<Bid> bids { get; set; }
    }
}

