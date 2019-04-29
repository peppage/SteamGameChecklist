﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SteamGameChecklist.Web.Db.Contexts;

namespace SteamGameChecklist.Web.Migrations
{
    [DbContext(typeof(SteamGameChecklistContext))]
    [Migration("20190429195539_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("SteamGameChecklist.Web.Db.Models.Game", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Hidden");

                    b.Property<string>("Image");

                    b.Property<string>("Name");

                    b.Property<int>("Playtime2Weeks");

                    b.Property<int>("PlaytimeForever");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });
#pragma warning restore 612, 618
        }
    }
}
