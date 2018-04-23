# Prometheus Web API
<p>This projects contains a REST API implemented in Python using Flask that provides the scoring functionality for the images. The services is called using POST method on the endpoint /score. Use multiform post to submmit your files. </p>

<h3>Installation</h3>
<p>Project can be hosted either in Windows or Linux web server. Installations of the enviroment may vary based on it.<p/>
<ol>
    <li><b>Cognitive ToolKit (CNTK):</b> A machine running CNTK 2.5 with GPU support. GPU can be deativated using configuration files, however is on by default on the given configuration. Follow the steps on https://docs.microsoft.com/en-us/cognitive-toolkit/setup-cntk-on-your-machine to get your environment correctly working.</li>
    <li>
        <p>Special considerations to:
            <ul>
                <li><b>CUDA:</b> If GPU is used, then CUDA 9.X+ should be installed</li>
                <li><b>Intel Math library:</b>Follow instructions on Microsoft CNTK installation to install the library.</li>
            </ul>
        </p>
    </li>
    <li><b>Python version:</b> Ensure you are working with Python 3.5.4</li>
    <li><b>Python packages:</b> Ensure all the packages specified in requirements.txt are installed in your environment. Note that OpenCV may need a manual installation for Python 3.5.</li>
</ol>