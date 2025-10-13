# Brinedump
A mod for [Caves of Qud](http://www.cavesofqud.com/) to extract various configurations of junk for wiki assimilation.

Tested on game builds 2.0.210.24 and 2.0.211.7.

[Use this commit for builds older than 2.0.210.](https://github.com/TrashMonks/brinedump/tree/2aa9ab8526d3b6ee68fccb93d1d57656c20fd7ab) Simultaneous support for both the newest game versions and versions older than that is infeasible due to the lack of conditional dependencies before then.

## Installation - [Wiki](https://wiki.cavesofqud.com/wiki/Modding:Installing_a_mod)
Place (or create a symbolic link to) the Mod directory in your Caves of Qud Mods folder.
##### Windows
`%USERPROFILE%\AppData\LocalLow\Freehold Games\CavesOfQud\Mods`
##### macOS
`~/Library/Application Support/com.FreeholdGames.CavesOfQud/Mods`
##### Linux
`~/.config/unity3d/Freehold Games/CavesOfQud/Mods`

## Usage
Bind and use your `wish` key in game and enter any of the following  
`brinedump:gameobjects` - Dump information about GameObjects.  
`brinedump:textures` - Dump all game textures.  

The output will be placed in the "Trash" folder.
## Files
`.../CavesOfQud/Brinedump.json` - Configuration file for output paths.  
`.../CavesOfQud/Brinedump.log` - Log of export results.  
