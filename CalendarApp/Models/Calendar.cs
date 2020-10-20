using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalendarApp.Models
{
	public class Calendar
	{
		[Key]
		[Column(TypeName = "date")]
		public DateTime Date { get; set; }

		[NotMapped]
		public int TaskCount { get; set; }

		public List<Note> Notes { get; set; }
	}
}
