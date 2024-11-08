﻿using SelfEduNet.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


using static System.Reflection.Metadata.BlobBuilder;

namespace SelfEduNet.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<AppUser> Users { get; set; }
    }
}
