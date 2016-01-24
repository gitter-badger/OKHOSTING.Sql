using System;
using System.Collections;
using System.Collections.Generic;
using OKHOSTING.Sql.Schema;
using MySqlPCL = global::MySql.Data.MySqlClient;

namespace OKHOSTING.Sql.Xamarin.iOS.MySql
{
	public class DataReader : IDataReader
	{
		public readonly DataBase DataBase;
		public readonly MySqlPCL.MySqlDataReader NativeReader;

		public DataReader(DataBase dataBase, MySqlPCL.MySqlDataReader nativeReader)
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
		}

		public object this[int ordinal]
		{
			get
			{
				return NativeReader[ordinal];
			}
		}

		public object this[string name]
		{
			get
			{
				return NativeReader[name];
			}
		}

		public int FieldCount
		{
			get
			{
				return NativeReader.FieldCount;
			}
		}

		public bool IsClosed
		{
			get
			{
				return NativeReader.IsClosed;
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
			NativeReader.Close();
		}

		public void Dispose()
		{
			NativeReader.Dispose();
		}

		public string GetDataTypeName(int ordinal)
		{
			return NativeReader.GetDataTypeName(ordinal);
		}

		public Type GetType(int ordinal)
		{
			return NativeReader.GetFieldType(ordinal);
		}

		public T GetFieldValue<T>(int ordinal)
		{
			return OKHOSTING.Data.Convert.ChangeType<T>(NativeReader.GetValue(ordinal));
		}

		public string GetName(int ordinal)
		{
			return NativeReader.GetName(ordinal);
		}

		public int GetOrdinal(string name)
		{
			return NativeReader.GetOrdinal(name);
		}

		public bool IsNull(int ordinal)
		{
			return NativeReader.IsDBNull(ordinal);
		}

		public bool NextResult()
		{
			return NativeReader.NextResult();
		}

		public bool Read()
		{
			return NativeReader.Read();
		}

		public IEnumerator GetEnumerator()
		{
			for (int i = 0; i < FieldCount; i++)
			{
				yield return NativeReader[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
