using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Auctions.Models;
using System.Linq;
using System.Web;

namespace Auctions.Controllers
{
    public class auctionController : Controller
    {

        private auctionContext auctionContext;

        public auctionController(auctionContext context)
        {
            auctionContext = context;
        }

        // // // 

        [HttpGet("")]
        public IActionResult Index()
        {
            User user = auctionContext.users.FirstOrDefault(u => u.userId == HttpContext.Session.GetInt32("UserSession"));
            if (user != null)
            {
                return RedirectToAction("Auctions");
            }
            return View("Index");
        }

        [HttpPost("register")]
        public IActionResult Register(LoginRegModel loginRegModel)
        {
            User userForm = loginRegModel.newUser;
            if (ModelState.IsValid)
            {
                if (auctionContext.users.Any(g => g.userName == userForm.userName))
                {
                    ModelState.AddModelError("RegisterEmail", "Username already in use!");
                    return View("Index");
                }
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                userForm.password = hasher.HashPassword(userForm, userForm.password);

                auctionContext.Add(userForm);
                auctionContext.SaveChanges();
                var newuser = auctionContext.users.FirstOrDefault(g => g.userName == userForm.userName);
                HttpContext.Session.SetInt32("UserSession", newuser.userId);
                return RedirectToAction("Auctions");
            }
            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRegModel loginRegModel)
        {
            Login loginForm = loginRegModel.newLogin;
            if (ModelState.IsValid)
            {
                var loginViaDb = auctionContext.users.FirstOrDefault(g => g.userName == loginForm.username);
                if (loginViaDb == null)
                {
                    // ModelState.AddModelError("Login", "Invalid Email or Password");
                    ModelState.AddModelError("Login", "Invalid Email");
                    return View("Index");
                }

                var hasher = new PasswordHasher<Login>();
                var result = hasher.VerifyHashedPassword(loginForm, loginViaDb.password, loginForm.password);
                if (result == 0)
                {
                    ModelState.AddModelError("Login", "Invalid Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserSession", loginViaDb.userId);
                return RedirectToAction("Auctions");
            }
            return View("Index");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet("auctions")]
        public IActionResult Auctions()
        {
            User user = auctionContext.users.FirstOrDefault(u => u.userId == HttpContext.Session.GetInt32("UserSession"));
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            List<Auction> allAuctions = auctionContext.auctions.OrderBy(a => a.endDate).Include(a => a.user).Include(a => a.bids).ToList();
            List<Auction> endedAuctions = auctionContext.auctions.Include(a => a.user).Include(a => a.bids).ThenInclude(b=> b.user).Where(a => a.endDate < DateTime.Now).ToList();
            List<Auction> currentAuctions = allAuctions.Except(endedAuctions).ToList();
            List<Auction> endedUserAuctions = endedAuctions.Where(a => a.user == user).ToList();

            foreach (var auction in endedUserAuctions)
            {
                // Console.WriteLine(new String('%', 80));

                float max = 0;
                Bid highestBid = new Bid();
                foreach (var bid in auction.bids)
                {
                    if (bid.amount > max)
                    {
                        max = bid.amount;
                        highestBid = bid;
                    }
                }
                if (highestBid.user == user)
                {
                    user.wallet -= highestBid.amount;
                    auction.user.wallet += highestBid.amount;

                }

                // Console.WriteLine(highestBid.user.userName);

                // Console.WriteLine(new String('%', 80));
            }

            ViewBag.User = user;

            return View("Auctions", currentAuctions);
        }

        // /////////////////////////////////////////////////////////////////////////

        //                  AUCTION SECTION START

        // /////////////////////////////////////////////////////////////////////////

        [HttpGet("auction/new")]
        public IActionResult NewAuction()
        {
            User user = auctionContext.users.FirstOrDefault(u => u.userId == HttpContext.Session.GetInt32("UserSession"));
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            return View("NewAuction");
        }

        [HttpPost("auction")]
        public IActionResult PostAuction(Auction auctionForm)
        {
            User user = auctionContext.users.FirstOrDefault(u => u.userId == HttpContext.Session.GetInt32("UserSession"));
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                if (auctionForm.endDate < DateTime.Today)
                {
                    ModelState.AddModelError("Date", "End Date must be a date in the future!");
                    return View("NewAuction");
                }
                if (auctionForm.startingBid <= 0)
                {
                    ModelState.AddModelError("Bid", "Starting Bid must be greater than 0!");
                    return View("NewAuction");
                }

                auctionForm.userId = user.userId;
                auctionForm.user = user;

                auctionContext.Add(auctionForm);

                // Auction newAuction = auctionContext.auctions.Include(a=> a.bids).FirstOrDefault(a=> a.name == auctionForm.name && a.user == user && a.endDate == auctionForm.endDate);

                // Bid startingbid = new Bid()
                // {
                //     amount = auctionForm.startingBid,
                //     user = user
                // };

                // newAuction.bids.Add(startingbid);
                auctionContext.SaveChanges();
                return RedirectToAction("Auctions");
            }
            return View("NewAuction");
        }

        [HttpGet("auction/{id}")]
        public IActionResult Auction(int id)
        {
            User user = auctionContext.users.FirstOrDefault(u => u.userId == HttpContext.Session.GetInt32("UserSession"));
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            Auction auction = auctionContext.auctions.Include(a => a.user).Include(a => a.bids).ThenInclude(b=> b.user).FirstOrDefault(a => a.auctionId == id);

            Console.WriteLine(new String('%', 50));
            Console.WriteLine(auction.bids.Count);
            Console.WriteLine(new String('%', 50));

            if (auction.bids.Count > 0)
            {
                float max = 0;
                Bid highestBid = new Bid();

                for (int i = 0; i < auction.bids.Count(); i++)
                {
                    if (auction.bids[i].amount > max)
                    {
                        max = auction.bids[i].amount;
                        highestBid = auction.bids[i];
                    }
                }
                Console.WriteLine(new String('%', 50));
                Console.WriteLine($"Highest Bidder: {highestBid.user.userName}");
                Console.WriteLine(new String('%', 50));
                ViewBag.Auction = auction;
                ViewBag.HighestBid = max;
                ViewBag.HighestBidder = highestBid;
                return View("Auction");
            }
            else
            {
                ViewBag.Auction = auction;
                ViewBag.HighestBid = 0;
                ViewBag.HighestBidder = null;
                return View("Auction");
            }
        }

        [HttpGet("auction/{id}/delete")]
        public IActionResult DeleteAuction(int id)
        {
            User user = auctionContext.users.FirstOrDefault(u => u.userId == HttpContext.Session.GetInt32("UserSession"));
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            Auction auction = auctionContext.auctions.Include(a => a.user).Include(a => a.bids).FirstOrDefault(a => a.auctionId == id);

            if (auction.user == user)
            {
                auctionContext.auctions.Remove(auction);
                auctionContext.SaveChanges();
            }

            return RedirectToAction("Auctions");
        }

        [HttpPost("auction/{id}/bid")]
        public IActionResult PostBid(int id, Bid bidForm)
        {
            User user = auctionContext.users.FirstOrDefault(u => u.userId == HttpContext.Session.GetInt32("UserSession"));
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            Auction auction = auctionContext.auctions.Include(a => a.user).Include(a => a.bids).FirstOrDefault(a => a.auctionId == id);
            // Console.WriteLine(new String('%', 50));
            // Console.WriteLine(HttpContext.Request.Method);
            // Console.WriteLine(new String('%', 50));

            float max = 0;
            Bid highestBid = new Bid();

            for (int i = 0; i < auction.bids.Count(); i++)
            {
                if (auction.bids[i].amount > max)
                {
                    max = auction.bids[i].amount;
                    highestBid = auction.bids[i];
                }
            }


            if (ModelState.IsValid)
            {
                if (auction.user == user)
                {
                    ModelState.AddModelError("User", "You cannot bid on your own auction!");
                    ViewBag.Auction = auction;
                    ViewBag.HighestBidder = null;
                    ViewBag.HighestBid = max;
                    return View("Auction");
                }
                if (bidForm.amount < max || bidForm.amount < auction.startingBid)
                {
                    ModelState.AddModelError("Amount", "Bid amount must be higher than current highest bid!");
                    ViewBag.Auction = auction;
                    ViewBag.HighestBidder = null;
                    ViewBag.HighestBid = max;
                    return View("Auction");
                }
                if (bidForm.amount > user.wallet)
                {
                    ModelState.AddModelError("Wallet", "You do not have enough money to make that bid!");
                    ViewBag.Auction = auction;
                    ViewBag.HighestBidder = null;
                    ViewBag.HighestBid = max;
                    return View("Auction");
                }

                // user.wallet -= bidForm.amount;
                // auction.user.wallet += bidForm.amount;

                bidForm.userId = user.userId;
                bidForm.user = user;
                bidForm.auctionId = auction.auctionId;
                bidForm.auction = auction;

                auctionContext.Add(bidForm);
                Bid newBid = auction.bids.FirstOrDefault(b => b.auctionId == id && b.userId == user.userId && b.amount == bidForm.amount);

                user.bids.Add(bidForm);
                auction.bids.Add(bidForm);
                auctionContext.SaveChanges();
                return RedirectToAction("Auction", id);
            }
            ViewBag.Auction = auction;
            ViewBag.HighestBid = max;
            ViewBag.HighestBidder = null;
            return View("Auction");
        }

    }
}

