using OKHOSTING.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.Schema
{
	public class TableDictionary: DictionaryBase<string, Table>
	{
		public override void Add(KeyValuePair<string, Table> item)
		{
			throw new NotImplementedException();
		}

		public override bool ContainsKey(string key)
		{
			throw new NotImplementedException();
		}

		public override ICollection<string> Keys
		{
			get { throw new NotImplementedException(); }
		}

		public override bool Remove(string key)
		{
			throw new NotImplementedException();
		}

		public override Table this[string key]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		public override IEnumerator<KeyValuePair<string, Table>> GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}