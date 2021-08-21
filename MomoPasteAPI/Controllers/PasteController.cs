using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MomoPasteAPI.Models;

namespace MomoPasteAPI.Controllers
{
	public class PasteController : Controller
	{

		public const String urlBase = "http://momo.myuuiii.moe/p/";
		private MomoContext _db;
		public PasteController(MomoContext db)
		{
			_db = db;
		}

		[HttpGet("/")]
		public IActionResult Index()
		{
			return View("./Views/Index.cshtml");
		}

		[HttpGet(Routes.V1.PasteRoutes.GetPasteWeb)]
		public ActionResult<Paste> GetPaste(Guid id)
		{
			try
			{
				if (_db.Pastes.Any(p => p.Id == id))
				{
					Paste paste = _db.Pastes.Single(p => p.Id == id);

					ViewData["paste"] = paste;
					ViewData["pasteDescription"] = $"Characters: {paste.Content.ToCharArray().Length} \nCreated on: {paste.Created.Day} {paste.Created.Month} {paste.Created.Year}";

					if (!paste.Previewed && paste.InvalidateAfterViewing && !paste.IsExpired())
					{
						paste.Previewed = true;

						_db.Pastes.Update(paste);
						_db.SaveChanges();

						return View("./Views/Paste.cshtml");
					}
					else if (!paste.Viewed && paste.InvalidateAfterViewing && !paste.IsExpired())
					{
						paste.Viewed = true;

						_db.Pastes.Update(paste);
						_db.SaveChanges();

						return View("./Views/Paste.cshtml");
					}
					else if (paste.Viewed && paste.InvalidateAfterViewing && !paste.IsExpired())
					{
						return View("./Views/AlreadyViewed.cshtml");
					}
					else if (paste.IsExpired())
					{
						return View("./Views/Expired.cshtml");
					}
					else
					{
						return View("./Views/Paste.cshtml");
					}
				}
				else
				{
					return View("./Views/NotFound.cshtml");
				}
			}
			catch
			{
				return BadRequest();
			}
		}

		[HttpGet(Routes.V1.PasteRoutes.CreatePasteWeb)]
		public ActionResult CreatePasteWeb() => View("./Views/Create.cshtml");


		[HttpPost(Routes.V1.PasteRoutes.CreatePasteWeb)]
		public ActionResult CreatePasteWeb(Paste paste, Int32 delete, Int32 expires)
		{
			try
			{
				if (paste.Content.Length > 100000)
				{
					return View("./Views/TooLong.cshtml");
				}

				paste.Content = Helpers.Base64.Encode(paste.Content);

				if (expires != 0)
				{
					paste.Expires = DateTime.Now.AddMinutes(expires);
				}

				paste.InvalidateAfterViewing = Convert.ToBoolean(delete);

				_db.Pastes.Add(paste);
				_db.SaveChanges();

				ViewData["paste"] = paste;
				ViewData["pasteDescription"] = $"Characters: {paste.Content.ToCharArray().Length} \nCreated on: {paste.Created.Day} {paste.Created.Month} {paste.Created.Year}";
				return Redirect("/p/" + paste.Id);
			}
			catch (Exception e)
			{
				return BadRequest();
			}
		}

		[HttpGet(Routes.V1.PasteRoutes.GetPasteAPI)]
		public ActionResult<Paste> GetPasteAPI(Guid id)
		{
			try
			{
				if (_db.Pastes.Any(p => p.Id == id))
				{
					Paste paste = _db.Pastes.Single(p => p.Id == id);

					if (!paste.Viewed && paste.InvalidateAfterViewing && !paste.IsExpired())
					{
						paste.Previewed = true;
						paste.Viewed = true;

						_db.Pastes.Update(paste);
						_db.SaveChanges();

						return paste;
					}
					else if (paste.Viewed && paste.InvalidateAfterViewing && !paste.IsExpired())
					{
						return Ok("Paste has already been viewed");
					}
					else if (paste.IsExpired())
					{
						return Ok("Paste has expired");
					}
					else
					{
						return paste;
					}
				}
				else
				{
					return NotFound();
				}
			}
			catch
			{
				return BadRequest();
			}
		}

		[HttpPost(Routes.V1.PasteRoutes.CreatePasteAPI)]
		public ActionResult<Paste> CreatePasteAPI([FromBody] Paste paste, [FromQuery] Int32 expires = 0, [FromQuery] Boolean invalidateAfterViewing = false)
		{
			try
			{
				if (Helpers.Base64.Decode(paste.Content).Length > 100000)
				{
					return BadRequest("Your paste size is limited to 100.000 characters");
				}

				paste.Id = Guid.NewGuid();
				if (expires != 0)
				{
					paste.Expires = DateTime.Now.AddMinutes(expires);
				}
				paste.InvalidateAfterViewing = invalidateAfterViewing;

				_db.Pastes.Add(paste);
				_db.SaveChanges();
				return paste;
			}
			catch
			{
				return BadRequest();
			}
		}
	}
}