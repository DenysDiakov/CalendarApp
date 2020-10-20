using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CalendarApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CalendarApp.Controllers
{
	public class HomeController : Controller
	{
		private static DateTime _choosenDate = DateTime.Now;

		public IActionResult Index(int? m, int? y)
		{
			if (m != null &&
				m != 0 &&
				y != null &&
				y != 0)
			{
				var newDate = new DateTime((int)y, (int)m, 1);
				var diff = newDate.Subtract(_choosenDate);
				_choosenDate = _choosenDate.Add(diff);
			}
			return Index();
		}

		[HttpPost]
		public IActionResult Index()
		{
			var startDate = new DateTime(_choosenDate.Year, _choosenDate.Month, 1);
			var day = startDate.DayOfWeek;

			//Find the date of the first Monday
			if (day != DayOfWeek.Monday)
			{
				int subtractDays = day == DayOfWeek.Sunday ? 6 : (int)day - 1;
				startDate = startDate.AddDays(-subtractDays);
			}

			var endDate = startDate.AddDays(41);
			var allDates = new List<Calendar>();
			using (var db = new ApplicationContext())
			{
				//Get all dates for a selected time period with the amount of their tasks
				var notes = (from i in db.Notes
							 where i.CalendarDate >= startDate.Date && i.CalendarDate <= endDate.Date
							 group i by i.CalendarDate into g
							 select new
							 {
								 date = g.Key,
								 count = g.Count()
							 }).ToList();

				//Initialize TaskCount property if tasks exists
				for (var date = startDate; date <= endDate; date = date.AddDays(1))
				{
					var calendarDate = new Calendar() { Date = date };
					var dateNotes = notes.Find(x => x.date == date);
					if (dateNotes != null)
					{
						calendarDate.TaskCount = dateNotes.count;
					}
					allDates.Add(calendarDate);
				}
			}
			ViewBag.YearMonth = getYearMonth();

			//Divide days into weeks
			var weeks = allDates.Select((x, i) => new { Index = i, Value = x })
				.GroupBy(x => x.Index / 7)
				.Select(x => x.Select(v => v.Value).ToList())
				.ToList();
			return View(weeks);
		}

		private string getYearMonth()
		{
			return string.Format("{0} {1}", _choosenDate.ToString("MMMM"), _choosenDate.Year);
		}

		public IActionResult ChangeMonth(int number)
		{
			_choosenDate = _choosenDate.AddMonths(number);
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult Add(Note note)
		{
			if (ModelState.IsValid)
			{
				using (var db = new ApplicationContext())
				{
					var date = db.Dates.Where(x => x.Date.Equals(note.CalendarDate.Date)).ToList();
					if (date == null || date.Count == 0)
					{
						db.Dates.Add(new Calendar() { Date = note.CalendarDate.Date });
					}
					db.Notes.Add(note);
					db.SaveChanges();
				}
			}
			return RedirectToAction("Get", note.CalendarDate);
		}

		public IActionResult Get(DateTime date)
		{
			//Initialize hours dropdownlist
			string[] hours = new string[24];
			for (int i = 0; i < 24; i++)
			{
				string zero = i < 10 ? "0" : string.Empty;
				hours[i] = zero + i.ToString();
			}
			ViewBag.Hours = new SelectList(hours);

			//Initialize minutes dropdownlist
			string[] minutes = new string[12];
			minutes[0] = "00";
			for (int i = 1, j = 5; i < 12; i++, j = j + 5)
			{
				string zero = j < 10 ? "0" : string.Empty;
				minutes[i] = zero + j.ToString();
			}
			ViewBag.Minutes = new SelectList(minutes);

			using (var db = new ApplicationContext())
			{
				ViewBag.Title = string.Format("{0} {1}", date.Day, getYearMonth());

				//Get all tasks for a specific date
				var tasks = db.Notes.Where(x => x.CalendarDate.Equals(date.Date)).ToList();
				ViewBag.Dates = tasks;
			}
			return View(date);
		}

		public IActionResult Delete(int id, DateTime date)
		{
			using (var db = new ApplicationContext())
			{
				db.Notes.Remove(db.Notes.FirstOrDefault(x => x.Id == id));
				db.SaveChanges();
			}
			return RedirectToAction("Get", date);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
