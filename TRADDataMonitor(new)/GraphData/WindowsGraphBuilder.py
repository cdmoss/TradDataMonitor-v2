import pandas as pd
from matplotlib import pyplot as plt

df =  pd.read_csv('C:/Users/cheze/Desktop/TRADDataMonitor/GraphData\data.csv')
ylabel = df.loc[0, "SensorType"]
p = df.plot('DateTime', 'Data', kind = 'line', legend=False)
p.set_xlabel("Time")
p.set_ylabel(ylabel)
p.set_xticklabels(df['DateTime'], rotation=45)
plt.tight_layout()
plt.savefig('C:/Users/cheze/Desktop/TRADDataMonitor/GraphData/graph.png')