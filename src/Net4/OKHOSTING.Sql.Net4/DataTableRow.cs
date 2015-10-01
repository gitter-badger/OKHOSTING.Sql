using System;
using System.Collections;

namespace OKHOSTING.Sql.Net4
{
	public class DataTableRow : IDataRow
	{
		public readonly DataTable Container;
		public readonly System.Data.DataRow NativeRow;

		public DataTableRow(DataTable container, System.Data.DataRow nativeRow)
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
				return NativeRow[name];
			}
		}

		public int FieldCount
		{
			get
			{
				return NativeRow.Table.Columns.Count;
			}
		}

		public void Dispose()
		{
			//do nothing now?
		}

		public T GetFieldValue<T>(int ordinal)
		{
			return Data.Convert.ChangeType<T>(this[ordinal]);
		}

		public bool IsNull(int ordinal)
		{
			return NativeRow.IsNull(ordinal);
		}

		public IEnumerator GetEnumerator()
		{
			foreach (object item in NativeRow.ItemArray)
			{
				yield return item;
			}
		}
	}
}