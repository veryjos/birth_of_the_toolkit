using System;
using System.Text;

namespace BoTK.Entities {
  public class Magic {
    private byte[] m_bytes;

    public Magic(string asciiStr) {
      m_bytes = Encoding.ASCII.GetBytes(asciiStr);
    }

    public Magic(byte[] bytes) {
      m_bytes = bytes;
    }

    public static bool operator ==(Magic a, Magic b) {
      return a.Equals(b);
    }

    public static bool operator !=(Magic a, Magic b) {
      return !(a.Equals(b));
    }

    public override bool Equals(Object other) {
      return Equals(other as Magic);
    }

    public bool Equals(Magic other) {
      var minLen = Math.Min(other.m_bytes.Length, m_bytes.Length);

      for (var i = 0; i < minLen; ++i) {
        if (m_bytes[i] != other.m_bytes[i])
          return false;
      }

      return true;
    }

    public string AsASCII() {
      return Encoding.ASCII.GetString(m_bytes);
    }

  }
}