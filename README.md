# Survivable Engine (placeholder name)

An object-oriented rebuild of [Everlost Isle](https://cubee.games/?rel=games&sub=everlost_isle)'s engine, which was originally internally known as Survive. 
Built on Monogame 3.8.1 in Visual Studio 2022. Uses [Newtonsoft.Json](https://www.newtonsoft.com/json) for Json and [MoonSharp](https://www.moonsharp.org/) for Lua scripting.

Everlost Isle is intended to become a top-down survival game, largely inspired by Terraria's progression style and Stardew Valley's world structure and perspective, though this time I intend to build the engine separately from the game's content in a way that can allow easy modding through Lua and Json files, with potentially entirely new games able to be built on it.

I've decided to put my early experiments up here for people to see what I'm up to, and if I do something stupid or miss something obvious, hopefully someone can help me out :)

So far, I've:
- Set up an Instance system to have multiple game clients run in the same executable, and run as different modes like Server or Client. Potentially useful for split-screen.
- Set up a ticking system, so the update rate of each instance can be modified and the game's framerate can be unlocked without adversely affecting the game loop.
- Made a runtime-loading asset system called Warehouse, which lets me load Json, Lua, and PNG files at runtime for loading mods.
- Added an Entity/Mob system that can somewhat be controlled through basic Lua scripting and inherits properties from Json files.
- Made a simple world generator system that uses a List of Lua routines to build a world.
- Made entities smooth their visual position between ticks, if the framerate is higher than the tickrate.

Notable milestones to-do:
- Add dynamic tiles.
	- Don't-Starve-like "tile entities" that are solid and don't move, but are also not locked to the grid like normal tiles.
- Collisions between entities and tile entities.
- Inventories and items.
- Saving worlds and players.
	- Should players be per-world as before like Stardew, or separate like Terraria?
- Multiple worlds/"dimensions" per-save with their own mobs and world properties.
	- "Portals" with destination world/position. (may be doorways, caves, or literal portals)
	- Mob spawns, biome generators, etc.
- Networking/splitscreen/multiplayer?
	- Actual differences between host/client/dedicated.
	- Tilemap/entity position syncing.
- Game progression triggers.
	- May be step-by-step or simple boolean.
	- May have dependencies before availability. May optionally need to be fulfilled specifically after being made available, or can be completed beforehand without applying effects until later.


Controls:
- Arrow Keys / ESDF - Move
- Left Shift - Run
- J, K, L - Adjust tickrate (/8, x8, x16)
- Left Alt - (while held) Print tick stats to the console.

Controls (Hold Right CTRL)
- U - Unload all asset packs
- I Load all asset packs

