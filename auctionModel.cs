using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auctions.Models
{
    public class User
    {
        [Key]
        public int userId { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string userName { get; set; }

        [Required]
        [MinLength(2)]
        public string firstName { get; set; }

        [Required]
        [MinLength(2)]
        public string lastName { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Minimum password length is 8 characters")]
        // [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$")]
        public string password { get; set; }

        [NotMapped]
        [Compare("password", ErrorMessage = "Your passwords don't match!")]
        public string confirmPassword { get; set; }

        // // //
        public float wallet { get; set; } = 1000;
        public List<Auction> auctions { get; set; } = new List<Auction>();
        public List<Bid> bids { get; set; } = new List<Bid>();
        // // //
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;
    }

    public class Auction
    {
        [Key]
        public int auctionId { get; set; }

        [Required]
        [MinLength(3)]
        public string name { get; set; }

        [Required]
        [MinLength(10)]
        public string description { get; set; }

        [Required]
        public float startingBid { get; set; }

        [Required]
        public DateTime endDate { get; set; }

        public int userId { get; set; }
        public User user { get; set; }

        // // //
        public List<Bid> bids { get; set; } = new List<Bid>();

        // // //
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;

    }

    //

    public class Bid
    {
        [Key]
        public int bidId { get; set; }

        [Required]
        public float amount { get; set; }

        public int auctionId { get; set; }
        public Auction auction { get; set; }

        public int userId { get; set; }
        public User user { get; set; }
    }



    public class Login
    {
        [Key]
        public int loginId { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }
    }

    public class LoginRegModel
    {
        public User newUser { get; set; }
        public Login newLogin { get; set; }
    }

}

