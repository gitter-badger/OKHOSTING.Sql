using OKHOSTING.Core.Data;
using OKHOSTING.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.Linq;

namespace OKHOSTING.Sql.ORM
{
	/// <summary>
	/// Defines methods for serialization and deserialization of datatypes, datamembers and instances
	/// </summary>
	public static class Converter
	{
		/// <summary>
		/// Converts a DataType string representantion into an actual DataType instance
		/// </summary>
		/// <param name="value">
		/// Value to be converted to DataType
		/// </param>
		/// <returns>
		/// A DataType object deserialized from the string
		/// </returns>
		public static DataType ToDataType(string value)
		{
			//validate arguments
			if (string.IsNullOrWhiteSpace(value)) return null;

			//try to unparse as xml
			return Type.GetType(value, true, false);
		}

		/// <summary>
		/// Converts a DataMember string representantion into an actual DataMember instance
		/// </summary>
		/// <param name="value">
		/// Value to be converted to DataMember
		/// </param>
		/// <returns>
		/// A DataMember object deserialized from the string
		/// </returns>
		public static DataMember ToDataMember(string value)
		{
			//validate arguments
			if (string.IsNullOrWhiteSpace(value)) return null;

			//try to unparse as xml
			return (DataMember) ToIStringSerializable(value, typeof(DataMember));
		}
		
		/// <summary>
		/// Creates and populates a object based on a query string values
		/// </summary>
		/// <param name="value">
		/// Query string containing a serialized object
		/// </param>
		/// <returns>
		/// A populated object
		/// </returns>
		public static object ToObject(string value)
		{
			object dobj;
			DataType dtype;

			//Validating if the sting is null
			if (string.IsNullOrWhiteSpace(value)) return null;

			//deserialize datatype
			dtype = ToDataType(GetValueFromQueryString(value, "DataType"));

			//create new object
			dobj = object.From(dtype);

			//deserialize values
			((IStringSerializable)dobj).DeserializeFromString(value);

			return dobj;
		}

		/// <summary>
		/// Returns a object with all the DataValues
		/// whose values exists in the values collection
		/// </summary>
		/// <param name="dtype">
		/// DataType of the object whose DataValues will be loaded
		/// </param>
		/// <param name="valuesString">
		/// NameValueCollection with the next structure:
		/// 
		///		- Name: Name of the DataValue referenced by the item
		///		- Value: String representation of the value of referenced DataValue
		///		
		/// </param>
		/// <returns>
		/// object with all the DataValues
		/// of the specified DataType whose values exists in the values
		/// collection
		/// </returns>
		public static object ToObject(NameValueCollection values)
		{
			return ToObject(SerializeToString(values));
		}


		/// <summary>
		/// Converts a object to it's string representantion
		/// </summary>
		/// <param name="value">
		/// object to be converted to string
		/// </param>
		/// <returns>
		/// A string representation of object
		/// </returns>
		public static string ToString(object value)
		{
			if (value == null)
				return null;

			else if (value is DataType)
			{
				return ToString((DataType)value);
			}
			else if (value is DataMember)
			{
				return ToString((DataMember)value);
			}
			else if (DataType.IsMapped(value.GetType()))
			{
				DataType dtype = value.GetType();
				string result = string.Empty;

				foreach (DataMember member in dtype.AllDataMembers)
				{
					
				}
			}
			else
			{
				return Core.Data.Converter.ToString(value);
			}
		}

		/// <summary>
		/// Converts a DataType to it's string representantion
		/// </summary>
		/// <param name="value">
		/// DataType to be converted to string
		/// </param>
		/// <returns>
		/// A string representation of DataType
		/// </returns>
		public static string ToString(DataType value)
		{
			//null values
			if (value == null) return null;

			return value.InnerType.FullName;
		}

		/// <summary>
		/// Converts a DataMember to it's string representantion
		/// </summary>
		/// <param name="value">
		/// DataMember to be converted to string
		/// </param>
		/// <returns>
		/// A string representation of DataMember
		/// </returns>
		public static string ToString(DataMember value)
		{
			//null values
			if (value == null) return null;

			return "DataType=" + ToString(value.DataType) + "&DataMember=" + value.Member.Expression;
		}

		/// <summary>
		/// Creates a string representation of a List
		/// </summary>
		/// <param name="dataValuesInstances">List that will be parsed as a string</param>
		/// <returns>String containing all values in dataValuesInstances</returns>
		public static string ToString(List<DataMember> value)
		{
			string result = string.Empty;

			foreach (var member in value)
			{
				result += ToString(member) + '&';
			}

			return result.TrimEnd('&');
		}

		/// <summary>
		/// Creates a NameValueCollection filed with the atomized values of the object
		/// </summary>
		/// <param name="dobj">
		/// object which will be atomized and represented in the NameValueCollection
		/// </param>
		/// <returns>
		/// NameValueCollection filed with the atomized values of the List
		/// </returns>
		public static NameValueCollection ToNameValues(object value)
		{
			if (value == null) return null;

			NameValueCollection values = new NameValueCollection();

			values.Add("DataType", ((IStringSerializable)value.DataType).SerializeToString());
			values.Add(ToNameValues(value.PrimaryKey));

			return values;
		}

		/// <summary>
		/// Creates a NameValueCollection filed with the atomized values of the List
		/// </summary>
		/// <param name="dataValuesInstances">
		/// List which will be atomized and represented in the NameValueCollection
		/// </param>
		/// <returns>
		/// NameValueCollection filed with the atomized values of the List
		/// </returns>
		public static NameValueCollection ToNameValues(List<DataMember> values, object instance)
		{
			//Validating if the dataProperties argument is null
			if (values == null) return null;

			//Creating the result
			NameValueCollection result = new NameValueCollection();

			//Crossing the DataValues candidates to load
			foreach (DataMember member in values)
			{
				object val = member.Member.GetValue(instance);

				//Validating if the value was specified on dicResult collection
				if (Core.Data.Validation.RequiredValidator.HasValue(val))
				{
					result.Add(member.Member.Expression, Converter.ToString(val));
				}
			}

			//Returning the NameValueCollection
			return result;
		}

		/// <summary>
		/// Populates a List&lt;MemberValue&gt; with the atomized fields in queryString
		/// </summary>
		/// <param name="queryString">
		/// QueryString that contains atomized names and values of DataValues
		/// </param>
		/// <param name="instances">
		/// Collection that will be populated from queryString
		/// </param>
		public static void ToDataValueInstances(string queryString, List<DataMemberValue> instances)
		{
			//Validating if the dataProperties argument is null
			if (string.IsNullOrWhiteSpace(queryString)) return;
			if (instances == null) throw new ArgumentNullException("instances");

			NameValueCollection nameValues = ToNameValues(queryString);

			//populating instances
			ToDataValueInstances(nameValues, instances);
		}

		/// <summary>
		/// Populates a List&lt;MemberValue&gt; with the atomized fields in values
		/// </summary>
		/// <param name="values">
		/// Collection that contains atomazed names and values of DataValues
		/// </param>
		/// <param name="instances">
		/// Collection that will be populated from values
		/// </param>
		public static void ToDataValueInstances(NameValueCollection values, List<DataMemberValue> instances)
		{
			//Validating if the dataProperties argument is null
			if (values == null) return;
			if (instances == null) throw new ArgumentNullException("instances");

			//copy values from values to atomized
			foreach (DataMemberValue dvi in instances)
			{
				//Validating if the value was specified on dicResult collection
				if (Core.Data.Validation.RequiredValidator.HasValue(values[dvi.DataMember.Member.Expression]))
				{
					//Adding the value to the collection
					dvi.Value = Converter.ChangeType(values[dvi.DataMember.Member.Expression], dvi.DataMember.Member.ReturnType);
				}
			}
		}
	}
}