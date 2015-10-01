namespace OKHOSTING.Sql
{
	//
	// Summary:
	//	 Especifica el tipo de un parámetro dentro de una consulta relativa al System.Data.DataSet.
	public enum ParameterDirection
	{
		//
		// Summary:
		//	 Se trata de un parámetro de entrada.
		Input = 1,
		//
		// Summary:
		//	 Se trata de un parámetro de salida.
		Output = 2,
		//
		// Summary:
		//	 El parámetro puede ser de entrada o de salida.
		InputOutput = 3,
		//
		// Summary:
		//	 El parámetro representa un valor devuelto de una operación como un procedimiento
		//	 almacenado, una función integrada o una función definida por el usuario.
		ReturnValue = 6
	}
}
