using SQLitePCL.pretty;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace OKHOSTING.Sql.SQLite
{
	public class DataTableRow : IDataRow
	{
		public readonly DataTable Container;
		public readonly IReadOnlyList<IResultSetValue> NativeRow;

        public DataTableRow(DataTable container, IReadOnlyList<IResultSetValue> nativeRow)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}

			if (nativeRow == null)
			{
				throw new ArgumentNullException("nativeRow");
			}

			Container = container;
			NativeRow = nativeRow;
		}

		public object this[int ordinal]
		{
			get
			{
				return NativeRow[ordinal];
			}
		}

		public object this[string name]
		{
			get
			{
				var column = NativeRow.Columns().Where(c=> c.Name == name).Single();
				return NativeRow[NativeRow.Columns().ToList().IndexOf(column)];
			}
		}

		public int FieldCount
		{
			get
			{
				return NativeRow.Count;
			}
		}

		public void Dispose()
		{
		}

		public T GetFieldValue<T>(int ordinal)
		{
			return Data.Convert.ChangeType<T>(this[ordinal]);
		}

		public bool IsNull(int ordinal)
		{
			return NativeRow[ordinal] == null;
		}

		public IEnumerator GetEnumerator()
		{
			for (int i = 0; i < NativeRow.Columns().Count; i++)
			{
				yield return NativeRow[i];
			}
		}
	}
}