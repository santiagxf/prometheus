import os, sys, io, json
import datetime as dt
import pandas as pd
from utils.imageTools import imageToBase64, base64ToImage
from utils.config_helpers import merge_configs
import utils.od_utils as od
from cntk import load_model

# PARAMETERS

detectorName = 'FasterRCNN'
pretrainnedModelName = '/outputs/prometheus.dnn'
workingDir = os.path.dirname(os.path.abspath(__file__))

# GLOBALS

startTime = dt.datetime.now()

def getConfiguration(detector_name):
    # load configs for detector, base network and data set
    if detector_name == "FastRCNN":
        from FastRCNN.FastRCNN_config import cfg as detector_cfg
    elif detector_name == "FasterRCNN":
        from FasterRCNN.FasterRCNN_config import cfg as detector_cfg
    else:
        print('Unknown detector: {}'.format(detector_name))

    from utils.configs.AlexNet_config import cfg as network_cfg
    from utils.configs.Prometheus_config import cfg as dataset_cfg

    return merge_configs([detector_cfg, network_cfg, dataset_cfg, {'DETECTOR': detector_name}])

def toJson(bboxes, labels, scores):
    # Create json-encoded string of the model output
    executionTimeMs = (dt.datetime.now() - startTime).microseconds / 1000
    outDict = {"boxes": bboxes.tolist(), "labels": labels.tolist(), "scores": scores.tolist(), "executionTimeMs": str(executionTimeMs)}
    outJsonString = json.dumps(outDict)

    return(str(outJsonString))

def run(input_df):
    print(str(input_df))
    
    startTime = dt.datetime.now()

    #input
    imgPath = input_df

    # load configuration
    cfg = getConfiguration(detectorName)

    # load model
    od.prepareOnly_object_detector(cfg)
    eval_model = load_model(os.path.join(os.path.dirname(os.path.abspath(__file__)), r"../PretrainedModels/prometheus.dnn"))

    # score image
    regressed_rois, cls_probs = od.evaluate_single_image(eval_model, imgPath, cfg)
    bboxes, labels, scores = od.filter_results(regressed_rois, cls_probs, cfg)

    return toJson(bboxes, labels, scores)


def init():
    try:
        print("Executing init() method...")
        print("Python version: " + str(sys.version) + ", CNTK version: " + cntk.__version__)
    except Exception as e:
        print("Exception in init:")
        print(str(e))