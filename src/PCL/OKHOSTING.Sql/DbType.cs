namespace OKHOSTING.Sql
{
	//
	// Summary:
	//	 Especifica el tipo de datos de un campo, una propiedad o un objeto Parameter
	//	 de un proveedor de datos de .NET Framework.
	/// <summary>
	/// Created just for compatibility in PCLs since this DbType does not exist here
	/// </summary>
	public enum DbType
	{
		//
		// Summary:
		//	 Secuencia de longitud variable de caracteres no Unicode comprendida entre 1 y
		//	 8.000 caracteres.
		AnsiString = 0,
		//
		// Summary:
		//	 Secuencia de longitud variable de datos binarios comprendida entre 1 y 8.000
		//	 bytes.
		Binary = 1,
		//
		// Summary:
		//	 Entero de 8 bits sin signo cuyo valor está comprendido entre 0 y 255.
		Byte = 2,
		//
		// Summary:
		//	 Tipo simple que representa los valores booleanos true o false.
		Boolean = 3,
		//
		// Summary:
		//	 Valor de moneda comprendido entre -2 63 (o -922,337,203,685,477.5808) y 2 63
		//	 -1 (o +922,337,203,685,477.5807), con una precisión de una diezmilésima de unidad
		//	 de moneda.
		Currency = 4,
		//
		// Summary:
		//	 Tipo que representa un valor de fecha.
		Date = 5,
		//
		// Summary:
		//	 Tipo que representa un valor de fecha y hora.
		DateTime = 6,
		//
		// Summary:
		//	 Tipo simple que representa los valores comprendidos entre 1,0 x 10 -28 y aproximadamente
		//	 7,9 x 10 28, con 28-29 dígitos significativos.
		Decimal = 7,
		//
		// Summary:
		//	 Tipo de punto flotante que representa los valores comprendidos entre aproximadamente
		//	 5,0 x 10 -324 y 1,7 x 10 308, con una precisión de 15-16 dígitos.
		Double = 8,
		//
		// Summary:
		//	 Identificador único global (GUID).
		Guid = 9,
		//
		// Summary:
		//	 Tipo entero que representa enteros de 16 bits con signo con valores comprendidos
		//	 entre -32768 y 32767.
		Int16 = 10,
		//
		// Summary:
		//	 Tipo entero que representa enteros de 32 bits con signo con valores comprendidos
		//	 entre -2147483648 y 2147483647.
		Int32 = 11,
		//
		// Summary:
		//	 Tipo entero que representa enteros de 64 bits con signo con valores comprendidos
		//	 entre -9223372036854775808 y 9223372036854775807.
		Int64 = 12,
		//
		// Summary:
		//	 Tipo general que representa cualquier tipo de valor o referencia no representado
		//	 de forma explícita por otro valor DbType.
		Object = 13,
		//
		// Summary:
		//	 Tipo entero que representa enteros de 8 bits con signo con valores comprendidos
		//	 entre -128 y 127.
		SByte = 14,
		//
		// Summary:
		//	 Tipo de punto flotante que representa los valores comprendidos entre aproximadamente
		//	 1,5 x 10 -45 y 3,4 x 10 38, con una precisión de 7 dígitos.
		Single = 15,
		//
		// Summary:
		//	 Tipo que representa cadenas de caracteres Unicode.
		String = 16,
		//
		// Summary:
		//	 Tipo que representa un valor DateTime de SQL Server.Si desea utilizar un valor
		//	 time de SQL Server, use SqlDbType.Time.
		Time = 17,
		//
		// Summary:
		//	 Tipo entero que representa enteros de 16 bits sin signo con valores comprendidos
		//	 entre 0 y 65535.
		UInt16 = 18,
		//
		// Summary:
		//	 Tipo entero que representa enteros de 32 bits sin signo con valores comprendidos
		//	 entre 0 y 4294967295.
		UInt32 = 19,
		//
		// Summary:
		//	 Tipo entero que representa enteros de 64 bits sin signo con valores comprendidos
		//	 entre 0 y 18446744073709551615.
		UInt64 = 20,
		//
		// Summary:
		//	 Valor numérico de longitud variable.
		VarNumeric = 21,
		//
		// Summary:
		//	 Secuencia de longitud fija de caracteres no Unicode.
		AnsiStringFixedLength = 22,
		//
		// Summary:
		//	 Cadena de longitud fija de caracteres Unicode.
		StringFixedLength = 23,
		//
		// Summary:
		//	 Representación analizada de un documento o fragmento XML.
		Xml = 25,
		//
		// Summary:
		//	 Datos de fecha y hora.El intervalo de valores de fecha comprende desde el 1 de
		//	 enero de 1 d.C. hasta el 31 de diciembre de 9999 d.C.El intervalo de valor horario
		//	 está comprendido entre 00:00:00 y 23:59:59,9999999 con una precisión de 100 nanosegundos.
		DateTime2 = 26,
		//
		// Summary:
		//	 Datos de fecha y hora con conocimiento de la zona horaria.El intervalo de valores
		//	 de fecha comprende desde el 1 de enero de 1 d.C. hasta el 31 de diciembre de
		//	 9999 d.C.El intervalo de valor horario está comprendido entre 00:00:00 y 23:59:59,9999999
		//	 con una precisión de 100 nanosegundos.El intervalo horario es -14: 00 hasta +14:00.
		DateTimeOffset = 27
	}
}