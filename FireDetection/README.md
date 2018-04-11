# FireDetection model experimentation

<p>We used Azure Experimentation Service to test and tune the model according to our dataset.Azure Machine Learning Workbench is an analytics toolset enabling data scientists to prepare data, run machine learning training experiments and deploy models at cloud scale</p>

<h2>Training dataset</h2>
<p>It is indeed hard to get images with fires that are about to start. We addressed such problem using crawsource. Using the "similar to" feature un web search engines, we looked for videos posted in social channels and video platforms containing aereal images of such situations. We used a package on available on Python that will use the similar to feature to download all the videos that match the createria.</p>

<h3>Training dataset</h3>
<p>Tagging was done manually using the VOTT tool provided by Microsoft. We first extracted the frames from the videos and then we tag the the images accordly. The tools allows to export the tagged images for latter use with CNTK or TensorFlow. We used CNTK in this situation.</p>

<h2>Training model</h2>
<p>Faster R-CNNs (Region proposals with Convolutional Neural Networks) are a relatively new approach (2015). They have been widely adopted in the Machine Learning community, and now have implementations in most of popular Deep Neural Net frameworks (DNNs) including PyTorch, CNTK, Tensorflow, Caffe, and others.</p>