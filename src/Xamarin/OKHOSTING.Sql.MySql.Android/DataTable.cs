using System;
using System.Collections;
using System.Collections.Generic;
using OKHOSTING.Sql.Schema;

namespace OKHOSTING.Sql.MySql.Android
{
	public class DataTable : IDataTable
	{
		public readonly DataBase DataBase;
		public readonly System.Data.DataTable NativeTable;

		public DataTable(DataBase dataBase, System.Data.DataTable nativeTable)
		{
			if (dataBase == null)
			{
				throw new ArgumentNullException("dataBase");
			}

			if (nativeTable == null)
			{
				throw new ArgumentNullException("nativeTable");
			}

			DataBase = dataBase;
			NativeTable = nativeTable;
		}

		public IDataRow this[int index]
		{
			get
			{
				return new DataTableRow(this, NativeTable.Rows[index]);
			}
		}

		public int Count
		{
			get
			{
				return NativeTable.Rows.Count;
			}
		}

		public string Name
		{
			get
			{
				return NativeTable.TableName;
			}

			set
			{
				NativeTable.TableName = value;
            }
		}

		public IEnumerable<DataColumn> Schema
		{
			get
			{
				foreach (System.Data.DataColumn nativeColumn in NativeTable.Columns)
				{
					yield return new DataColumn(nativeColumn.ColumnName, nativeColumn.DataType);
				}
			}
		}

		public void Dispose()
		{
			NativeTable.Dispose();
		}

		public IEnumerator<IDataRow> GetEnumerator()
		{
			foreach (System.Data.DataRow nativeRow in NativeTable.Rows)
			{
				yield return new DataTableRow(this, nativeRow);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}