<div align="center">

**Cryptic Resolver in C#**


[![word-count](https://img.shields.io/badge/Keywords%20Inlcuded-437-brightgreen)](#default-sheets)
[![GitHub version](https://badge.fury.io/gh/cryptic-resolver%2Fcr_Go.svg)](https://badge.fury.io/gh/cryptic-resolver%2Fcr_Go)

</div>

This command line tool `cr` is used to **record and explain cryptic commands, acronyms and so forth** in daily life.
The effort is to study etymology and know of naming conventions.

Not only can it be used in the computer filed, but also you can use this to manage your own knowledge base easily.

- Currently we have **437** keywords explained in our default sheets.

<br>


<a name="default-sheets"></a> 
## Default Sheets

- [cryptic_computer]
- [cryptic_common]
- [cryptic_science]
- [cryptic_economy]
- [cryptic_medicine]

<br>


## Install

The binary is very huge(12MB) since it includes Runtime, so I'm not going to provide a exe file to install. You can clone this repo and use `build.ps1` to generate binaries for yourself.


<br>

## Why

The aim of this project is to:

1. make cryptic things clear
2. help maintain your own personal knowledge base

rather than

1. record the use of a command, for this you can refer to [tldr], [cheat] and so on. 

<br>

# Usage

```bash
$ cr emacs
# -> Emacs: Edit macros
# ->
# ->   a feature-rich editor
# ->
# -> SEE ALSO Vim 

$ cr -u 
# -> update all sheets

$ cr -u https://github.com/ccmywish/ruby_things.git
# -> Add your own knowledge base! 

$ cr -h
# -> show help
```


<br>

# Implementation

`cr` is written in pure **C#**. You can implement this tool in any other language you like(name your projects as `cr_Python` for example), just remember to reuse our [cryptic_computer] or other sheets which are the core parts anyone can contribute to.

## Sheet layout

Sheet is the knowledgebase. Every sheet should be a git repository. And each should contain these files(we call these dictionarys):

1. 0123456789.toml
2. a.toml
3. b.toml
3. ...
4. z.toml

## Dictionary format(File format)

In every file(or dictionary), your definition format looks like this in pure **toml**:
```toml
# A normal definition
#
# NOTICE: 
#   We MUST keep the key downcase
#   We use a key 'disp' to display its original form 
#   Because the case sometimes contains details to help we understand
#
#   And 'disp' && 'desc' is both MUST-HAVE. 
#   But if you use 'same', all other infos are not needed.   
#
[xdg]
disp = "XDG"
desc = "Cross Desktop Group"

# If you want to explain more, use 'full'
[xxd]
disp = "xxd"
desc = "hex file dump"
full = "Why call this 'xxd' rather than 'xd'?? Maybe a historical reason"

# If there are multiple meanings, you should add a subkey to differ
[xdm.Download]
disp = "XDM"
desc = "eXtreme Download Manager"

[xdm.Display]
disp = "XDM"
desc = "X Display Manager"
```

We have more features than above
```toml
[jpeg]
disp = "JPEG"
desc = "Joint Photographic Experts Group"
full = "Introduced in 1992. A commonly used method of lossy compression for digital images"
see = ['MPG','PNG'] # This is a `see also`

[jpg]
same = "JPEG" # We just need to redirect this. No duplicate!

[sth]
same = "xdm" # If we direct to a multimeaning word, we don't need to specify its category(subkey).

["h.265"]
disp = "H.265"
desc = "A video compression standard" # The 'dot' keyword supported using quoted strings

```

## Name collision

In one sheet, you should consider adding a subkey to differ each other like the example above.

*But what if a sheet has 'gdm' while another also has a 'GDM'?*

> That's nothing, because cr knows this.

*But what if a sheet has two 'gdm'?* 

> This will lead to toml's parser library fail. You have these solutions
> 1. Use a better lint for example: [VSCode's Even Better TOML](https://github.com/tamasfe/taplo)
> 2. Watch the fail message, you may notice 'override path xxx', the xxx is the collision, you may correct it back manually.


<br>

## cr in C# development

This is built in .NET 6.0

maybe you need `sudo` access

How to manage dependecy:

```bash
# control `cr.csproj`
# For example
dotnet add package Pastel
dotnet remove pacakge Pastel
```

**Notice**: There's a bug in the dependecy `Carbon.Toml`, it can't parse `''` string in `SEE` key, for example you can see the word `mop`'s `see(see also)` in `m.toml` in the `cryptic_computer` sheet. I think this is because
1. the lib is only compatible with an old-version TOML language spec
2. it's a bug the lib author doesn't notice at present

- `dotnet new console`
- `dotnet run -- emacs`



<br>

# LICENSE
`cr` itself is under MIT

Official [default sheets](#default-sheets) are all under CC-BY-4.0


[cryptic_computer]: https://github.com/cryptic-resolver/cryptic_computer
[cryptic_common]: https://github.com/cryptic-resolver/cryptic_common
[cryptic_science]: https://github.com/cryptic-resolver/cryptic_science
[cryptic_economy]: https://github.com/cryptic-resolver/cryptic_economy
[cryptic_medicine]: https://github.com/cryptic-resolver/cryptic_medicine
[tldr]: https://github.com/tldr-pages/tldr
[cheat]: https://github.com/cheat/cheat
