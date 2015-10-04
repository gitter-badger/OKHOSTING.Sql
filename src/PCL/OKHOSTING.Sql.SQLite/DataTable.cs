using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using OKHOSTING.Sql.Schema;
using SQLitePCL.pretty;

namespace OKHOSTING.Sql.SQLite
{
	public class DataTable : IDataTable
	{
		public readonly DataBase DataBase;
		public readonly DataReader DataReader;

		public DataTable(DataBase dataBase, DataReader reader)
		{
			if (dataBase == null)
			{
				throw new ArgumentNullException("dataBase");
			}

			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}

			DataBase = dataBase;
			DataReader = reader;
		}

		public IDataRow this[int index]
		{
			get
			{
				return new DataTableRow(this, DataReader.NativeReader.ToArray()[index]);
			}
		}

		public int Count
		{
			get
			{
				return DataReader.NativeReader.Count();
			}
		}

		public string Name
		{
			get
			{
				return DataReader.NativeReader.First().Columns()[0].TableName;
			}
			set
			{
				//NativeTable.TableName = value;
            }
		}

		public IEnumerable<DataColumn> Schema
		{
			get
			{
				foreach (var nativeColumn in DataReader.NativeReader.First())
				{
					yield return new DataColumn(nativeColumn.ColumnInfo.Name, DataBase.Parse(nativeColumn.SQLiteType));
				}
			}
		}

		public void Dispose()
		{
			DataReader.Dispose();
		}

		public IEnumerator<IDataRow> GetEnumerator()
		{
			while (DataReader.Read())
			{
				yield return new DataTableRow(this, DataReader.CurrentResult);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}