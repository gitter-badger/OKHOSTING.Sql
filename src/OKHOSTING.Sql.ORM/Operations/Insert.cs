﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Operations
{
	public class Insert
	{
		public DataType Into { get; set; }
		public readonly List<MemberValue> Values = new List<MemberValue>();
	}
}