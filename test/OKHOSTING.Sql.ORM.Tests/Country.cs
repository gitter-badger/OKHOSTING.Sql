﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Tests
{
	public class Country
	{
		public int Id;
		
		[System.ComponentModel.DataAnnotations.StringLength(50)]
		public string Name;
	}
}