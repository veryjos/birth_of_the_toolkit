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
# Decoding a yaz0 compressed byml file to stdout
mono BoTK.exe --input actors.byml

# Recursively decoding a yaz0 compressed sarc archive to a folder
mono BoTK.exe --input 56000009C8.hght.sstera --output 56000009C8
```

## License
MIT License
