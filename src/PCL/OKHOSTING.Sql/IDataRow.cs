using System;
using System.Collections;

namespace OKHOSTING.Sql
{
	/// <summary>
	/// Represents a row of data, with multiple columns
	/// </summary>
	public interface IDataRow: IEnumerable, IDisposable
	{
		//
		// Summary:
		//	 Obtiene el valor de la columna especificada como una instancia de System.Object.
		//
		// Parameters:
		//   name:
		//	 Nombre de la columna.
		//
		// Returns:
		//	 Valor de la columna especificada.
		//
		// Exceptions:
		//   T:System.IndexOutOfRangeException:
		//	 No se ha encontrado la columna con el nombre especificado.
		object this[string name] { get; }

		//
		// Summary:
		//	 Obtiene el valor de la columna especificada como una instancia de System.Object.
		//
		// Parameters:
		//   ordinal:
		//	 Índice de columna de base cero.
		//
		// Returns:
		//	 Valor de la columna especificada.
		//
		// Exceptions:
		//   T:System.IndexOutOfRangeException:
		//	 El índice que se ha pasado se encontraba fuera del intervalo de 0 a System.Data.IDataRecord.FieldCount.
		object this[int ordinal] { get; }

		//
		// Summary:
		//	 Obtiene de forma sincrónica el valor de la columna especificada como un tipo.
		//
		// Parameters:
		//   ordinal:
		//	 Columna que va a recuperarse.
		//
		// Type parameters:
		//   T:
		//	 Obtiene de forma sincrónica el valor de la columna especificada como un tipo.
		//
		// Returns:
		//	 Columna que va a recuperarse.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//	 La conexión se interrumpe o se cierra durante la recuperación de datos.System.Data.SqlClient.SqlDataReader
		//	 se cierra durante la recuperación de datos.No hay ningún dato listo para leer
		//	 (por ejemplo, no se ha llamado al primer System.Data.SqlClient.SqlDataReader.Read
		//	 o ha devuelto false).Se intentó leer una columna leída previamente en modo secuencial.Había
		//	 una operación asincrónica en curso.Esto se aplica a todos los métodos Get* cuando
		//	 se ejecutan en modo secuencial, ya que se les podía llamar mientras se leía una
		//	 secuencia.
		//
		//   T:System.IndexOutOfRangeException:
		//	 Se intentó leer una columna que no existe.
		//
		//   T:System.InvalidCastException:
		//	 T no coincide con el tipo devuelto por SQL Server o no se puede convertir.
		T GetFieldValue<T>(int ordinal);

		//
		// Summary:
		//	 Obtiene un valor que indica si la columna contiene valores que no existen o faltan
		//	 valores.
		//
		// Parameters:
		//   ordinal:
		//	 Índice de columna de base cero.
		//
		// Returns:
		//	 true si la columna especificada equivale a System.DBNull; de lo contrario, false.
		bool IsNull(int ordinal);

		//
		// Summary:
		//	 Obtiene el número de columnas de la fila actual.
		//
		// Returns:
		//	 Número de columnas de la fila actual.
		//
		// Exceptions:
		//   T:System.NotSupportedException:
		//	 No hay ninguna conexión a una instancia de SQL Server.
		int FieldCount { get; }
	}
}