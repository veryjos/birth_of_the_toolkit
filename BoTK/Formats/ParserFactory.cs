using BoTK.Formats.Parsers;

namespace BoTK.Formats {
  public static class ParserFactory {
    public static Parser CreateFromName(string name) {
      name = name.ToLower();

      if (name == BYMLParser.GetName())
        return new BYMLParser();

      if (name == Yaz0Parser.GetName())
        return new Yaz0Parser();

      return null;
    }
  }
}