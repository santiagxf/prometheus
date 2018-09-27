# Project Prometheus

<p>Prometheus is an early-stage fire detection solution that combines AI, Computer Vision, auto-piloted drones and weather services to detect wildfires before they spread too large. Named after Prometheus, the Greek Titan, who defies the gods by stealing fire and giving it to humanity, an act that enabled progress and civilization. We are itching to do the same.</p>

<p>We combine autopiloted drones and computer vision to detect wildfires while they are still in their early stages. The concept is to leverage the vast amounts of wildfire images and videos available on the internet to train a regional deep learning neural network to detect the presence of a fire, however small it may be. The drone would then fly over parks and forests collecting the images and an algorithm  will determine the confidence level at which it believes a region is about to develop a fire. This solution is capable of detecting small occurences of fire, hard to see from the distance.
</p>

<h2>
Prometheus is a joint efford of
</h2>
<table>
    <tr>
        <td align="center" width="190"><img src='doc/asu.png' width='120'/><div style='word-wrap: break-word;width:120px'>Arizona State University</div></td>
        <td align="center" width="190"><img src='doc/bomberos.png' width='110'/><div style='word-wrap: break-word;width:140px'>Bomberos Voluntarios República Argentina</div></td>
        <td align="center" width="190"><img src='doc/noaa.png' width='120'/><div style='word-wrap: break-word;width:120px'>National Weather Service</div></td>
        <td align="center" width="190"><img src='doc/tempe.png' width='105'/><div style='word-wrap: break-word;width:120px'>Tempe Fire Department</div></td>
    </tr>
</table>

<h2>
Technologies
</h2>
<p>
Prometheus uses Image Detection with computer vision to spot the small fires. By using this technique, the model is able to detect small fires and outperform image classification techniques. We are also able to avoid the use of infrared sensors that are not suitable for hot areas like Arizona, California, Nevada or Colorado. 
</p>
<p>
    <ul>
        <li><b>Cognitive ToolKit (CNTK):</b> We used an implementation of Faster R-CNN over CNTK, a framework for the modeling, training and design of deep neural networks.</li>
        <li><b>Python:</b> The model and prediction API is implemented in Python. Both for scoring an image and for training the model. Current target environment is Python 3.5</li>
        <li><b>CUDA:</b> The training was done using GPUs for speed and scalability, leveraging the CUDA technology.</li>
        <li><b>Docker container service:</b> The web service is hosted inside a docker container that can be deployed in any platform either locally or in the cloud. To leverage GPU, you need to use Nvidia Containers.</li>
        <li><b>Serverless functions:</b> Prometheus can send alerts by SMS to people to respond to fires. This functionality is implemented using serverless functions in the cloud along with Twilio automatic SMS capabilities.
        <li><b>Visual Studio: </b>The solution is modeled as a solution in Visual Studio, containing 3 projects: The FireDetection experimentation project - where the DNN was modeled and trained, the Prometheus App - a WPF Windows application and a web API shipped as a docker container - implemented using Flask with Python for scoring.</li>
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
        <li><b>.NET Framework:</b> You will need a Windows box with .NET 4 or later. Visual Studio is required to compile the WPF application. All required packages are included in Packages folder. A web based app is under development, but it's a bit hard to get Weather Maps working on it. Easier on the desktop.</li>
        <li><b>Serverless Functions provider: </b>We keep this as cloud provider agnostic as possible. Serverless functions can be implemented in any provider as long as there is a webhook available over the GET method:
            <ul>
                <li>You need functions to support the Subscribe, Unsubscribe and Notify functions. IBM Bluemix contians implementations of this using Twilio. Azure contains a template for Twilio automatic SMS.</li>
                <li><b>A Twillio account:</b> It is required if planning to use the SMS alerts capabilities.</li>
            </ul>
        </li>
        <li><b>Weather services APIs keys:</b> We connect with a lot of weather services providers, some of them using an specific key that was provided to us. We do not share those keys as they are registered under our name and there are specific threshold we need to agree. You will have to get the following keys:
            <ul>
                <li>NASA FIRMS RADAR</li>
                <li>NowCoast NOAA Oceanic Observations</li>
                <li>OpenWeather</li>
                <li>Argentina Red Flag Alerts</li>
            </ul> 
        </li>
    </ul>
</p>
<h3>
    Installation
</h3>
<p>
    <ol>
        <li><b>Build the docker container: </b>Build the docker image using the dockerfile provided. We use a prebuilt docker image of CNTK to speed up the setup process.</li>
        <li><b>Download the solution and build:</b> Download the solution and build the project Prometheus.Planner.Console. You will require a Windows OS running .NET framework 4.0 or later.</li>
        <li><b>Configure the service:</b> Open the file Prometheus.Planner.Console\App.config and point the configuration key prometheusWebServiceUrl to your hosting URL. By default the docker container maps the API service to port 80. The endpoint for scoring images is /score</li>
        <li><b>Deploy the serverless functions (optional):</b> If you want to use the SMS alerts capabilities, you need to deploy serverless Functions and configure a Twilio account to process them. Open the file Prometheus.Planner.Console\App.config to configure the endpoints of your webhooks functions</li>
        <li><b>Run the service:</b>Run the container publishing the port 80.</li>
        <li><b>Run the Windows App: </b>Run the WPF application you compiled or use Visual Studio, Debug, Start new instance.</li>
    </ol>
</p>

