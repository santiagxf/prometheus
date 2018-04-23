# Copyright (c) Microsoft. All rights reserved.

# Licensed under the MIT license. See LICENSE.md file in the project root
# for full license information.
# ==============================================================================

from __future__ import print_function
import os
import sys
try:
    from urllib.request import urlretrieve 
except ImportError: 
    from urllib import urlretrieve

# Add models here like this: (category, model_name, model_url)
models = (('Image Classification', 'AlexNet_ImageNet_Caffe', 'https://www.cntk.ai/Models/Caffe_Converted/AlexNet_ImageNet_Caffe.model'),
         )

def download_model(model_file_name, model_url):
    model_dir = os.path.dirname(os.path.abspath(__file__))
    filename = os.path.join(model_dir, model_file_name)
    if not os.path.exists(filename):
        print('Downloading model from ' + model_url + ', may take a while...')
        urlretrieve(model_url, filename)
        print('Saved model as ' + filename)
    else:
        print('CNTK model already available at ' + filename)
    
def download_model_by_name(model_name):
    if model_name.endswith('.model'):
        model_name = model_name[:-6]

    model = next((x for x in models if x[1]==model_name), None)
    if model is None:
        print("ERROR: Unknown model name '%s'." % model_name)
        list_available_models()
    else:
        download_model(model_name + '.model', model[2])

def list_available_models():
    print("\nAvailable models (for more information see Readme.md):")
    max_cat = max(len(x[1]) for x in models)
    max_name = max(len(x[1]) for x in models)
    print("{:<{width}}   {}".format('Model name', 'Category', width=max_name))
    print("{:-<{width}}   {:-<{width_cat}}".format('', '', width=max_name, width_cat=max_cat))
    for model in sorted(models):
        print("{:<{width}}   {}".format(model[1], model[0], width=max_name))

if __name__ == "__main__":
    download_model_by_name("AlexNet_ImageNet_Caffe")
            
