using System;
using Pomelo.EntityFrameworkCore.MySql;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace cinema.Models;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Cinema> Cinemas { get; set; }

    public virtual DbSet<Follow> Follows { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<MovieLanguage> MovieLanguages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<Showtime> Showtimes { get; set; }

    public virtual DbSet<Sub> Subs { get; set; }

    public virtual DbSet<SeatReservation> SeatReservations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var conn = "Server=localhost;Port=3306;Database=cinema;User=root;Password=;";
            optionsBuilder.UseMySql(
                conn,
                ServerVersion.AutoDetect(conn),
                mySqlOpts => mySqlOpts
                    .EnableRetryOnFailure()
                    .CommandTimeout(60)
            );
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("account");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Birthday)
                .HasColumnType("date")
                .HasColumnName("birthday");
            entity.Property(e => e.Created)
                .HasColumnType("date")
                .HasColumnName("created");
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(250)
                .HasColumnName("gender");
            entity.Property(e => e.Password)
                .HasMaxLength(250)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(250)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasColumnType("int(11)")
                .HasColumnName("role");
            entity.Property(e => e.Securitycode)
                .HasMaxLength(250)
                .HasColumnName("securitycode");
            entity.Property(e => e.Username)
                .HasMaxLength(250)
                .HasColumnName("username");
            entity.Property(e => e.Verify)
                .HasColumnType("int(1)")
                .HasColumnName("verify");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("booking");

            entity.HasIndex(e => e.ShowtimeId, "showtimeID");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Created)
                .HasColumnType("date")
                .HasColumnName("created");
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(250)
                .HasColumnName("phone");
            entity.Property(e => e.ShowtimeId)
                .HasColumnType("int(11)")
                .HasColumnName("showtimeID");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Showtime).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ShowtimeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("booking_ibfk_1");
        });

        modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("booking_details");

            entity.HasIndex(e => e.BookingId, "bookingID");

            entity.HasIndex(e => e.SeatId, "seatID");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.BookingId)
                .HasColumnType("int(11)")
                .HasColumnName("bookingID");
            entity.Property(e => e.SeatId)
                .HasColumnType("int(11)")
                .HasColumnName("seatID");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("booking_details_ibfk_1");

            entity.HasOne(d => d.Seat).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("booking_details_ibfk_2");
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("chat");

            entity.HasIndex(e => e.AccountId, "accountID");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AccountId)
                .HasColumnType("int(11)")
                .HasColumnName("accountID");
            entity.Property(e => e.Message)
                .HasColumnType("text")
                .HasColumnName("message");
            entity.Property(e => e.Role)
                .HasColumnType("int(11)")
                .HasColumnName("role");
            entity.Property(e => e.Time)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("timestamp")
                .HasColumnName("time");

            entity.HasOne(d => d.Account).WithMany(p => p.Chats)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("chat_ibfk_1");
        });

        modelBuilder.Entity<Cinema>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cinema");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.City)
                .HasMaxLength(500)
                .HasColumnName("city");
            entity.Property(e => e.District)
                .HasMaxLength(500)
                .HasColumnName("district");
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .HasColumnName("name");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Follow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("follow");

            entity.HasIndex(e => e.AccountId, "accountId");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AccountId)
                .HasColumnType("int(11)")
                .HasColumnName("accountId");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Account).WithMany(p => p.Follows)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("follow_ibfk_1");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("language");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.CountryId)
                .HasMaxLength(250)
                .HasColumnName("countryID");
            entity.Property(e => e.CountryName)
                .HasMaxLength(250)
                .HasColumnName("countryName");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("movie");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Actor)
                .HasColumnType("text")
                .HasColumnName("actor");
            entity.Property(e => e.Age)
                .HasColumnType("int(11)")
                .HasColumnName("age");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Director)
                .HasColumnType("text")
                .HasColumnName("director");
            entity.Property(e => e.Duration)
                .HasMaxLength(250)
                .HasColumnName("duration");
            entity.Property(e => e.Genre)
                .HasColumnType("text")
                .HasColumnName("genre");
            entity.Property(e => e.Photo)
                .HasMaxLength(250)
                .HasColumnName("photo");
            entity.Property(e => e.Publisher)
                .HasColumnType("text")
                .HasColumnName("publisher");
            entity.Property(e => e.ReleaseDate)
                .HasColumnType("date")
                .HasColumnName("releaseDate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Title)
                .HasColumnType("text")
                .HasColumnName("title");
            entity.Property(e => e.Trailer)
                .HasColumnType("text")
                .HasColumnName("trailer");
        });

        modelBuilder.Entity<MovieLanguage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("movie_language");

            entity.HasIndex(e => e.LanguageId, "languageID");

            entity.HasIndex(e => e.MovieId, "movieID");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Duration)
                .HasMaxLength(250)
                .HasColumnName("duration");
            entity.Property(e => e.Genre)
                .HasColumnType("text")
                .HasColumnName("genre");
            entity.Property(e => e.LanguageId)
                .HasColumnType("int(11)")
                .HasColumnName("languageID");
            entity.Property(e => e.MovieId)
                .HasColumnType("int(11)")
                .HasColumnName("movieID");
            entity.Property(e => e.ReleaseDate)
                .HasColumnType("date")
                .HasColumnName("releaseDate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Title)
                .HasColumnType("text")
                .HasColumnName("title");

            entity.HasOne(d => d.Language).WithMany(p => p.MovieLanguages)
                .HasForeignKey(d => d.LanguageId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("movie_language_ibfk_1");

            entity.HasOne(d => d.Movie).WithMany(p => p.MovieLanguages)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("movie_language_ibfk_2");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("payment");

            entity.HasIndex(e => e.BookingId, "bookingID");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.BookingId)
                .HasColumnType("int(11)")
                .HasColumnName("bookingID");
            entity.Property(e => e.Created)
                .HasColumnType("datetime")
                .HasColumnName("created");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.PaymentType)
                .HasColumnType("int(11)")
                .HasColumnName("paymentType");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Qr)
                .HasColumnType("text")
                .HasColumnName("QR");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TicketNumber)
                .HasColumnType("int(11)")
                .HasColumnName("ticketNumber");
            entity.Property(e => e.TransactionNo)
                .HasMaxLength(250)
                .HasColumnName("transactionNo");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("payment_ibfk_1");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("rating");

            entity.HasIndex(e => e.AccountId, "accountID");

            entity.HasIndex(e => e.MovieId, "movieID");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AccountId)
                .HasColumnType("int(11)")
                .HasColumnName("accountID");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.Created)
                .HasColumnType("date")
                .HasColumnName("created");
            entity.Property(e => e.MovieId)
                .HasColumnType("int(11)")
                .HasColumnName("movieID");
            entity.Property(e => e.Rate)
                .HasColumnType("int(11)")
                .HasColumnName("rate");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Account).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("rating_ibfk_1");

            entity.HasOne(d => d.Movie).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("rating_ibfk_2");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("room");

            entity.HasIndex(e => e.CinemaId, "cinemaID");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.CinemaId)
                .HasColumnType("int(11)")
                .HasColumnName("cinemaID");
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .HasColumnName("name");
            entity.Property(e => e.Quantity)
                .HasColumnType("int(11)")
                .HasColumnName("quantity");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Cinema).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.CinemaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("room_ibfk_1");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("seat");

            entity.HasIndex(e => e.RoomId, "roomID");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.RoomId)
                .HasColumnType("int(11)")
                .HasColumnName("roomID");
            entity.Property(e => e.SeatType)
                .HasColumnType("int(11)")
                .HasColumnName("seatType");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Room).WithMany(p => p.Seats)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("seat_ibfk_1");
        });

        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("showtime");

            entity.HasIndex(e => e.CinemaId, "cinemaID");

            entity.HasIndex(e => e.MovieId, "movieID");

            entity.HasIndex(e => e.RoomId, "roomID");

            entity.HasIndex(e => e.SubId, "subID");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.CinemaId)
                .HasColumnType("int(11)")
                .HasColumnName("cinemaID");
            entity.Property(e => e.MovieId)
                .HasColumnType("int(11)")
                .HasColumnName("movieID");
            entity.Property(e => e.RoomId)
                .HasColumnType("int(11)")
                .HasColumnName("roomID");
            entity.Property(e => e.ShowDate)
                .HasColumnType("datetime")
                .HasColumnName("showDate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.SubId)
                .HasColumnType("int(11)")
                .HasColumnName("subID");

            entity.HasOne(d => d.Cinema).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.CinemaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("showtime_ibfk_1");

            entity.HasOne(d => d.Movie).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("showtime_ibfk_2");

            entity.HasOne(d => d.Room).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("showtime_ibfk_4");

            entity.HasOne(d => d.Sub).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.SubId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("showtime_ibfk_3");
        });

        modelBuilder.Entity<Sub>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("sub");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Created)
                .HasColumnType("date")
                .HasColumnName("created");
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .HasColumnName("name");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
