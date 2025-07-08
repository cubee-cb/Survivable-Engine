-- test map generation routine

function Generate(x, y, w, h)
  x = x or 0
  y = y or 0
  w = w or 16
  h = h or 16

  for ix = x, x + w do
    for iy = y, y + h do
      local tile = "@ground.grass"
      local elevation = 0

      -- create some simple slopes
      if ix == 8 and iy > 7 or ix == 14 then
        tile = "@ground.grass_slope_horizontal"
      end
      if iy == 7 and ix < 8 then
        tile = "@ground.grass_slope_vertical"
      end
      if ix > 8 or iy < 7 then
        elevation = 1
      end
      if ix > 14 then
        elevation = 3
      end

      -- place the tiles
      Plot(ix, iy, tile)
      SetElevation(ix, iy, elevation)

    end
  end

  -- make a little raised area for collision testing
  for ix = 1, 2 do
    for iy = 10, 14 do
      SetElevation(ix, iy, 1)
    end
  end

end

