namespace DM.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Patient_Code_Unique : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Patient", "Code", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Patient", new[] { "Code" });
        }
    }
}
