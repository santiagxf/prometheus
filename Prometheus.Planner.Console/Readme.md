# Prometheus Planner Console

A .NET application is provided with all the features for planning, tracking and analyzing results from flights. Since Prometheus is drone agnostric, we do not provide a way to create a flight plan for auto-piloting. You should use the app shipped by your drone provider. Prometheus Planner console can be used to plan which areas are of interest to create a flight plan, analyze and track the progress. Work is in progress to provide support for flight plan creation using the DroneKit open-source project. However, drone owner may still want to use the one shipped with the drone.

<h2>Configuration</h2>
<p>The app is connected with a suite of services in the cloud, including:
    <ul>
        <li>Prometheus REST-API: This is the service where the Prometheus Deep Learning algorithm is implemented. It is deployed as a docker container.</li>
        <li>NASA FIRMS Radar: The Fire Information for Resource Management System (FIRMS) distributes Near Real-Time (NRT) active fire data within 3 hours of satellite overpass from both the Moderate Resolution Imaging Spectroradiometer (MODIS) and the Visible Infrared Imaging Radiometer Suite (VIIRS).</li>
        <li>NowCoast NOAA Oceanic Observations: This service is used to provide Mean Sea Level Pressure visualizations in the map. This observations are important as they are not "forecasted". They are real readings from stations deployed all around the world.</li>
        <li>NWS: National Weather Services provides information to undestand and display Red Flag Alerts in the US</li>
        <li>INTA: National Institute of Agricultural Techinology provides information to undestand and display Red Flag Alerts in Argentina.</li>
    </ul>
</p>
<p>
The file app.config.debug contains the configuration you have to configure in order to use the app. To use it rename the file to app.config, open the file with Visual Studio and configure all the indicated data. You will have to provide keys and URLs in some cases depending on the specific deployment you are using. 
</p>