﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Sql.Operations
{
	/// <summary>
	/// Use to define paging in select operations, for example, if you only want to retrieve DataObjects from index 0 to 100, or 101 to 200
	/// </summary>
	public class SelectLimit
	{
		public int Id { get; set; }

		/// <summary>
		/// Starting 0-basex index for the list you want to select
		/// </summary>
		public int From { get; set; }

		/// <summary>
		/// Finishing 0-basex index for the list you want to select
		/// </summary>
		public int To { get; set; }

		/// <summary>
		/// Returns the count
		/// </summary>
		public int Count 
		{ 
			get
			{
				return From - To;
			}
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public SelectLimit()
		{
			From = To = 0;
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="from">
		/// </param>
		/// <param name="to">
		/// </param>
		public SelectLimit(int from, int to)
		{
			From = from;
			To = to;
		}
	}
}
