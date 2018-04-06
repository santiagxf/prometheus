# Copyright (c) Microsoft. All rights reserved.

# Licensed under the MIT license. See LICENSE.md file in the project root
# for full license information.
# ==============================================================================

import os, sys
import numpy as np
import utils.od_utils as od
from cntk import load_model
from utils.config_helpers import merge_configs

detector_name = 'FasterRCNN'

def get_configuration(detector_name):
    from FasterRCNN.FasterRCNN_config import cfg as detector_cfg

    # for AlexNet base model use:       from utils.configs.AlexNet_config import cfg as network_cfg
    from utils.configs.AlexNet_config import cfg as network_cfg
    # for the Grocery data set use:     from utils.configs.Prometheus_config import cfg as dataset_cfg
    from utils.configs.Prometheus_config import cfg as dataset_cfg

    return merge_configs([detector_cfg, network_cfg, dataset_cfg, {'DETECTOR': detector_name}])

if __name__ == '__main__':
    cfg = get_configuration(detector_name)
    modelPath = os.path.join(os.path.dirname(os.path.abspath(__file__)), r"PretrainedModels\prometheus.model");

    # load model
    od.prepareOnly_object_detector(cfg);
    print('Loading model from', modelPath)
    eval_model = load_model(modelPath)

    # detect objects in single image
    img_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), "DataSets", "Fire", "positive", "1010_nws_ocr-l-ahfire-032.jpg")
    print('Testing image', img_path)
    regressed_rois, cls_probs = od.evaluate_single_image(eval_model, img_path, cfg)
    bboxes, labels, scores = od.filter_results(regressed_rois, cls_probs, cfg)

    # write detection results to output
    fg_boxes = np.where(labels > 0)
    print("#bboxes: before nms: {}, after nms: {}, foreground: {}".format(len(regressed_rois), len(bboxes), len(fg_boxes[0])))
    for i in fg_boxes[0]: print("{:<12} (label: {:<2}), score: {:.3f}, box: {}".format(
                                cfg["DATA"].CLASSES[labels[i]], labels[i], scores[i], [int(v) for v in bboxes[i]]))

    # visualize detections on image
    od.visualize_results(img_path, bboxes, labels, scores, cfg)

    # measure inference time
    od.measure_inference_time(eval_model, img_path, cfg, num_repetitions=100)

     # detect objects in single image
    img_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), r"./DataSets/Fire/negative/videoblocks-beginning-of-a-forest-fire-uhd_buoxnjgpb__WL_frame_27.jpg")
    regressed_rois, cls_probs = od.evaluate_single_image(eval_model, img_path, cfg)
    bboxes, labels, scores = od.filter_results(regressed_rois, cls_probs, cfg)

    # write detection results to output
    fg_boxes = np.where(labels > 0)
    print("#bboxes: before nms: {}, after nms: {}, foreground: {}".format(len(regressed_rois), len(bboxes), len(fg_boxes[0])))
    for i in fg_boxes[0]: print("{:<12} (label: {:<2}), score: {:.3f}, box: {}".format(
                                cfg["DATA"].CLASSES[labels[i]], labels[i], scores[i], [int(v) for v in bboxes[i]]))

    # visualize detections on image
    od.visualize_results(img_path, bboxes, labels, scores, cfg)

    # measure inference time
    od.measure_inference_time(eval_model, img_path, cfg, num_repetitions=100)
