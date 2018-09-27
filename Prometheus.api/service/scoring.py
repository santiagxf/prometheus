import os, sys, io, json, time
import datetime as dt

from service.utils.imageTools import imageToBase64, base64ToImage
from service.utils.config_helpers import merge_configs
import service.utils.od_utils as od
from cntk import load_model, __version__

class scorer:
    # PARAMETERS

    detectorName = 'FasterRCNN'
    pretrainnedModelName = r'./PretrainedModels/prometheus.dnn'
    workingDir = os.path.dirname(os.path.abspath(__file__))

    # GLOBALS

    startTime = dt.datetime.now()

    generateScoringId = lambda: str(round(time.time() * 1000))

    @staticmethod
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
    
    @staticmethod
    def buildResults(fileName, scoringId, bboxes, labels, scores, startTime):
        # Create json-encoded string of the model output
        executionTimeMs = (dt.datetime.now() - startTime).microseconds / 1000
        outDict = {"fileName": fileName, "scoringId": scoringId, "boxes": bboxes.tolist(), "labels": labels.tolist(), "scores": scores.tolist(), "executionTimeMs": str(executionTimeMs)}

        return(outDict)

    def run(self, input_df, fileName, scoringId = 0):
        startTime = dt.datetime.now()

        if (scoringId == 0):
            scoringId = self.generateScoringId()

        #input
        imgPath = input_df

        # score image
        regressed_rois, cls_probs = od.evaluate_single_image(self.eval_model, imgPath, self.cfg)
        bboxes, labels, scores = od.filter_results(regressed_rois, cls_probs, self.cfg)

        return scorer.buildResults(fileName, scoringId, bboxes, labels, scores, startTime)


    def __init__(self):
        try:
            print("Executing init() method...")
            print("Python version: " + str(sys.version) + ", CNTK version: " + __version__)

            # load configuration
            self.cfg = scorer.getConfiguration(self.detectorName)

            # load model
            od.prepareOnly_object_detector(self.cfg)
            self.eval_model = load_model(os.path.join(self.workingDir, self.pretrainnedModelName))

        except Exception as e:
            print("Exception in init:")
            print(str(e))