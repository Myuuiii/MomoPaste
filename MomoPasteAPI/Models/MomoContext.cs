using System;
using System.IO;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace MomoPasteAPI.Models
{
	public class MomoContext : DbContext
	{
		public MomoContext()
		{
			if (!File.Exists("config.json"))
			{
				MomoConfiguration config = new MomoConfiguration();
				File.WriteAllText("config.json", JsonSerializer.Serialize(config));
				Console.WriteLine("Created config.json, please configure");
				Environment.Exit(0);
			}
			else
			{
				MomoConfiguration _config = JsonSerializer.Deserialize<MomoConfiguration>(File.ReadAllText("config.json"));
				this._connectionString = $"server={_config.Host};uid={_config.Username};pwd={_config.Password};database={_config.Database}";
			}
		}

		private string _connectionString = "";
		override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
		}
		public DbSet<Paste> Pastes { get; set; }
	}
}