-- flatgrass generation routine

function Generate(x, y, w, h)
  x = x or 0
  y = y or 0
  w = w or 16
  h = h or 16

  for ix = x, x + w do
    for iy = y, y + h do
      Plot(ix, iy, "@ground.grass")

    end
  end

  -- place a ground tile at tile coords.
  --Plot(ix, iy, "@grass")

  -- set the elevation of an existing tile at the tile coords.
  --SetElevation(ix, iy, elevation)

  --todo / place a tile entity on the ground at tile coords. accepts decimal positions, snaps to pixels
  --PlaceTile(ix, iy, "@rock")

  --todo / use to call the Generate function from other lua files
  --Call("@scatter_trees")

end
