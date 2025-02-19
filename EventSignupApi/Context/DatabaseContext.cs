
using EventSignupApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSignupApi.Context;

/* Her lager vi en databaseContext class, som inheriter en haug av ferdigskrevne metoder og subklasser fra DbContext classen gitt til  oss.
Samt et sett med metoder som tar våre modeller, og genererer tabeller basert på Modellene sine.*/
public class DatabaseContext: DbContext
{
    /* DbSet representerer en tabell i vår database som heter Events. I vår context kan vi se på det som en liste av Event objekter. 
    Dette er hovedkjernen til funksjonen til en O R M, å Mappe Relations mellom Objekter i dataminnet og Objekter på databasen.
    I dette tilfellet knytte en relation mellom et Table på databasen Events, og en liste over Event objekter som heter Events.*/
    public DbSet<Event> Events{get;set;}
    public DbSet<User> Users{get;set;}
    public DbSet<EventGenreLookupTable> EventGenreLookup{get;set;}
    /* Her lager vi en Override av OnConfiguring metoden til DbContext, som i vår builder, sier at EfCore skal knyttes til en sqlite Database. */
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        /* Data Source representerer hvor metoden skal finne / lage databasefilen. */
        optionsBuilder.UseSqlite("Data Source=Database/EventDatabase.db; foreign keys=true;");
        optionsBuilder.LogTo(Console.WriteLine);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Genre)
            .WithMany(g =>  g.Events)
            .HasForeignKey(e => e.GenreId);
        modelBuilder.Entity<Event>()
            .HasOne(e=>e.Owner)
            .WithOne(o => o.OwnedEvent);
        modelBuilder.Entity<Event>()
            .HasMany(e => e.Admins)
            .WithMany(a => a.AdminEvents);
        modelBuilder.Entity<Event>()
            .HasMany(e => e.SignUps)
            .WithMany(s => s.SignUpEvents);
    }
}