using System;
using System.Collections.Generic;

namespace OKHOSTING.Sql
{
	/// <summary>
	/// Represents a read only data table that allows free access to all its rows and values, and is considered to be offline or "disconected" from the data source
	/// </summary>
	public interface IDataTable: IReadOnlyList<IDataRow>, IDisposable
	{
		/// <summary>
		/// Returns the list of columns that define this data result
		/// </summary>
		IEnumerable<DataColumn> Schema { get; }

		//
		// Summary:
		//	 Obtiene o establece el nombre de System.Data.DataTable.
		//
		// Returns:
		//	 Nombre del objeto System.Data.DataTable.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//	 Se pasa un valor null o una cadena vacía ("") y esta tabla pertenece a una colección.
		//
		//   T:System.Data.DuplicateNameException:
		//	 La tabla pertenece a una colección que ya contiene una tabla con el mismo nombre.
		//	 (La comparación distingue entre mayúsculas de minúsculas.)
		string Name { get; set; }
	}
}