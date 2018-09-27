# Copyright (c) Microsoft. All rights reserved.

# Licensed under the MIT license. See LICENSE.md file in the project root
# for full license information.
# ==============================================================================

import os, sys
import numpy as np
import utils.od_utils as od
from utils.config_helpers import merge_configs
from cntk import load_model

detectorName = 'FasterRCNN'
pretrainnedModelName = r'./PretrainedModels/prometheus.dnn'
workingDir = os.path.dirname(os.path.abspath(__file__))
detector_name = 'FasterRCNN'
azureTuning = true

def get_configuration(detector_name):
    # load configs for detector, base network and data set
    from FasterRCNN.FasterRCNN_config import cfg as detector_cfg
    # for AlexNet base model use:       from utils.configs.AlexNet_config import cfg as network_cfg
    from utils.configs.AlexNet_config import cfg as network_cfg
    # for the Fire data set use:     from utils.configs.Prometheus_config import cfg as dataset_cfg
    from utils.configs.Prometheus_config import cfg as dataset_cfg

    return merge_configs([detector_cfg, network_cfg, dataset_cfg, {'DETECTOR': detector_name}])

if __name__ == '__main__':
    cfg = get_configuration(detector_name)

    # train and test
    eval_model = od.train_object_detector(cfg, "./PretrainedModels")
    eval_results = od.evaluate_test_set(eval_model, cfg)
    
    # write AP results to output
    for class_name in eval_results: print('AP for {:>15} = {:.4f}'.format(class_name, eval_results[class_name]))
    
    if (azureTuning):
        from azureml.logging import get_azureml_logger
        run_logger = get_azureml_logger()

        run_logger.log("MAP", np.nanmean(list(eval_results.values())))
    else:
        print('Mean AP = {:.4f}'.format(np.nanmean(list(eval_results.values()))))