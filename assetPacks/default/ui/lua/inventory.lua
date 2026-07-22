-- basic inventory
-- by cubee

local t = 1


function Tick(self)
  if t % 60 == 0 then
    DebugLog("Inventory ui lua test. ping!")
  end

  t = t + 1
end



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
