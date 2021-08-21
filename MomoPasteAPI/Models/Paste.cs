using System;
using System.Text.Json.Serialization;

namespace MomoPasteAPI.Models
{
	public class Paste
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public String Title { get; set; } = "Untitled MomoPaste";
		public String Content { get; set; }
		public Boolean InvalidateAfterViewing { get; set; } = false;

		[JsonIgnore]
		public Boolean Viewed { get; set; } = false;

		[JsonIgnore]
		public Boolean Previewed { get; set; } = false;
		
		[JsonIgnore]
		public DateTime Created { get; set; } = DateTime.Now;

		[JsonIgnore]
		public DateTime? Expires { get; set; }
        
		public Boolean IsExpired()
		{
			return Expires != null && Expires < DateTime.Now;
		}

		public void Discard()
		{
			MomoContext db = new MomoContext();
			db.Pastes.Remove(this);
			db.SaveChanges();
		}

		public Int32 CharCount()
		{
			return Content.Length;
		}

		public String GetContent()
		{
			return Helpers.Base64.Decode(this.Content);
		}
	}
}