-- test smelter inventory
-- minecraft-style

local t = 1


local fuelDuration = 3600
local cookTimer = 0

--[[ called every tick
function Tick(self)
  local inputSlot = GetSlot(1)
  local outputSlot = GetSlot(2)
  local fuelSlot = GetSlot(3)

  local fuelWidget = GetWidgetByName("fuel")
  local progressWidget = GetWidgetByName("timer")

  -- yes fuel
  if fuelDuration > 0 then
    fuelDuration = fuelDuration - 1

    if inputSlot then
      cookTimer = cookTimer + 1
    else
      cookTimer = 0
    end

    -- cook item
    if cookTimer >= 30 * 5 then
      cookTimer = 0
      -- todo: cook item
      AddItem(outputSlot, inputSlot.id, AMOUNT, 1)
      AddItem(inputSlot, inputSlot.id, AMOUNT, -1)
    end


  -- no fuel
  else

    -- restock on fuel
    if fuelSlot and inputSlot and inputSlot.cookable then -- or something idk, api doesn't exist yet
      AddItem(fuelSlot, fuelSlot.id, AMOUNT, -1)
    end
  end

end
--]]
