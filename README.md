# Project Prometheus

<p>Prometheus is an early fire detection artificial intelligence technology using drones. Named after Prometheus, the Greek Titan, who defies the gods by stealing fire and giving it to humanity, an act that enabled progress and civilization. We are itching to do the same.</p>

<p>We combine autopiloted drones and computer vision to detect wildfires while they are still in their early stages. The concept is to leverage the vast amounts of wildfire images and videos available on the internet to train a deep learning neural network to detect the presence of a fire, however small it may be. The drone would then fly over parks and forests collecting the images and an algorithm  will determine the confidence level at which it believes a region is about to develop a fire. This solution is capable of detecting small occurences of fire, hard to see from the distance.
</p>

<h2>
Prometheus is a joint efford of
</h2>
<div>
    <div style="float:left; text-align:center;padding:8px;" width="190"><img src='doc/asu.png' width='120'/><div style='word-wrap: break-word;width:120px'>Arizona State University</div></div>
    <div style="float:left; text-align:center;padding:15px;" width="190"><img src='doc/bomberos.png' width='110'/><div style='word-wrap: break-word;width:140px'>Bomberos Voluntarios República Argentina</div></div>
    <div style="float:left; text-align:center;padding:8px;" width="190"><img src='doc/noaa.png' width='120'/><div style='word-wrap: break-word;width:120px'>National Weather Service</div></div>
    <div style="float:left; text-align:center;padding:15px;" width="190"><img src='doc/tempe.png' width='105'/><div style='word-wrap: break-word;width:120px'>Tempe Fire Department</div></div>
    <div style="clear:both"></div>
</div>

<h3>
    Dreamt and built by people from
</h3>
<div>
    <div style="float:left; text-align:center;padding:8px;"><img src='doc/ar.png' width='80'/><div style='word-wrap: break-word;width:90px'>Argentina</div></div>
    <div style="float:left; text-align:center;padding:8px;"><img src='doc/ir.png' width='80'/><div style='word-wrap: break-word;width:90px'>Irland</div></div>
    <div style="float:left; text-align:center;padding:8px;"><img src='doc/us.png' width='80'/><div style='word-wrap: break-word;width:90px'>United States</div></div>
    <div style="clear:both"></div>
</div>

<h2>
Technologies
</h2>
<p>
Prometheus uses Image Detection with computer vision to spot the small fires. By using this technique, the model is able to detect small fires and outperform image classification techniques. We are also able to avoid the use of infrared sensors that are not suitable for hot areas like Arizona, California, Nevada or Colorado.
</p>
<p>
    <ul>
        <li><b>Cognitive ToolKit (CNTK):</b> We used an implementation of Faster R-CNN over CNTK, a framework for the modeling, training and design of deep neural networks.</li>
        <li><b>Python:</b> The model and prediction API is implemented in Python. Both for scoring an image and for training the model.</li>
        <li><b>CUDA:</b> The training was done using GPUs for speed and scalability, leveraging the CUDA technology.</li>
        <li><b>Azure Experimentation Service:</b> We use the Azure Experimentation service to train and tune the parameters we need for the neural network.</li>
        <li><b>Docker container service:</b> The web service is hosted inside a docker container that can be deployed in any platform.</li>
        <li><b>Azure Functions:</b> Prometheus can send alerts by SMS to people to respond to fires. This functionality is hosted in Azure Functions. An storage account is also required.
        <li><b>Visual Studio: </b>The proposal is modeled as a solution in Visual Studio, containing 3 projects: The FireDetection experimentation project - used with Azure Experimentation Service for training, the Prometheus App - a WPF Windows application and a web API shipped as a docker container - implemented using Flask with Python for scoring.</li>
    </ul>
</p>

<h2>
    Instalation instructions
</h2>
<h3>
    Prerequisites
</h3>
<p>
    <ul>
        <li><b>Docker Container Service:</b> Prometheus API is shipped as a Docker container for convenience. You can deploy the container localy in your system or in any cloud provider. The image is a Linux OS. We recomend to deploy it on a Nvidia Docker system to leverage GPU technology if available, but it is not required. The API will use GPU if available, otherwise it will use CPU. By default, the REST api listens at port 80. The docker image deploys the following components: 
            <ul>
                <li><b>Cognitive ToolKit (CNTK):</b> Version 2.5</li>
                <li><b>Python:</b> 3.5.4. The file Prometheus.api\conda_requirements.yml contains all the packages that will be installed in the Conda Environment.</li>
                <li><b>CUDA:</b> If GPU is used, then NVidia dockers support should be present.</li>
                <li><b>Intel Math library:</b>Used for fast computation of the areas of interest in the images. If you are planning to deploy the service localy without the container, please follow the installation instructions for this SDK.</li>
                <li><b>Flask:</b>REST API is implemented in Flask.</li>
                <li><b>Promethus DNN:</b>Our pretrained model is included in the docker file. There is no need to train the model.</li>
            </ul>
        </li>
        <li><b>.NET Framework:</b> You will need a Windows box with .NET 4 or later. Visual Studio is required to compile the WPF application. All required packages are included in Packages folder. A web based app is under development. Yeap!</li>
        <li><b>Azure account: </b>The following services are used:
            <ul>
                <li>Azure Docker Services (if deploying to the cloud - not required)</li>
                <li>Azure Experimentation Services (required if you plan to train the model)</li>
                <li>Azure Functions</li>
                <li>Azure Blob Storage</li>
                <li>Azure Table Storage</li>
            </ul>
        </li>
        <li><b>A Twillio account:</b> It is required if planning to use the SMS alerts capabilities.</li>
        <li><b>Azure Workbench: </b>Optional if parameter tunning is needed.</li>
    </ul>
</p>
<h3>
    Installation
</h3>
<p>
    <ol>
        <li><b>Build the docker container: </b>Build the docker image using the dockerfile provided. A ready to use version is available in Docker Hub under the tag santiagxf/prometheus.0.9.0.</li>
        <li><b>Download the solution and build:</b> Download the solution and build the project Prometheus.Planner.Console. You will require a Windows OS running .NET framework 4.0 or later.</li>
        <li><b>Configure the service:</b> Open the file Prometheus.Planner.Console\App.config and point the configuration key prometheusWebServiceUrl to your hosting URL. By default the docker container maps the API service to port 80. The endpoint for scoring images is /score</li>
        <li><b>Deploy the Azure functions (optional):</b> If you want to use the SMS alerts capabilities, you need to deploy the Azure Functions and configure a Twilio account to process them. Open the file Prometheus.Planner.Console\App.config to configure the endpoints in your Azure Functions</li>
        <li><b>Run the service:</b>Run the container publishing the port 80.</li>
        <li><b>Run the Windows App: </b>Run the WPF application you compiled or use Visual Studio, Debug, Start new instance.</li>
    </ol>
</p>

