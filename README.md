<!-- the fork repo owner is stupid asf, borked main branch and gonna make alt to commit changes again -->
# ALittleNoahFpsFix
A fork of [KingKrouch's ALittleNoahFix](https://github.com/KingKrouch/ALittleNoahFix), but everything have been removed except framerate unlocker feature remains and improved a bit.

# Patches
* Cursor will always not being hidden (was borked in upstream repo)
* Framerate will no more locked max at 60 FPS
* Framerate will be lowed to 30 FPS if the game window is inactive (not focused, beta)

# Compile

Require .NET 6.0 or newer

```shell
# clone the repository and open the directory
git clone https://github.com/appleneko2001/ALittleNoahFpsFix
cd ALittleNoahFpsFix

# restore dependencies
dotnet restore

# build
dotnet build
```

# Usage

<!-- Install instructions backup:
Download the ALittleNoahFix.zip file from the GitHub releases section.
Extract the contents into the game's install directory.
If running on Linux or the Steam Deck, add
`WINEDLLOVERRIDES="winhttp=n,b" %command%`
to the game's launch parameters in the game's Steam properties.
-->

> [!IMPORTANT]
> Please compile the solution first. 
> 
> After compile should appear a folder `bin` under repository folder. Open `Debug`, `net6.0` and take out `ALittleNoahFix.dll` and `ALittleNoahFix.deps.json` (and `ALittleNoahFix.pdb` if you need to symbol debugging) for use later.

1. Follow "Install Instructions" first three steps: 
https://steamcommunity.com/app/1883260/discussions/0/4358998644635061533

2. Replace files in games folder `BepInEx/plugins` with files that you compiled before.
3. Launch the game and disable V-Sync settings in game. Its broken once you have applied this patch.
4. Optional. Change configuration file in `BepInEx/config/ALittleNoahFix.cfg` if necessary. Only VSync settings is usable since I have removed all features of upstream patches.

Once you complete those steps your game should have been framerate unlocked. You can also disable VSync in configuration file to let the game go higher framerate.