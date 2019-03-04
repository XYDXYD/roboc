using System.Text;

namespace LiteNetLibExt
{
	public static class Common
	{
		public const int FooterSize = 4;

		public static string ByteArrayToString(byte[] ba, int offset, int length)
		{
			StringBuilder stringBuilder = new StringBuilder(length * 2);
			for (int i = offset; i < offset + length; i++)
			{
				stringBuilder.AppendFormat("{0:x2}", ba[i]);
			}
			return stringBuilder.ToString();
		}
	}
}
