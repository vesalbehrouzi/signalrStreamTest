# signalrStreamTest
Simple tets application for streaming in Asp.Net Core With SignalR

to run the test application using docker, run this commands in the root of project:
```
docker build -t signalrtestapp .
docker run -d -it --rm -p 5000:80 signalrtestapp
```

then navigate to : http://localhost:5000 to see the demo client
