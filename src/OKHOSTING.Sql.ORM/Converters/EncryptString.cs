using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Sql.ORM.Converters
{
	public class EncryptString: ConverterBase<string, string>
	{
		public readonly System.Security.SecureString Password;

		public EncryptString(string password)
		{
			Password = OKHOSTING.Core.Cryptography.SimpleEncryption.ToSecureString(password);
		}

		public EncryptString(System.Security.SecureString password)
		{
			Password = password;
		}

		public override string MemberToColumn(string memberValue)
		{
			return OKHOSTING.Core.Cryptography.SimpleEncryption.Encrypt(memberValue, Password);
		}

		public override string ColumnToMember(string columnValue)
		{
			return OKHOSTING.Core.Cryptography.SimpleEncryption.Decrypt(columnValue, Password);
		}
	}
}