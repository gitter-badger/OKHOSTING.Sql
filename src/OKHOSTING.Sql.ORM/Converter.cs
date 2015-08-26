using OKHOSTING.Core.Data;
using OKHOSTING.Core.Extensions;
using OKHOSTING.Sql.ORM.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OKHOSTING.Sql.ORM
{
	/// <summary>
	/// Defines methods for converting objects from one Type to another,
	/// as well as serialization and deserialization methods
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
			return (DataType) ToIStringSerializable(value, typeof(DataType));
		}

		/// <summary>
		/// Returns a DataType created by deserializing an xml element
		/// </summary>
		/// <param name="xml">
		/// XmlElement containing a serialized DataType
		/// </param>
		/// <returns>
		/// A deserialized DataType
		/// </returns>
		public static DataType ToDataType(XmlElement xml)
		{
			DataType dtype;

			//Validating if the argumetn is null
			if (xml == null) return null;
			if (string.IsNullOrWhiteSpace(xml.InnerXml)) return null;

			//retrieve DataType
			dtype = System.Type.GetType(xml.GetAttribute("InnerType"));

			//deserialize
			return dtype;
		}

		/// <summary>
		/// Returns a DataType created by deserializing an XElement
		/// </summary>
		/// <param name="xml">
		/// XElement containing a serialized DataType
		/// </param>
		/// <returns>
		/// A deserialized DataType
		/// </returns>
		public static DataType ToDataType(XElement xml)
		{
			return ToDataType(xml.ToXmlElement());
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
		/// Returns a DataMember created by deserializing an xml element
		/// </summary>
		/// <param name="xml">
		/// XmlElement containing a serialized DataMember
		/// </param>
		/// <returns>
		/// A deserialized DataMember
		/// </returns>
		public static DataMember ToDataMember(XmlElement xml)
		{
			DataType dtype;
			string name;

			//Validating if the argumetn is null
			if (xml == null) return null;
			if (string.IsNullOrWhiteSpace(xml.InnerXml)) return null;

			//retrieve DataType
			dtype = DataType.Parse(xml.GetAttribute("DataType"));

			//retrieve DataMemeber's name
			name = xml.GetAttribute("Name");

			//return DataMember
			return dtype[name];
		}

		/// <summary>
		/// Returns a DataMember created by deserializing an XElement
		/// </summary>
		/// <param name="xml">
		/// XElement containing a serialized DataMember
		/// </param>
		/// <returns>
		/// A deserialized DataMember
		/// </returns>
		public static DataMember ToDataMember(XElement xml)
		{
			return ToDataMember(xml.ToXmlElement());
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
		/// Returns a object populated by deserializing an xml element
		/// </summary>
		/// <param name="xml">
		/// XmlElement containing a serialized object
		/// </param>
		/// <returns>
		/// A populated object
		/// </returns>
		public static object ToObject(XmlElement xml)
		{
			DataType dtype;

			//Validating if the argumetn is null
			if (xml == null) return null;
			if (string.IsNullOrWhiteSpace(xml.InnerXml)) return null;

			//retrieve DataType
			dtype = DataType.Parse(xml.GetAttribute("DataType"));

			//deserialize
			return (object) ToIXmlSerializable(xml.OuterXml, dtype.InnerType);
		}

		/// <summary>
		/// Returns a object populated by deserializing an XElement
		/// </summary>
		/// <param name="xml">
		/// XElement containing a serialized object
		/// </param>
		/// <returns>
		/// A populated object
		/// </returns>
		public static object ToObject(XElement xml)
		{
			return ToObject(xml.ToXmlElement());
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

			return ((OKHOSTING.Core.Data.IStringSerializable) value).SerializeToString();
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

			return ((IStringSerializable)value).SerializeToString();
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
			//null values
			if (value == null) return null;

			return ((IStringSerializable)value).SerializeToString();
		}

		/// <summary>
		/// Creates a string representation of a List
		/// </summary>
		/// <param name="dataValuesInstances">List that will be parsed as a string</param>
		/// <returns>String containing all values in dataValuesInstances</returns>
		public static string ToString(List<MemberValue> value)
		{
			//First, convert to NameValueCollection
			NameValueCollection nameValues = ToNameValues(value);

			//Now convert to string
			return SerializeToString(nameValues);
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
		public static NameValueCollection ToNameValues(List<MemberValue> values)
		{
			//Validating if the dataProperties argument is null
			if (values == null) return null;

			//Creating the result
			NameValueCollection result = new NameValueCollection();

			//Crossing the DataValues candidates to load
			foreach (MemberValue dvi in values)
			{
				//Validating if the value was specified on dicResult collection
				if (Core.Data.Validation.RequiredValidator.HasValue((dvi.Value)))
				{
					//Adding the value to the collection
					result.Add(dvi.DataMember.Member.Expression, Converter.SerializeToString(dvi.Value));
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
		public static void ToDataValueInstances(string queryString, List<MemberValue> instances)
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
		public static void ToDataValueInstances(NameValueCollection values, List<MemberValue> instances)
		{
			//Validating if the dataProperties argument is null
			if (values == null) return;
			if (instances == null) throw new ArgumentNullException("instances");

			//copy values from values to atomized
			foreach (MemberValue dvi in instances)
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