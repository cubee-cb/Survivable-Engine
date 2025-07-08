-- basic inventory
-- by cubee

local t = 1





--[[ called every tick
function Tick(self)

  local gearSlots = GetSlotsByName("gear")
  for i in all(gearSlots) do
    -- process gear slot
  end

  local trashSlots = GetSlotsByName("trash")
  for i in all(trashSlots) do
    -- process trash slot
  end


end
--]]
