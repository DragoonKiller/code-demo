import math
from collections import namedtuple
import matplotlib.pyplot as plt
import numpy as np
from scipy.optimize import minimize
Record = namedtuple("Record", ['freq', 'delta', 'length'])

baseFreq = 440.0


data = [
    # 1
    [[30.0 - 2.95], [495.0, 499.0]],
    [[30.0 + 0.05], [456.0, 469.5]],
    [[30.0 + 2.95], [420.0, 426.0]],
    [[30.0 + 6.00], [397.0, 400.0]],
    [[30.0 + 8.15], [378.0, 381.0]],
    # 6
    [[30.0 + 10.90], [356.0, 364.0]],
    [[30.0 + 13.45], [340.0, 344.5]],
    [[30.0 + 15.45], [329.0, 330.5]],
    [[30.0 + 17.50], [315.0, 318.5]],
    [[30.0 + 19.30], [303.0, 308.0]],
    # 11
    [[50.0 + 1.35], [296.0, 297.0]],
    [[50.0 + 3.45], [286.0, 287.0]],
    [[50.0 + 5.80], [275.0, 277.5]],
    [[50.0 + 8.35], [256.0, 258.0]],
]
# data[_][1]: 长度, 单位 cm
# data[_][2]: 音高, 单位 hz

# 除以 100 是 cm 转 m
l = np.average([ x[0] for x in data ], axis = 1) / 100
print('len array \n %s' % (l))

f = np.average([ x[1] for x in data ], axis = 1)
print('freq array \n %s' % (f))

# 拟合
# F = A + B / L
params = [0.0, 220.0]
def loss(params):
    return np.sqrt(np.sum(np.square(
        f - (params[0] + params[1] / l)
    )))
mm = minimize(loss, x0 = params)

print("::: F = %f + %f / L (in m.)" % (mm.x[0], mm.x[1]))

fig = plt.figure()
ax = fig.add_subplot()
ax.plot(np.array(l) * 100.0, f)
dr = range(25, 60)
ax.plot(dr, [mm.x[0] + mm.x[1] / (length / 100.0) for length in dr])
plt.ylim(0)
plt.show()