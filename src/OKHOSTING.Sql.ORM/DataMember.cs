using OKHOSTING.Core.Data.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OKHOSTING.Sql.ORM
{
	public class DataMember
	{
		public DataMember()
		{
		}

		public DataMember(DataType type, string member): this(type, member, null)
		{
		}

		public DataMember(DataType type, string member, Schema.Column column)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			if (string.IsNullOrWhiteSpace(member))
			{
				throw new ArgumentNullException("member");
			}

			DataType = type;
			Member = new MemberExpression(type.InnerType, member);

			if (column == null)
			{
				CreateColumn();
			}
			else
			{
				if (column.Table != type.Table)
				{
					throw new ArgumentOutOfRangeException("column", column, "This column does not belong the the Table that TypeMap is mapped to");
				}

				Column = column;
			}
		}

		/// <summary>
		/// The type map that contains this object
		/// </summary>
		public DataType DataType { get; set; }
		
		/// <summary>
		/// The database column where this field will be stored
		/// </summary>
		public Schema.Column Column { get; set; }

		/// <summary>
		/// String representing the member (property or field) that is being mapped
		/// </summary>
		public MemberExpression Member { get; set; }

		/// <summary>
		/// Conversions to apply when writing yo or reading from the database
		/// </summary>
		public Converters.ConverterBase Converter { get; set; }

		public object GetValueForColumn(object obj)
		{
			object value = Member.GetValue(obj);

			if (Converter != null)
			{
				value = Converter.MemberToColumn(value);
			}

			return value;
		}

		public void SetValueFromColumn(object obj, object value)
		{
			if (Converter != null)
			{
				value = Converter.ColumnToMember(value);
			}

			Member.SetValue(obj, value);
		}

		public void CreateColumn()
		{
			var finalMember = Member.FinalMemberInfo;

			Column = new Schema.Column()
			{
				Table = DataType.Table,
				Name = Member.Expression.Replace('.', '_'),
				DbType = Sql.DataBase.Parse(MemberExpression.GetReturnType(finalMember)),
				IsNullable = !Core.Data.Validation.RequiredValidator.IsRequired(finalMember),
				IsPrimaryKey = IsPrimaryKey(finalMember),
			};

			Column.IsAutoNumber = Column.IsNumeric && Column.IsPrimaryKey && DataType.BaseDataType == null;

			if (Column.IsString)
			{
				Column.Length = Core.Data.Validation.StringLengthValidator.GetMaxLenght(finalMember);
			}

			DataType.Table.Columns.Add(Column);
		}

		public override string ToString()
		{
			return Member.Expression;
		}

		public static bool IsPrimaryKey(System.Reflection.MemberInfo memberInfo)
		{
			return memberInfo.Name.ToString().ToLower() == "id" || memberInfo.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), false).Length > 0;
		}
	}

	public class DataMember<T> : DataMember
	{
		public DataMember(): base()
		{
			DataType = typeof(T);
		}

		public DataMember(string member): base(typeof(T), member)
		{
		}

		public DataMember(string member, Schema.Column column): base(typeof(T), member, column)
		{
		}

		public DataMember(System.Linq.Expressions.Expression<Func<T, object>> memberExpression): base(typeof(T), MemberExpression<T>.GetMemberString(memberExpression))
		{
		}

		public DataMember(System.Linq.Expressions.Expression<Func<T, object>> memberExpression, Schema.Column column): base(typeof(T), MemberExpression<T>.GetMemberString(memberExpression), column)
		{
		}

		public static DataMember<T> ToGeneric(DataMember memberExpression)
		{
			if (memberExpression is DataMember<T>)
			{
				return (DataMember<T>) memberExpression;
			}

			Type genericDataTypeType = typeof(DataMember<>).MakeGenericType(memberExpression.DataType.InnerType);

			ConstructorInfo constructor = genericDataTypeType.GetConstructor
			(
				System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance,
				null,
				null,
				null
			);

			DataMember<T> genericMember = (DataMember<T>) constructor.Invoke(null);
			genericMember.Column = memberExpression.Column;
			genericMember.Converter = memberExpression.Converter;

			return genericMember;
		}
	}
}