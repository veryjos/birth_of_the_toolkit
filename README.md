## Birth of the Toolkit

A CLI tool for decoding formats in Legend of Zelda: Breath of the Wild.

## Building
```bash
git submodule init
git submodule update
xbuild BoTK.sln
```

## Example
Encodings currently supported:
- yaz0
- byml

```bash
# Decoding a yaz0 compressed byml file
BoTK.exe --inputfile actors.byml --parserchain yaz0,byml --outputfile actors_decoded.byml
```

## License
MIT License
