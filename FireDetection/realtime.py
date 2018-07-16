import os, sys, io, json, time
import datetime as dt
import cv2

from utils.imageTools import imageToBase64, base64ToImage
from utils.plot_helpers import visualize_detections_raw
from utils.config_helpers import merge_configs
from FasterRCNN.FasterRCNN_eval import FasterRCNN_Evaluator
import utils.od_utils as od
from cntk import load_model, __version__

# PARAMETERS

detectorName = 'FasterRCNN'
pretrainnedModelName = r'./PretrainedModels/prometheus.dnn'
workingDir = os.path.dirname(os.path.abspath(__file__))

def getConfiguration(detector_name):
    from FasterRCNN.FasterRCNN_config import cfg as detector_cfg
    from utils.configs.AlexNet_config import cfg as network_cfg
    from utils.configs.Prometheus_config import cfg as dataset_cfg

    return merge_configs([detector_cfg, network_cfg, dataset_cfg, {'DETECTOR': detector_name}])

print("\nPython version: " + str(sys.version) + ", CNTK version: " + __version__)
startTime = dt.datetime.now()

# load configuration
print("Loading Prometheus configuration:", detectorName)
cfg = getConfiguration(detectorName)

# load model
print("Loading Neural Network: ", pretrainnedModelName)
od.prepareOnly_object_detector(cfg)
eval_model = load_model(os.path.join(workingDir, pretrainnedModelName))
evaluator = FasterRCNN_Evaluator(eval_model, cfg)


cap = cv2.VideoCapture(0)

while(True):
    # Capture frame-by-frame
    ret, frame = cap.read()

    # Prepare to detect
    regressed_rois = None
    cls_probs = None
    
    regressed_rois, cls_probs = evaluator.process_image_raw(frame)
    bboxes, labels, scores = od.filter_results(regressed_rois, cls_probs, cfg)

    # Add text label and network score to the video captue
    scoredFrame = visualize_detections_raw(frame, bboxes, labels, scores,
                               cfg.IMAGE_WIDTH, cfg.IMAGE_HEIGHT,
                               classes = cfg["DATA"].CLASSES,
                               draw_negative_rois = cfg.DRAW_NEGATIVE_ROIS)

    # Display the resulting frame
    cv2.namedWindow('frame', cv2.WINDOW_NORMAL)
    cv2.resizeWindow('frame', cfg.IMAGE_WIDTH, cfg.IMAGE_HEIGHT)
    cv2.imshow('frame',scoredFrame)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# When everything done, release the capture
cap.release()
cv2.destroyAllWindows()