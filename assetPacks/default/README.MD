# Default pack

## asset packs

for all intents and purposes, a pack's `nameSpace` is it's internal identifier. `internal name` is intended only for translation strings.

### games

note that `game.json` is a subclass of `pack.json`. it contains the same fields, plus some more specific to game packs.

## subfolders

the subfolders are arbitrary, and are intended for organising assets. if you don't care about that, just throw everything in an `assets` folder.

standard folders are the following:
- `common`
- `character`
- `ground`
- `tile`
- `dimension`
- `biome`
- `item`
- `mob`
- `worldgen`
- `font`
- `ui`
