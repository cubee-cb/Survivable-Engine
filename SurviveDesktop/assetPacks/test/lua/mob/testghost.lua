-- test ghost ai

t = 1

-- called every tick
function AI(self)
  --DebugLog(self)

  t = t + 1
  --DebugLog(pos)

  --Move(1, 0, 10)
  local target = GetTarget("player", "Nearest")
  local x = target.x
  local y = target.y
  local targetIsValid = target.valid

  --DebugLog("x target: " .. x)
  --DebugLog("y target: " .. y)

  -- move toward target if one is found
  if (targetIsValid) then
	  MoveToward(x or 0, y or 0, 1)

  -- complain loudly if not
  else
	  if (t % 10 == 9) then
		  PlaySound("*.mob_testghost_alert")
      DebugLog("no target!!")
	  end
  end

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