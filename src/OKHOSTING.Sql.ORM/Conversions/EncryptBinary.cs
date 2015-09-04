namespace OKHOSTING.Sql.ORM.Conversions
{
	public class EncryptBinary : ConverterBase<byte[], byte[]>
	{
		public readonly System.Security.SecureString Password;

		public EncryptBinary(string password)
		{
			Password = OKHOSTING.Core.Cryptography.SimpleEncryption.ToSecureString(password);
		}

		public EncryptBinary(System.Security.SecureString password)
		{
			Password = password;
		}

		public override byte[] MemberToColumn(byte[] memberValue)
		{
			return OKHOSTING.Core.Cryptography.SimpleEncryption.Encrypt(memberValue, Password);
		}

		public override byte[] ColumnToMember(byte[] columnValue)
		{
			return OKHOSTING.Core.Cryptography.SimpleEncryption.Decrypt(columnValue, Password);
		}

		public override object MemberToColumn(object memberValue)
		{
			return MemberToColumn((byte[]) memberValue);
		}

		public override object ColumnToMember(object columnValue)
		{
			return ColumnToMember((byte[]) columnValue);
		}
	}
}