-- test map generation routine

function Generate(x, y, w, h)
  x = x or 0
  y = y or 0
  w = w or 16
  h = h or 16

  for ix = x, x + w do
    for iy = y, y + h do
      tile = "*.ground.grass"
      elevation = 0

      -- create some simple slopes
      if ix == 4 and iy > 3 or ix == 10 then
        tile = "*.ground.grass_slope_horizontal"
      end
      if iy == 3 and ix < 4 then
        tile = "*.ground.grass_slope_vertical"
      end
      if ix > 4 or iy < 3 then
        elevation = 1
      end
      if ix > 10 then
        elevation = 3
      end

      -- place the tiles
      Plot(ix, iy, tile)
      SetElevation(ix, iy, elevation)

    end
  end

end

