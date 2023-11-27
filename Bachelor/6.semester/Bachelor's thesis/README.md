# About
Author: Jan Lorenc  
Solution: RQA System Anomaly Detection - Bachelor's thesis  
University: Brno University of Technology  
Faculty: Faculty of Information Technology  

# Implementation overview
In the ./src directory can be found the source code of Y Soft's RQA Anomaly detection.  
The solution AnomalyDetection.sln contains 5 projects:
* AnomalyDetection - Executable web api application to control downloading logs from Graylog. Extendable to control whole anomaly detection via a web api.
* AnomalyDetection.Application - Library for running anomaly detection. Contains classes to perform cluster analysis and statistical computations.
* AnomalyDetection.Client - Small project generating and providing NSwag client for AnomalyDetection web api.
* AnomalyDetection.Data - Library for downloading logs from Graylog and transforming the data to a dataset (csv) suitable for further AI analysis.
* AnomalyDetection.Metrics - Library providing middlewares collecting the data.

Each of the libraries is packable to a nuget package and the usage is quite simple:
* If we have a service which we want to include in the anomaly detection, we just install AnomalyDetection.Metrics nuget package, register the middlewares provided and it's done.
* If the web api is running somewhere and we want to send requests to it in C# code, we install AnomalyDetection.Client nuget package and simply use the client's methods to make the requests.
* If we want just the anomaly detection by itself (which is also the case of the demo, see below), we just install the AnomalyDetection.Application nuget package and use whatever we want. The package references AnomalyDetection.Data ... the installation of AnomalyDetection.Data nuget is not necessary (it will be done automatically by AnomalyDetection.Application), it has to exist though.

To pack a project, do one of the following:
1) Open the solution in Visual Studio, right-click on a project and select Pack.
2) Run CLI command: dotnet pack &lt;path to project&gt; -o &lt;path to nuget source directory&gt;

To be able to install the local nuget package, it needs to be in a nuget source directory. To create one, do one of the following:
1) Open Visual Studio. Navigate to Tools->Options. In there go to NuGet Package Manager->Package Sources. There you can add/modify/delete available nuget sources.
2) Run CLI command: dotnet nuget add source &lt;directory path&gt; [--name &lt;source name&gt;]  
  
The solution is prebuilt in ./build directory and the nuget packages are already packed and can be found in ./nuget directory.

# DOCUMENTATION
The thesis itself can be found in ./doc directory. It also contains LaTex source files.

# DEMO
How the anomaly detection works demonstrate several notebooks in ./demo directory. These are standard Jupyter notebooks, however, as it's implemented in .NET, .NET Interactive tool and a .NET kernel are required for it to run.  
  
At the time of writing the theses the following 2 commands should suffice to install it:  
dotnet tool install -g --add-source "https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3/index.json" Microsoft.dotnet-interactive   
dotnet interactive jupyter install  
For more info follow the intructions on: https://github.com/dotnet/interactive  
  
As described in the theses, two forms of anomaly detection are performed. One is in errors during the service requests and it's demonstrated in _"error\_anomaly.ipynb"_. Second is in request durations and it's described in _"duration\_anomaly.ipynb"_. In addition to that, to show the variability of the detection, a third notebook _"random\_traffic.ipynb"_ generates random traffic and detects duration anomalies in it.  
  
Moreover, there are several example CSV files with generated traffic in ./demo/csv\_examples directory to demonstrate the format of the data. To be able to view those in other fashion than just CSV lines, notebook _"csv\_examples.ipynb"_ is prepared.

# Graylog
At the current state the downloading wouldn't work. The data collection from thesis is indeed implemented and functional. However, it can be noticed that in appsettings.json file in GraylogConfiguration section the login and password are missing. It's for security reasons as Y Soft is private company. Moreover, Graylog runs on a private network so the credentials wouldn't help anyway so it's another reason for not exposing them. Should you want try it, you would need to set up your own Graylog and pass respecting information to the configuration (mind that the stream ids would be different).

There are several reasons for not preparing Graylog:
1) It's not even possible to just pack Graylog in zip od burn on CD/DVD.
2) Having it running somewhere just in case someone would try the solution in a few years is just wasteful, not to mention pricy.
3) It's not in thesis requirements. The goal was to design data collection, not to implement it. The only implementation required is that of the data analysis. Therefore the fact it really is implemented is somewhat added value and somehow setting up Graylog would be unnecessary overhead.