﻿// <auto-generated />
using System;
using Auctions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Auctions.Migrations
{
    [DbContext(typeof(auctionContext))]
    [Migration("20190821195247_migration4")]
    partial class migration4
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Auctions.Models.Auction", b =>
                {
                    b.Property<int>("auctionId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("createdAt");

                    b.Property<string>("description")
                        .IsRequired();

                    b.Property<DateTime>("endDate");

                    b.Property<string>("name")
                        .IsRequired();

                    b.Property<float>("startingBid");

                    b.Property<DateTime>("updatedAt");

                    b.Property<int>("userId");

                    b.HasKey("auctionId");

                    b.HasIndex("userId");

                    b.ToTable("auctions");
                });

            modelBuilder.Entity("Auctions.Models.Bid", b =>
                {
                    b.Property<int>("bidId")
                        .ValueGeneratedOnAdd();

                    b.Property<float>("amount");

                    b.Property<int>("auctionId");

                    b.Property<int>("userId");

                    b.HasKey("bidId");

                    b.HasIndex("auctionId");

                    b.HasIndex("userId");

                    b.ToTable("bids");
                });

            modelBuilder.Entity("Auctions.Models.Login", b =>
                {
                    b.Property<int>("loginId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("password")
                        .IsRequired();

                    b.Property<string>("username")
                        .IsRequired();

                    b.HasKey("loginId");

                    b.ToTable("logins");
                });

            modelBuilder.Entity("Auctions.Models.User", b =>
                {
                    b.Property<int>("userId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("createdAt");

                    b.Property<string>("firstName")
                        .IsRequired();

                    b.Property<string>("lastName")
                        .IsRequired();

                    b.Property<string>("password")
                        .IsRequired();

                    b.Property<DateTime>("updatedAt");

                    b.Property<string>("userName")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<float>("wallet");

                    b.HasKey("userId");

                    b.ToTable("users");
                });

            modelBuilder.Entity("Auctions.Models.Auction", b =>
                {
                    b.HasOne("Auctions.Models.User", "user")
                        .WithMany("auctions")
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Auctions.Models.Bid", b =>
                {
                    b.HasOne("Auctions.Models.Auction", "auction")
                        .WithMany("bids")
                        .HasForeignKey("auctionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Auctions.Models.User", "user")
                        .WithMany("bids")
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
