using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{

    public DbSet<AppUser> Users { get; set; }



}
