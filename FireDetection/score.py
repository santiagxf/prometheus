# Copyright (c) Microsoft. All rights reserved.

# Licensed under the MIT license. See LICENSE.md file in the project root
# for full license information.
# ==============================================================================

import os, sys, io, json
import datetime as dt
import pandas as pd
from utils.imageTools import imageToBase64, base64ToImage
from utils.config_helpers import merge_configs
import utils.od_utils as od
from cntk import load_model

detectorName = 'FasterRCNN'
pretrainnedModelName = r'./PretrainedModels/prometheus.dnn'
workingDir = os.path.dirname(os.path.abspath(__file__))

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


def run(input_df):
    print(str(input_df))
    
    startTime = dt.datetime.now()

    # convert input back to image and save to disk
    base64ImgString = input_df['image base64 string'][0]
    imgPath = base64ToImage(base64ImgString, workingDir)

    # load configuration
    cfg = getConfiguration(detectorName)

    # load model
    od.prepareOnly_object_detector(cfg)
    eval_model = load_model(os.path.join(workingDir), pretrainnedModelName))

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

def main():
    from azureml.api.schema.dataTypes import DataTypes
    from azureml.api.schema.sampleDefinition import SampleDefinition
    from azureml.api.realtime.services import generate_schema

    demoimage = os.path.join(os.path.dirname(workingDir), r"./uploadedImg.jpg")
    base64ImgString = imageToBase64(demoimage)
    df = pd.DataFrame(data=[[base64ImgString]], columns=['image base64 string'])
    
    # Turn on data collection debug mode to view output in stdout
    os.environ["AML_MODEL_DC_DEBUG"] = 'true'

    # Call init() and run() function
    init()
    inputs = {"input_df": SampleDefinition(DataTypes.PANDAS, df)}

    results = run(df)
    print("resultString = " + str(results))

    # Genereate the schema
    generate_schema(run_func=run, inputs=inputs, filepath='.\schema.json')
    print("Schema generated.")


if __name__ == "__main__":
    main()