# run this with just "python ./Detection/FasterRCNN/run_sweep_parameters.py your_context_name"

import os, sys

# set context
context_name = ''
if len(sys.argv) > 1:
        context_name = sys.argv[1]

base_models = ['AlexNet', 'VGG16']

for base_model in base_models:
    os.system('az ml experiment submit -c {} ./Detection/FasterRCNN/run_faster_rcnn.py {}'.format(context_name, base_model))