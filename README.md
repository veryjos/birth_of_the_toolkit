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

Archives currently supported:
- sarc

```bash
# Decoding a yaz0 compressed byml file
BoTK.exe --inputfile actors.byml --parserchain yaz0,byml --outputfile actors_decoded.byml

# Decoding a yaz0 compressed sarc archive to stdout
BoTK.exe --inputfile 56000009C8.hght.sstera --parserchain yaz0,sarc
```

## License
MIT License
