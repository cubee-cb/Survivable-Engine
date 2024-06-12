-- test worldgen routine

function Generate(width, height)
  --CreateMap(width, height)

  --FillGround(tileName, height, deviance)
  -- Covers the ground with a tile. If tiles already exist in the way, replaces them.
  -- tileName - ground tile to cover the map with.
  -- height - height to start filling at. lowest height when deviance is non-zero.
  -- deviance - controls the maximum height of the ground, 0 for fully flat.
  FillGround("ground_grass", 1)

  --DistributeTile(tileName, portion, x, y, w, h)
  -- Scatters tiles across an area.
  -- tileName - name of the tile to scatter.
  -- portion - percentage of ground to cover, approximately.
  -- x, y, w, h (optional) - area to spread within. omit to cover the whole map.
  DistributeTile("tile_tree", 30)
  DistributeTile("tile_rock", 20)
  DistributeTile("tile_flower", 10)
  DistributeTile("tile_grass", 40)

  --CreatePortal(portalName, worldTagTo, portalTag)
  -- Creates a portal that can take the player between worlds.
  -- portalName - name of the portal object to spawn.
  -- worldTagTo - tag of the world to go to. "here" for same world.
  -- portalTag - specific portal within the world to link to.

end
