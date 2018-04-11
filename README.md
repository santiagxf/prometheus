# Project Prometheus

<p>An early fire detection AI technology using drones. Named after Prometheus, the Greek TItan, who defies the gods by stealing fire and giving it to humanity, an act that enabled progress and civilization. We are itching to do the same.</p>

<p>We combine surveillance drones and concepts of machine learning to detect wildfires while they are still in their early stages. The concept is to leverage the vast amounts of wildfire images and videos available on the internet to train a machine learning algorithm to detect the presence of a fire, however small it may be. The drone would then fly over parks and forests collecting the images. An algorithm  will determine the confidence level at which it believes a region is about to develop a fire.
</p>

<h2>
    Instalation instructions
</h2>
<h3>
    Prerequisites
</h3>
<p>
    <ul>
        <li><b>Cognitive ToolKit (CNTK):</b> A machine running CNTK 2.5 with GPU support. GPU can be deativated using configuration files.</li>
        <li><b>Python:</b> 3.5.4. The file PrometheusWS\requirements.txt contains all the packages that have to be present in the Python Environment</li>
        <li><b>CUDA:</b> If GPU is used, then CUDA 9.X+ should be installed</li>
        <li><b>Intel Math library:</b>Follow instructions on Microsoft CNTK installation for further details.</li>
        <li><b>.NET Framework:</b> 4.5. Visual Studio is required to compile the WPF application.</li>
        <li><b>Azure Workbench: </b>Optional.</li>
    </ul>
</p>
<h3>
    Installation
</h3>
<p>
    <ol>
        <li><b>Download the solution and build:</b> Download the solution and build it. Ensure that the project PrometheusWS is using Python environment with support for CNTK.</li>
        <li><b>Configure the service:</b> Open the file Prometeo.Planner.Console\App.config and point the configuration key prometheusWebServiceUrl to your hosting URL. By default we are running on http://localhost:55555/. The endpoint for scoring images is /score</li>
        <li><b>Run the service:</b>Run the service and make it available.</li>
        <li><b>Run the Windows App: </b>Run the WPF application you compiled.</li>
    </ol>
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
        <li><b>Azure Apps Services:</b> The web service is hosted as a App Service in Azure Cloud.</li>
        <li><b>Visual Studio: </b>The proposal is modeled as a solution in Visual Studio, containing 3 projects: The FireDetection experimentation project - used with Azure Experimentation Service for training-, the Prometheus App - a WPF Windows application- and a web API implemented using Flask with Python for scoring.</li>
    </ul>
</p>

