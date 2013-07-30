using System.Text;

namespace Q42.DbTranslations.Services
{
  public static class EncodingDetection
  {
    /// <summary>
    /// Detects the byte order mark of a file and returns
    /// an appropriate encoding for the file.
    /// </summary>
    /// <param name="buffer">A byte array representation of the file for which to detect encoding</param>
    /// <returns>The encoding</returns>
    public static Encoding GetEncoding(this byte[] buffer)
    {
      // *** Use UTF8 by default
      Encoding enc = Encoding.UTF8;

      if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
        enc = Encoding.UTF8;
      else if (buffer[0] == 0xfe && buffer[1] == 0xff)
        enc = Encoding.Unicode;
      else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
        enc = Encoding.UTF32;
      else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
        enc = Encoding.UTF7;

      return enc;
    }

    public static string ToStringUsingEncoding(this byte[] buffer)
    {
      return GetEncoding(buffer).GetString(buffer);
    }
  }
}