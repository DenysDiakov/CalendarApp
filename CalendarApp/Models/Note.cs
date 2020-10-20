using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalendarApp.Models
{
	public class Note
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[RegularExpression("^([0-1]?[0-9]|2[0-3])$")]
		public string Hours { get; set; }

		[Required]
		[RegularExpression("^[0-5][0-9]$")]
		public string Minutes { get; set; }

		[Required]
		[MaxLength(70)]
		public string Task { get; set; }

		public DateTime CalendarDate { get; set; }

		public Calendar Calendar { get; set; }
	}
}
