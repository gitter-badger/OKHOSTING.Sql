﻿using OKHOSTING.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Schema
{
	[System.ComponentModel.DefaultProperty("Name")]
	public class Index
	{
		public string Name { get; set; }
		public Table Table { get; set; }
		public List<Column> Columns { get; set; }
		public SortDirection Direction { get; set; }
		public bool Unique { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}