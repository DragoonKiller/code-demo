local Iterator = { }
local meta = {
    __index = Iterator,
    __newindex = function() error('don\'t assign a value to linq object!') end,
}
local next = next

function Iterator.New(t)
    local iter = { }
    local k
    iter.iterator = function()
        local v
        k, v = next(t, k)
        return k, v
    end
    setmetatable(iter, meta)
    return iter
end

function Iterator:Map(f)
    local iter = self.iterator
    self.iterator = function()
        local k, v = iter()
        if k == nil and v == nil then return end
        return f(k, v)
    end
    return self
end

function Iterator:Filter(f)
    local iter = self.iterator
    self.iterator = function()
        local k, v
        repeat
            k, v = iter()
        until k == nil or v == nil or f(k, v)
        return k, v
    end
    return self
end

function Iterator:ToTable()
    local iter = self.iterator
    local res = { }
    for k, v in self:Iter(), nil, nil do
        res[k] = v
    end
    return res
end

function Iterator:Count()
    local c = 0
    for k, v in self:Iter(), nil, nil do
        c = c + 1
    end
    return c
end

function Iterator:Iter()
    local iter = self.iterator
    self.iterator = nil
    return iter
end


-------------------------------------------------------------------------------
-------------------------------------------------------------------------------
-------------------------------------------------------------------------------


local arr = {
    a = 1,
    b = 2,
    c = 3,
    d = 4,
    e = 5,
}

local p = Iterator.New(arr):ToTable()
assert(p.a == 1)
assert(p.b == 2)
assert(p.c == 3)
assert(p.d == 4)
assert(p.e == 5)

local g = Iterator.New(arr)
    :Map(function(k, v) return k, v + 1 end)
    :ToTable()
assert(g.a == 2)
assert(g.b == 3)
assert(g.c == 4)
assert(g.d == 5)
assert(g.e == 6)

local w = Iterator.New(arr)
    :Filter(function(k, v) return v % 2 == 0 end)
    :ToTable()
assert(w.a == nil)
assert(w.b == 2)
assert(w.c == nil)
assert(w.d == 4)
assert(w.e == nil)

local r = Iterator.New(arr)
    :Map(function(k, v) return v, k end)
    :ToTable()
assert(r[1] == 'a')
assert(r[2] == 'b')
assert(r[3] == 'c')
assert(r[4] == 'd')
assert(r[5] == 'e')

local c = Iterator.New(arr):Count()
assert(c == 5)

-------------------------------------------------------------------------------
-------------------------------------------------------------------------------
-------------------------------------------------------------------------------

local gset = { }
for i = 1, 10000000 do
    gset[i] = (i % 3 == 0 and 999) or (i % 3 == 1 and 998) or 997
end

print(
    gset[1], gset[2], gset[3],
    gset[4], gset[5], gset[6],
    gset[7], gset[8], gset[9]
)

-- map
local begin = os.clock()
for i = 1, 10000000 do
    gset[i] = 99 - gset[i]
end
local total = os.clock()
print('map ordinary', total)

local begin = os.clock()
for i, _ in Iterator.New(gset):Iter(), nil, nil do
    gset[i] = 99 - gset[i]
end
local total = os.clock()
print('map iterator', total)

local begin = os.clock()
for i, _ in pairs(Iterator.New(gset):ToTable()) do
    gset[i] = 99 - gset[i]
end
local total = os.clock()
print('map table', total)

-- map ordinary    1.291
-- map iterator    2.447
-- map table       4.078

return Iterator.New
