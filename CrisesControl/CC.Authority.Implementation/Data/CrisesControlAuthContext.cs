using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CC.Authority.Implementation.Models;

namespace CC.Authority.Implementation.Data
{
    public partial class CrisesControlAuthContext : DbContext
    {
        public CrisesControlAuthContext()
        {
        }

        public CrisesControlAuthContext(DbContextOptions<CrisesControlAuthContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Companies { get; set; } = null!;
        public virtual DbSet<CompanyParameter> CompanyParameters { get; set; } = null!;
        public virtual DbSet<CompanyPaymentProfile> CompanyPaymentProfiles { get; set; } = null!;
        public virtual DbSet<Country> Countries { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<FailedLoginAttempt> FailedLoginAttempts { get; set; } = null!;
        public virtual DbSet<GetStarted> GetStarteds { get; set; } = null!;
        public virtual DbSet<Group> Groups { get; set; } = null!;
        public virtual DbSet<Location> Locations { get; set; } = null!;
        public virtual DbSet<ObjectRelation> ObjectRelations { get; set; } = null!;
        public virtual DbSet<PasswordChangeHistory> PasswordChangeHistories { get; set; } = null!;
        public virtual DbSet<PaymentProfile> PaymentProfiles { get; set; } = null!;
        public virtual DbSet<PreContractOffer> PreContractOffers { get; set; } = null!;
        public virtual DbSet<SecurityGroup> SecurityGroups { get; set; } = null!;
        public virtual DbSet<StdTimeZone> StdTimeZones { get; set; } = null!;
        public virtual DbSet<TwoFactorAuthLog> TwoFactorAuthLogs { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserDepartment> UserDepartments { get; set; } = null!;
        public virtual DbSet<UserGroup> UserGroups { get; set; } = null!;
        public virtual DbSet<UserLocation> UserLocations { get; set; } = null!;
        public virtual DbSet<UserLocation1> UserLocations1 { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;
        public virtual DbSet<UserRoleChange> UserRoleChanges { get; set; } = null!;
        public virtual DbSet<UserSecurityGroup> UserSecurityGroups { get; set; } = null!;
        public virtual DbSet<VwUserGroup> VwUserGroups { get; set; } = null!;
        public virtual DbSet<VwUserLocation> VwUserLocations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Company");

                entity.Property(e => e.AndroidLogo).HasMaxLength(100);

                entity.Property(e => e.CompanyLogoPath).HasMaxLength(100);

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(200)
                    .HasColumnName("Company_Name");

                entity.Property(e => e.CompanyProfile).HasMaxLength(150);

                entity.Property(e => e.ContactLogoPath).HasMaxLength(100);

                entity.Property(e => e.CustomerId).HasMaxLength(100);

                entity.Property(e => e.Fax).HasMaxLength(20);

                entity.Property(e => e.IOslogo)
                    .HasMaxLength(100)
                    .HasColumnName("iOSLogo");

                entity.Property(e => e.InvitationCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Isdcode)
                    .HasMaxLength(10)
                    .HasColumnName("ISDCode");

                entity.Property(e => e.PlanDrdoc)
                    .HasMaxLength(100)
                    .HasColumnName("PlanDRDoc");

                entity.Property(e => e.Sector).HasMaxLength(150);

                entity.Property(e => e.SwitchBoardPhone).HasMaxLength(20);

                entity.Property(e => e.UniqueKey)
                    .HasMaxLength(150)
                    .UseCollation("Ukrainian_BIN2");

                entity.Property(e => e.Website).HasMaxLength(250);

                entity.Property(e => e.WindowsLogo).HasMaxLength(100);
            });

            modelBuilder.Entity<CompanyParameter>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Name).HasMaxLength(70);

                entity.Property(e => e.Value).HasMaxLength(500);
            });

            modelBuilder.Entity<CompanyPaymentProfile>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CompanyPaymentProfile");

                entity.Property(e => e.AgreementNo).HasMaxLength(50);

                entity.Property(e => e.BillingAddress1).HasMaxLength(150);

                entity.Property(e => e.BillingAddress2).HasMaxLength(150);

                entity.Property(e => e.BillingEmail).HasMaxLength(150);

                entity.Property(e => e.CardHolderName).HasMaxLength(150);

                entity.Property(e => e.CardType).HasMaxLength(50);

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.ConfUplift).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.Country).HasMaxLength(150);

                entity.Property(e => e.CreditBalance).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.CreditLimit).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.EmailUplift).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(20)
                    .HasColumnName("IPAddress");

                entity.Property(e => e.MaxTransactionLimit).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MinimumBalance).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.MinimumConfRate).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.MinimumEmailRate).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.MinimumPhoneRate).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.MinimumPushRate).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.MinimumTextRate).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.PaymentPeriod).HasMaxLength(10);

                entity.Property(e => e.PhoneUplift).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.Postcode).HasMaxLength(20);

                entity.Property(e => e.PushUplift).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.SoptokenValue)
                    .HasColumnType("decimal(20, 4)")
                    .HasColumnName("SOPTokenValue");

                entity.Property(e => e.TextUplift).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.Town).HasMaxLength(50);

                entity.Property(e => e.Vatrate)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VATRate");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Country");

                entity.Property(e => e.CountryCode).HasMaxLength(128);

                entity.Property(e => e.CountryId).HasColumnName("CountryID");

                entity.Property(e => e.CountryPhoneCode).HasMaxLength(50);

                entity.Property(e => e.Iso2code)
                    .HasMaxLength(3)
                    .HasColumnName("ISO2Code");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Smsavailable).HasColumnName("SMSAvailable");

                entity.Property(e => e.SmspriceUrl)
                    .HasMaxLength(200)
                    .HasColumnName("SMSPriceURL");

                entity.Property(e => e.VoicePriceUrl)
                    .HasMaxLength(200)
                    .HasColumnName("VoicePriceURL");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Department");

                entity.Property(e => e.DepartmentName).HasMaxLength(100);
            });

            modelBuilder.Entity<FailedLoginAttempt>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("FailedLoginAttempt");

                entity.Property(e => e.CustomerId).HasMaxLength(50);

                entity.Property(e => e.EmailId).HasMaxLength(150);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(250)
                    .HasColumnName("IPAddress");

                entity.Property(e => e.PasswordUsed).HasMaxLength(150);
            });

            modelBuilder.Entity<GetStarted>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("GetStarted");

                entity.Property(e => e.Address1)
                    .HasMaxLength(250)
                    .HasColumnName("address1");

                entity.Property(e => e.Address2)
                    .HasMaxLength(250)
                    .HasColumnName("address2");

                entity.Property(e => e.Assdone).HasColumnName("assdone");

                entity.Property(e => e.Cisdcode)
                    .HasMaxLength(10)
                    .HasColumnName("CISDCode");

                entity.Property(e => e.City).HasMaxLength(100);

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(250)
                    .HasColumnName("companyName");

                entity.Property(e => e.Country).HasMaxLength(5);

                entity.Property(e => e.DepartmentName).HasMaxLength(150);

                entity.Property(e => e.Depdone).HasColumnName("depdone");

                entity.Property(e => e.Email).HasMaxLength(150);

                entity.Property(e => e.FirstName).HasMaxLength(150);

                entity.Property(e => e.Gsid).HasColumnName("GSId");

                entity.Property(e => e.Incidone).HasColumnName("incidone");

                entity.Property(e => e.Landline).HasMaxLength(15);

                entity.Property(e => e.LastName).HasMaxLength(150);

                entity.Property(e => e.Llisdcode)
                    .HasMaxLength(10)
                    .HasColumnName("LLISDCode");

                entity.Property(e => e.Locdone).HasColumnName("locdone");

                entity.Property(e => e.Mobile).HasMaxLength(15);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.Postcode).HasMaxLength(20);

                entity.Property(e => e.SessionId)
                    .HasMaxLength(100)
                    .HasColumnName("sessionId");

                entity.Property(e => e.State).HasMaxLength(100);

                entity.Property(e => e.SwtchPhone).HasMaxLength(15);

                entity.Property(e => e.Uisdcode)
                    .HasMaxLength(10)
                    .HasColumnName("UISDCode");

                entity.Property(e => e.Userdone).HasColumnName("userdone");

                entity.Property(e => e.Website).HasMaxLength(250);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Group");

                entity.Property(e => e.GroupName).HasMaxLength(100);
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Location");

                entity.Property(e => e.Desc).HasMaxLength(250);

                entity.Property(e => e.Lat).HasMaxLength(20);

                entity.Property(e => e.LocationName)
                    .HasMaxLength(150)
                    .HasColumnName("Location_Name");

                entity.Property(e => e.Long).HasMaxLength(20);

                entity.Property(e => e.PostCode).HasMaxLength(250);
            });

            modelBuilder.Entity<ObjectRelation>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ObjectRelation");
            });

            modelBuilder.Entity<PasswordChangeHistory>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PasswordChangeHistory");

                entity.Property(e => e.LastPassword).HasMaxLength(50);
            });

            modelBuilder.Entity<PaymentProfile>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PaymentProfile");

                entity.Property(e => e.ConfUplift).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.CreditBalance).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.CreditLimit).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.EmailUplift).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.MinimumBalance).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.MinimumConfRate).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.MinimumEmailRate).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.MinimumPhoneRate).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.MinimumPushRate).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.MinimumTextRate).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.PhoneUplift).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.PushUplift).HasColumnType("decimal(20, 4)");

                entity.Property(e => e.SoptokenValue)
                    .HasColumnType("decimal(20, 4)")
                    .HasColumnName("SOPTokenValue");

                entity.Property(e => e.TextUplift).HasColumnType("decimal(20, 4)");
            });

            modelBuilder.Entity<PreContractOffer>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PreContractOffer");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.KeyHolderRate).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MonthlyContractValue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.OfferId).HasColumnName("OfferID");

                entity.Property(e => e.StaffRate).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.YearlyContractValue).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<SecurityGroup>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SecurityGroup");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UserRole).HasMaxLength(20);
            });

            modelBuilder.Entity<StdTimeZone>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("StdTimeZone");

                entity.Property(e => e.PortalTimeZone).HasMaxLength(100);

                entity.Property(e => e.TimeZoneId).HasColumnName("TimeZoneID");

                entity.Property(e => e.ZoneId).HasMaxLength(100);

                entity.Property(e => e.ZoneLabel).HasMaxLength(100);
            });

            modelBuilder.Entity<TwoFactorAuthLog>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TwoFactorAuthLog");

                entity.Property(e => e.CloudMessageId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.FirstName)
                    .HasMaxLength(70)
                    .UseCollation("Ukrainian_BIN2");

                entity.Property(e => e.Isdcode)
                    .HasMaxLength(10)
                    .HasColumnName("ISDCode")
                    .UseCollation("Ukrainian_BIN2");

                entity.Property(e => e.Landline).HasMaxLength(20);

                entity.Property(e => e.LastName)
                    .HasMaxLength(70)
                    .UseCollation("Ukrainian_BIN2");

                entity.Property(e => e.Lat).HasMaxLength(20);

                entity.Property(e => e.Llisdcode)
                    .HasMaxLength(10)
                    .HasColumnName("LLISDCode")
                    .UseCollation("Ukrainian_BIN2");

                entity.Property(e => e.Lng).HasMaxLength(20);

                entity.Property(e => e.MobileNo)
                    .HasMaxLength(20)
                    .UseCollation("Ukrainian_BIN2");

                entity.Property(e => e.Otpcode)
                    .HasMaxLength(10)
                    .HasColumnName("OTPCode");

                entity.Property(e => e.Otpexpiry).HasColumnName("OTPExpiry");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .UseCollation("Ukrainian_BIN2");

                entity.Property(e => e.PrimaryEmail)
                    .HasMaxLength(150)
                    .UseCollation("Ukrainian_BIN2");

                entity.Property(e => e.SecondaryEmail)
                    .HasMaxLength(150)
                    .UseCollation("Ukrainian_BIN2");

                entity.Property(e => e.Smstrigger).HasColumnName("SMSTrigger");

                entity.Property(e => e.UniqueGuiId)
                    .HasMaxLength(70)
                    .HasColumnName("UniqueGuiID");

                entity.Property(e => e.UserHash)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.UserLanguage).HasMaxLength(20);

                entity.Property(e => e.UserPhoto).HasMaxLength(70);

                entity.Property(e => e.UserRole).HasMaxLength(10);
            });

            modelBuilder.Entity<UserDepartment>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("User_Department");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.DepartmentName).HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(70);

                entity.Property(e => e.LastName).HasMaxLength(70);

                entity.Property(e => e.UniqueId).HasColumnName("UniqueID");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("User_Group");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.FirstName).HasMaxLength(70);

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.GroupName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(70);

                entity.Property(e => e.UniqueId).HasColumnName("UniqueID");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserLocation>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("User_Location");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.Desc).HasMaxLength(250);

                entity.Property(e => e.FirstName).HasMaxLength(70);

                entity.Property(e => e.LastName).HasMaxLength(70);

                entity.Property(e => e.Lat).HasMaxLength(20);

                entity.Property(e => e.LocationName)
                    .HasMaxLength(150)
                    .HasColumnName("Location_Name");

                entity.Property(e => e.Long).HasMaxLength(20);

                entity.Property(e => e.PostCode).HasMaxLength(250);

                entity.Property(e => e.UniqueId).HasColumnName("UniqueID");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserLocation1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("UserLocation");

                entity.Property(e => e.CreatedOnGmt).HasColumnName("CreatedOnGMT");

                entity.Property(e => e.LocationAddress).HasMaxLength(500);

                entity.Property(e => e.UserDeviceId).HasColumnName("UserDeviceID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.UserLocationId).HasColumnName("UserLocationID");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("UserRole");

                entity.Property(e => e.RoleCode).HasMaxLength(20);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<UserRoleChange>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("UserRoleChange");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.RoleChangeId).HasColumnName("RoleChangeID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.UserRole).HasMaxLength(20);
            });

            modelBuilder.Entity<UserSecurityGroup>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("UserSecurityGroup");

                entity.Property(e => e.UserSecurityGroupId).HasColumnName("UserSecurityGroupID");
            });

            modelBuilder.Entity<VwUserGroup>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("vw_UserGroups");
            });

            modelBuilder.Entity<VwUserLocation>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("vw_UserLocations");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
