﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace backendWeb.Models.Repositories
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RYMimoneyEntities : DbContext
    {
        public RYMimoneyEntities()
            : base("name=RYMimoneyEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<recevieCases> recevieCases { get; set; }
        public virtual DbSet<postfile> postfile { get; set; }
        public virtual DbSet<members> members { get; set; }
        public virtual DbSet<logJson> logJson { get; set; }
        public virtual DbSet<backendMenu> backendMenu { get; set; }
        public virtual DbSet<backendUser> backendUser { get; set; }
        public virtual DbSet<backendRoleGroup> backendRoleGroup { get; set; }
    }
}
