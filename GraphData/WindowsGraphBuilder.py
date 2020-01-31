import pandas as pd
from matplotlib import pyplot as plt

df =  pd.read_csv('C:/Users/chase.mossing2/Desktop/Trad-Data-Monitor-3/GraphData\data.csv')

p = df.plot('DateTime', 'Data', kind = 'line')
p.set_xlabel("Time")
p.set_ylabel("Sensor Data")


plt.savefig('C:/Users/chase.mossing2/Desktop/Trad-Data-Monitor-3/GraphData/graph.png')