-- test ghost ai

-- called every tick
function AI(self)
  DebugLog(self)
  --self.position.X = self.position.X + 1
  Move(1, 0)
  local target = GetTarget({"tags", "are", "kinda", "cool"})
  local x = target[1]
  local y = target[2]

  DebugLog("x target: " .. x)
  DebugLog("y target: " .. y)

  MoveToward(x, y)

end

-- called every tick
function Tick(self)
  print(self)
end

-- called when the mob is interacted with
function Interact(self)
  print(self)
end

-- called when the mob is damaged
function Damaged(self)
  print(self)
end

-- called when the mob is defeated
function Defeated(self)
  print(self)
end