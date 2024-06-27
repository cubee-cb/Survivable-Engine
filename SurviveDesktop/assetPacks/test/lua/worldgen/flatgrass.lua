-- flatgrass generation routine

function Generate(x, y, w, h)

  for i = x, x + w do
    for iy = y, y + h do
      Plot(ix, iy, "*.grass")
    end
  end

  --PlaceTile(ix * 16, iy * 16, "*.rock")
  --Call("*.scatter_trees")

end