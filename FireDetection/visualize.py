import os
from keras.models import load_model
import matplotlib.pyplot as plt
import cntk

pretrainnedModelName = './PretrainedModels/prometheus.dnn'
workingDir = os.path.dirname(os.path.abspath(__file__))

model = load_model(pretrainnedModelName)
cntk.logging.graph.plot(model, 'model.png')
