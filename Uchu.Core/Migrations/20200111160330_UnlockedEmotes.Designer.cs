﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Uchu.Core;

namespace Uchu.Core.Migrations
{
    [DbContext(typeof(UchuContext))]
    [Migration("20200111160330_UnlockedEmotes")]
    partial class UnlockedEmotes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Uchu.Core.Character", b =>
                {
                    b.Property<long>("CharacterId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BaseHealth");

                    b.Property<int>("BaseImagination");

                    b.Property<long>("Currency");

                    b.Property<int>("CurrentArmor");

                    b.Property<int>("CurrentHealth");

                    b.Property<int>("CurrentImagination");

                    b.Property<string>("CustomName")
                        .IsRequired()
                        .HasMaxLength(33);

                    b.Property<long>("EyeStyle");

                    b.Property<long>("EyebrowStyle");

                    b.Property<bool>("FreeToPlay");

                    b.Property<long>("HairColor");

                    b.Property<long>("HairStyle");

                    b.Property<bool>("LandingByRocket");

                    b.Property<long>("LastActivity");

                    b.Property<long>("LastClone");

                    b.Property<int>("LastInstance");

                    b.Property<int>("LastZone");

                    b.Property<int>("LaunchedRocketFrom");

                    b.Property<long>("Level");

                    b.Property<long>("Lh");

                    b.Property<int>("MaximumArmor");

                    b.Property<int>("MaximumHealth");

                    b.Property<int>("MaximumImagination");

                    b.Property<long>("MouthStyle");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(33);

                    b.Property<bool>("NameRejected");

                    b.Property<long>("PantsColor");

                    b.Property<long>("Rh");

                    b.Property<string>("Rocket")
                        .HasMaxLength(30);

                    b.Property<long>("ShirtColor");

                    b.Property<long>("ShirtStyle");

                    b.Property<long>("TotalArmorPowerUpsCollected");

                    b.Property<long>("TotalArmorRepaired");

                    b.Property<long>("TotalBricksCollected");

                    b.Property<long>("TotalCurrencyCollected");

                    b.Property<long>("TotalDamageHealed");

                    b.Property<long>("TotalDamageTaken");

                    b.Property<long>("TotalDistanceDriven");

                    b.Property<long>("TotalDistanceTraveled");

                    b.Property<long>("TotalEnemiesSmashed");

                    b.Property<long>("TotalFirstPlaceFinishes");

                    b.Property<long>("TotalImaginationPowerUpsCollected");

                    b.Property<long>("TotalImaginationRestored");

                    b.Property<long>("TotalImaginationUsed");

                    b.Property<long>("TotalLifePowerUpsCollected");

                    b.Property<long>("TotalMissionsCompleted");

                    b.Property<long>("TotalPetsTamed");

                    b.Property<long>("TotalQuickBuildsCompleted");

                    b.Property<long>("TotalRacecarBoostsActivated");

                    b.Property<long>("TotalRacecarWrecks");

                    b.Property<long>("TotalRacesFinished");

                    b.Property<long>("TotalRacingImaginationCratesSmashed");

                    b.Property<long>("TotalRacingImaginationPowerUpsCollected");

                    b.Property<long>("TotalRacingSmashablesSmashed");

                    b.Property<long>("TotalRocketsUsed");

                    b.Property<long>("TotalSmashablesSmashed");

                    b.Property<long>("TotalSuicides");

                    b.Property<long>("TotalTimeAirborne");

                    b.Property<long>("UniverseScore");

                    b.Property<long>("UserId");

                    b.HasKey("CharacterId");

                    b.HasIndex("UserId");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Uchu.Core.Friend", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("FriendId");

                    b.Property<long>("FriendTwoId");

                    b.Property<bool>("IsAccepted");

                    b.Property<bool>("IsBestFriend");

                    b.Property<bool>("IsDeclined");

                    b.Property<bool>("RequestHasBeenSent");

                    b.Property<bool>("RequestingBestFriend");

                    b.HasKey("Id");

                    b.HasIndex("FriendId");

                    b.HasIndex("FriendTwoId");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("Uchu.Core.InventoryItem", b =>
                {
                    b.Property<long>("InventoryItemId")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("CharacterId");

                    b.Property<long>("Count");

                    b.Property<string>("ExtraInfo");

                    b.Property<int>("InventoryType");

                    b.Property<bool>("IsBound");

                    b.Property<bool>("IsEquipped");

                    b.Property<int>("LOT");

                    b.Property<int>("Slot");

                    b.HasKey("InventoryItemId");

                    b.HasIndex("CharacterId");

                    b.ToTable("InventoryItems");
                });

            modelBuilder.Entity("Uchu.Core.Mission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("CharacterId");

                    b.Property<int>("CompletionCount");

                    b.Property<long>("LastCompletion");

                    b.Property<int>("MissionId");

                    b.Property<int>("State");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.ToTable("Missions");
                });

            modelBuilder.Entity("Uchu.Core.MissionTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("MissionId");

                    b.Property<int>("TaskId");

                    b.HasKey("Id");

                    b.HasIndex("MissionId");

                    b.ToTable("MissionTasks");
                });

            modelBuilder.Entity("Uchu.Core.MissionTaskValue", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Count");

                    b.Property<int>("MissionTaskId");

                    b.Property<float>("Value");

                    b.HasKey("Id");

                    b.HasIndex("MissionTaskId");

                    b.ToTable("MissionTaskValue");
                });

            modelBuilder.Entity("Uchu.Core.ServerSpecification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ActiveUserCount");

                    b.Property<long>("MaxUserCount");

                    b.Property<int>("Port");

                    b.Property<int>("ServerType");

                    b.Property<long>("ZoneCloneId");

                    b.Property<int>("ZoneId");

                    b.Property<int>("ZoneInstanceId");

                    b.HasKey("Id");

                    b.ToTable("Specifications");
                });

            modelBuilder.Entity("Uchu.Core.SessionCache", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("CharacterId");

                    b.Property<string>("Key");

                    b.Property<long>("UserId");

                    b.Property<int>("ZoneId");

                    b.HasKey("Id");

                    b.ToTable("SessionCaches");
                });

            modelBuilder.Entity("Uchu.Core.UnlockedEmote", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("CharacterId");

                    b.Property<int>("EmoteId");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.ToTable("UnlockedEmote");
                });

            modelBuilder.Entity("Uchu.Core.User", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Banned");

                    b.Property<string>("BannedReason");

                    b.Property<int>("CharacterIndex");

                    b.Property<string>("CustomLockout");

                    b.Property<bool>("FirstTimeOnSubscription");

                    b.Property<bool>("FreeToPlay");

                    b.Property<int>("GameMasterLevel");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(60);

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(33);

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Uchu.Core.WorldServerRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("SpecificationId");

                    b.Property<int>("State");

                    b.Property<int>("ZoneId");

                    b.HasKey("Id");

                    b.ToTable("WorldServerRequests");
                });

            modelBuilder.Entity("Uchu.Core.Character", b =>
                {
                    b.HasOne("Uchu.Core.User", "User")
                        .WithMany("Characters")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Uchu.Core.Friend", b =>
                {
                    b.HasOne("Uchu.Core.Character", "FriendOne")
                        .WithMany()
                        .HasForeignKey("FriendId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Uchu.Core.Character", "FriendTwo")
                        .WithMany()
                        .HasForeignKey("FriendTwoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Uchu.Core.InventoryItem", b =>
                {
                    b.HasOne("Uchu.Core.Character", "Character")
                        .WithMany("Items")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Uchu.Core.Mission", b =>
                {
                    b.HasOne("Uchu.Core.Character", "Character")
                        .WithMany("Missions")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Uchu.Core.MissionTask", b =>
                {
                    b.HasOne("Uchu.Core.Mission", "Mission")
                        .WithMany("Tasks")
                        .HasForeignKey("MissionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Uchu.Core.MissionTaskValue", b =>
                {
                    b.HasOne("Uchu.Core.MissionTask", "MissionTask")
                        .WithMany("Values")
                        .HasForeignKey("MissionTaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Uchu.Core.UnlockedEmote", b =>
                {
                    b.HasOne("Uchu.Core.Character", "Character")
                        .WithMany("UnlockedEmotes")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
