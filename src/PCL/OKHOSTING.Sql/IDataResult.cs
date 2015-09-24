using System;
using System.Collections.Generic;
using System.Linq;

namespace OKHOSTING.Sql
{
	/// <summary>
	/// The result of a query to a data source, contains a collection of rows with values
	/// </summary>
	public interface IDataResult: IEnumerable<IDataRow>, IDisposable
	{
		/// <summary>
		/// Returns the list of columns that define this data result
		/// </summary>
		Schema.Table Schema { get; }
	}
}