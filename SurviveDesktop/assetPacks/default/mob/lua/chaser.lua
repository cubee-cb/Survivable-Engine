-- test chaser ai

local t = 1
local target = nil

local state = 1
local stateTimer = 0
local stateDuration = 300

local dashEnd = nil





local states = {
  -- follow
  function()
    stateDuration = 180
  
    if target ~= nil then
      MoveToward(target.x or 0, target.y or 0, 2)
    end
  
  end, 

  -- pause
  function()
    stateDuration = 30

    if (stateTimer == 0) then
      PlaySound("@mob.chaser_shout")
    end

  end,

  -- dash
  function()
    stateDuration = 60

    if (stateTimer == 0) then
      PlaySound("@mob.chaser_dash")
      dashEnd = target
    end

    local dashSpeed = 10;

    if dashEnd then
      -- snap and finish state if the target is within dash step distance
      if (DistanceTo(dashEnd.x, dashEnd.y) <= dashSpeed) then
        SnapPosition(dashEnd.x, dashEnd.y)

        stateTimer = stateDuration
        dashEnd = nil

      -- move by dash step speed
      else
        MoveToward(dashEnd.x or 0, dashEnd.y or 0, dashSpeed)
      end
    end

  end
}

-- called every tick
function AI(self)
  target = GetTarget("player", "Nearest")

  --Move(1, 0, 10)

  -- check if near the target
  local near = true
  --local near = DistanceTo(target.x, target.y) < 64

  -- use states list if there is a target close enough
  if (target.valid and near) then

    -- run the current state from the states table
    states[state]()

    -- transition states
    stateTimer = stateTimer + 1
    if (stateTimer >= stateDuration) then
      stateTimer = 0
      state = state + 1

      -- loop back to start
      if (state > #states) then
        state = 1
      end

    end
    
  -- use predefined wander behaviour if there is no valid target
  else
    state = 1
    stateTimer = 0

    --Wander()
  end

	--PlaySound("a")

  t = t + 1
end

-- called every tick
function Tick()
end

-- called when the mob is interacted with
function Interact()
  DebugLog("I have been interacted with.")
end

-- called when a collision is entered
function CollisionEnter(tags)
  DebugLog("I have collided with " .. (tags[1] or "unknown tag") .. ".")
end

-- called when the mob is damaged
function CollisionExit(tags)
  DebugLog("I have stopped colliding with " .. (tags[1] or "unknown tag") .. ".")
end

-- called when the mob is damaged
function Damaged()
  DebugLog("I have been damaged.")
end

-- called when the mob is defeated
function Defeated()
  DebugLog("I have been defeated.")
end
