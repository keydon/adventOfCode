import itertools
fp = open('/code/input01.txt', 'r')
content = fp.readlines()
lines = [x.strip() for x in content]
numbers = [int(x) for x in lines]
divided = [int(x/3) for x in numbers]
subs = [int(x-2) for x in divided]
sum(subs)

#part2
def calc_fuel(x):
  fuel = int(x/3) - 2
  if fuel > 0:
    return fuel + calc_fuel(fuel)
  else:
    return 0

calced = [calc_fuel(x) for x in numbers]
sum(calced)
