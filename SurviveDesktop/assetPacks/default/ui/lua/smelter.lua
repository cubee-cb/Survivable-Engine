-- test smelter inventory

local t = 1





--[[ called every tick
function Tick(self)
  local inputSlot = GetSlot(1)
  local outputSlot = GetSlot(2)
  local fuelSlot = GetSlot(3)
  if not fuelSlot.empty and not inputSlot.empty then
    AddItem(fuel, fuelSlot.id, AMOUNT, -1)

  end



end
--]]
