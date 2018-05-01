# Prometheus Web API
<p>Prometheus.API contains a REST API implemented in Python using Flask that provides the scoring functionality for the images. The service supports the following methods: </p>
<ul>
    <li>/score, which allows to score one or multiple files. A JSON object is returned including the labels and the probabilities of detecting fire</li>
    <li>/supervise, which allows the client to provide feedback about the scored image. Each scored image receives a unique ID that can be used in this endpoint to provide insight if the picture actually contained a fire or not. It is used to retrain the network over time.</li>
    <li>/help, provides help about how to use the service</li>
</ul>

<h3>Installation</h3>
<p>The Prometheus API is provided as a docker container image that contains all the resoruces to run the service. The container starts a Flask web server which listen by default in port 80. All dependencies are automatically installed by the Dockerfile. It is also available in Dockerhub with the tag santiagxf/prometheus.0.9.0<p/>
<h3>Components</h3>
<ol>
    <li><b>Cognitive ToolKit (CNTK):</b>The docker image contains the Microsoft Data Science Virtual Machine with a Linux distribution with GPU support. GPU can be deativated using configuration files, however is on by default on the given configuration. Follow the steps on https://docs.microsoft.com/en-us/cognitive-toolkit/setup-cntk-on-your-machine to get the image working in a local environment using NVidia dockers.</li>
    <li>
        <p>Special considerations to:
            <ul>
                <li><b>CUDA:</b> If GPU is used, then CUDA 9.X+ should be installed</li>
                <li><b>Intel Math library:</b>Follow instructions on Microsoft CNTK installation to install the library.</li>
            </ul>
        </p>
    </li>
    <li><b>Python version:</b> Python 3.5.4 is used. All dependencies are installed in a Conda environemnt.</li>
    <li><b>Python packages:</b> OpenCV may need a manual installation for Python 3.5 and a compiled version is downloaded from a personal repository. You can change the file conda_dependencies.yml to modify this.</li>
</ol>