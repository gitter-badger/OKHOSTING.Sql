using SQLitePCL.pretty;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace OKHOSTING.Sql.SQLite
{
	public class DataReader : IDataReader
	{
		public readonly DataBase DataBase;
		public readonly IEnumerable<IReadOnlyList<IResultSetValue>> NativeReader;
		public readonly IReadOnlyList<IResultSetValue> CurrentResult;
		public readonly IEnumerator<IReadOnlyList<IResultSetValue>> Enumerator;

		public DataReader(DataBase dataBase, IEnumerable<IReadOnlyList<IResultSetValue>> nativeReader)
		{
			if (dataBase == null)
			{
				throw new ArgumentNullException("dataBase");
			}

			if (nativeReader == null)
			{
				throw new ArgumentNullException("nativeReader");
			}

			DataBase = dataBase;
			NativeReader = nativeReader;
			Enumerator = nativeReader.GetEnumerator();
		}

		public object this[int ordinal]
		{
			get
			{
				return CurrentResult[ordinal];
			}
		}

		public object this[string name]
		{
			get
			{
				return CurrentResult[GetOrdinal(name)];
			}
		}

		public int FieldCount
		{
			get
			{
				return CurrentResult.Count;
			}
		}

		public bool IsClosed
		{
			get
			{
				return false;
			}
		}

		IEnumerable<DataColumn> IDataReader.Schema
		{
			get
			{
				for(int i = 0; i < FieldCount; i++)
				{
					yield return new DataColumn(GetName(i), DbTypeMapper.Parse(GetType(i)));
				}
			}
		}

		public void Close()
		{
		}

		public void Dispose()
		{
		}

		public string GetDataTypeName(int ordinal)
		{
			return CurrentResult[ordinal].ColumnInfo.DeclaredType;
		}

		public Type GetType(int ordinal)
		{
			DbType dbType = OKHOSTING.Sql.SQLite.DataBase.Parse(CurrentResult[ordinal].SQLiteType);
			return DbTypeMapper.Parse(dbType);
		}

		public T GetFieldValue<T>(int ordinal)
		{
			return OKHOSTING.Data.Convert.ChangeType<T>(CurrentResult[ordinal]);
		}

		public string GetName(int ordinal)
		{
			return CurrentResult[ordinal].ColumnInfo.Name;
		}

		public int GetOrdinal(string name)
		{
			var column = CurrentResult.Columns().Where(c=> c.Name == name).Single();
			return CurrentResult.Columns().ToList().IndexOf(column);
		}

		public bool IsNull(int ordinal)
		{
			return CurrentResult[ordinal] != null;
		}

		public bool NextResult()
		{
			throw new NotSupportedException();
		}

		public bool Read()
		{
			return Enumerator.MoveNext();
		}

		public IEnumerator GetEnumerator()
		{
			for (int i = 0; i < FieldCount; i++)
			{
				yield return CurrentResult[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
