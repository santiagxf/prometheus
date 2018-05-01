# Project Prometheus

<p>An early fire detection AI technology using drones. Named after Prometheus, the Greek Titan, who defies the gods by stealing fire and giving it to humanity, an act that enabled progress and civilization. We are itching to do the same.</p>

<p>We combine surveillance drones and concepts of machine learning to detect wildfires while they are still in their early stages. The concept is to leverage the vast amounts of wildfire images and videos available on the internet to train a deep learning neural network to detect the presence of a fire, however small it may be. The drone would then fly over parks and forests collecting the images and an algorithm  will determine the confidence level at which it believes a region is about to develop a fire.
</p>

<h2>
Technologies
</h2>
<p>
    <ul>
        <li><b>Cognitive ToolKit (CNTK):</b> We used an implementation of Faster R-CNN over CNTK, a framework for the modeling, training and design of deep neural networks.</li>
        <li><b>Python:</b> The model and prediction API is implemented in Python. Both for scoring an image and for training the model.</li>
        <li><b>CUDA:</b> The training was done using GPUs for speed and scalability, leveraging the CUDA technology.</li>
        <li><b>Azure Experimentation Service:</b> We use the Azure Experimentation service to train and tune the parameters we need for the neural network.</li>
        <li><b>Docker container service:</b> The web service is hosted inside a docker container that can be deployed in any platform.</li>
        <li><b>Azure Functions:</b> Prometheus can send alerts by SMS to people to respond to fires. This functionality is hosted in Azure Functions. An storage account is also required.
        <li><b>Visual Studio: </b>The proposal is modeled as a solution in Visual Studio, containing 3 projects: The FireDetection experimentation project - used with Azure Experimentation Service for training-, the Prometheus App - a WPF Windows application- and a web API implemented using Flask with Python for scoring.</li>
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
        <li><b>Cognitive ToolKit (CNTK):</b> The included dockerfile will provision a Linux container with all the comonents required to run the Prometheus API either in the cloud or in a local docker instalation. By default, the flask REST api listen in the port 80.</li>
        <li><b>Python:</b> 3.5.4. The file Prometheus.api\conda_requirements.yml contains all the packages that have to be present in the Conda Environment.</li>
        <li><b>CUDA:</b> If GPU is used, then NVidia dockers support should be present.</li>
        <li><b>Intel Math library:</b>Follow instructions on Microsoft CNTK installation to install the library.</li>
        <li><b>.NET Framework:</b> 4.5. Visual Studio is required to compile the WPF application. All required packages are included in Packages folder.</li>
        <li><b>Azure Workbench: </b>Optional.</li>
    </ul>
</p>
<h3>
    Installation
</h3>
<p>
    <ol>
        <li><b>Build the docker container: </b>Build the docker image using the dockerfile provided. A ready to use version is available in Docker Hub under the tag santiagxf/prometheus.0.9.0.</li>
        <li><b>Download the solution and build:</b> Download the solution and build it.</li>
        <li><b>Configure the service:</b> Open the file Prometheus.Planner.Console\App.config and point the configuration key prometheusWebServiceUrl to your hosting URL. By default we are running on http://localhost/. The endpoint for scoring images is /score</li>
        <li><b>Deploy the Azure functions (optional):</b> If you want to use the SMS alerts capabilities, you need to deploy the Azure Functions and configure a Twilio account to process them.
        <li><b>Run the service:</b>Run the container publishing the port 80.</li>
        <li><b>Run the Windows App: </b>Run the WPF application you compiled or use Visual Studio, Debug, Start new instance.</li>
    </ol>
</p>

