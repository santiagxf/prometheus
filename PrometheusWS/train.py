# Spark configuration and packages specification. The dependencies defined in
# this file will be automatically provisioned for each run that uses Spark.
import os, sys
import numpy as np
import utils.od_utils as od
from utils.config_helpers import merge_configs

import argparse

from azureml.logging import get_azureml_logger


# GLOBAL VARIABLES

availableDetectors = ['FastRCNN', 'FasterRCNN'];
defaultDetectorName = 'FasterRCNN'

def getDetectorName(args):
    detectorName = defaultDetectorName
    if len(args) != 2:
        print("Please provide a detector name as the single argument. Usage: \n\tpython DetectionDemo.py <detector_name>")
        print("Available detectors: {}".format(availableDetectors))
    else:
        detectorName = args[1]
        if not detectorName in availableDetectors:
            print("Unknown detector. Switching to default", defaultDetectorName)
            detectorName = defaultDetectorName
    return detectorName


def loadConfiguration(detector_name):
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


# initialize the logger
logger = get_azureml_logger()

# add experiment arguments
parser = argparse.ArgumentParser()
# parser.add_argument('--arg', action='store_true', help='My Arg')
args = parser.parse_args()
print(args)

# This is how you log scalar metrics
# logger.log("MyMetric", value)

# Create the outputs folder - save any outputs you want managed by AzureML here
os.makedirs('./outputs', exist_ok=True)