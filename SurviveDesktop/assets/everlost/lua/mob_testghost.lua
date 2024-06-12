-- test ghost ai

t = 1

-- called every tick
function AI(self)
  --DebugLog(self)

  t = t + 1
  --DebugLog(pos)

  --Move(1, 0, 10)
  local target = GetTarget({"player"})
  local x = target[1]
  local y = target[2]

  --DebugLog("x target: " .. x)
  --DebugLog("y target: " .. y)

  if (t % 60 == 59) then
    PlaySound("mob_testghost_alert")
  end

  MoveToward(x, y, 5)

end

-- called every tick
function Tick()
end

-- called when the mob is interacted with
function Interact()
  DebugLog("I have been interacted with.")
end

-- called when the mob is damaged
function Damaged()
  DebugLog("I have been damaged.")
end

-- called when the mob is defeated
function Defeated()
  DebugLog("I have been defeated.")
end