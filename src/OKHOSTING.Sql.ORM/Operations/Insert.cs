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
		public object Instance { get; set; }
		public readonly List<DataMember> Values = new List<DataMember>();
	}

	public class Insert<T> : Insert
	{
		public new T Instance
		{
			get
			{
				return (T) base.Instance;
			}
			set
			{
				base.Instance = value;
			}
		}

		public Insert()
		{
			Into = typeof(T);
		}
	}
}