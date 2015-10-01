using System;
using System.Collections.Generic;

namespace OKHOSTING.Sql
{
	/// <summary>
	/// Represents a collection of rows that are accesed one at a time, and must finally be closed, since its "conected" to the data source
	/// </summary>
	public interface IDataReader: IDataRow
	{
		/// <summary>
		/// Returns the list of columns that define this data result
		/// </summary>
		IEnumerable<DataColumn> Schema { get; }

		bool IsClosed { get; }

		//
		// Summary:
		//
		// Summary:
		//	 Obtiene el nombre del tipo de datos de la columna especificada.
		//
		// Parameters:
		//   ordinal:
		//	 Índice de columna de base cero.
		//
		// Returns:
		//	 Cadena que representa el nombre del tipo de datos.
		//
		// Exceptions:
		//   T:System.InvalidCastException:
		//	 La conversión especificada no es válida.
		string GetDataTypeName(int ordinal);

		Type GetType(int ordinal);

		//
		// Summary:
		//	 Obtiene el nombre de la columna a partir del índice de columna de base cero.
		//
		// Parameters:
		//   ordinal:
		//	 Índice de columna de base cero.
		//
		// Returns:
		//	 Nombre de la columna especificada.
		string GetName(int ordinal);

		//
		// Summary:
		//	 Obtiene el índice de columna a partir del nombre de la columna.
		//
		// Parameters:
		//   name:
		//	 Nombre de la columna.
		//
		// Returns:
		//	 Índice de columna de base cero.
		//
		// Exceptions:
		//   T:System.IndexOutOfRangeException:
		//	 El nombre especificado no es un nombre de columna válido.
		int GetOrdinal(string name);

		//
		// Summary:
		//	 Cuando se leen los resultados de un lote de instrucciones, desplaza el lector
		//	 hasta el resultado siguiente.
		//
		// Returns:
		//	 true si hay más conjuntos de resultados; en caso contrario, false.
		bool NextResult();
		
		//
		// Summary:
		//	 Avanza el lector al siguiente registro de un conjunto de resultados.
		//
		// Returns:
		//	 true si hay más filas; en caso contrario, false.
		bool Read();

		//
		// Summary:
		//	 Cierra el objeto System.Data.Common.DbDataReader.
		void Close();
	}
}