# EFK
It's a stack with the components: ElasticSearch (Db), Fluent Bit or FluentD (log collectors), Kibana (Dashboard, graphs).
Useful for output logging for the components in our cluster, for example.
We're choosing FluentBit because it's smaller, more performant and specific for our needs, and enriches data with pod_name, pod_id etc.

We're NOT gonna follow this guide: https://akomljen.com/get-kubernetes-logs-with-efk-stack-in-5-minutes/
But we're gonna read https://kubernetes.io/docs/concepts/cluster-administration/logging/ and decide what to use.
If you're using GKE, you can (almost) automatically set this up, although with FLuentD, see here: https://kubernetes.io/docs/tasks/debug-application-cluster/logging-elasticsearch-kibana/.

If we wanna do everything manually, we will do https://www.digitalocean.com/community/tutorials/how-to-set-up-an-elasticsearch-fluentd-and-kibana-efk-logging-stack-on-kubernetes

Lastly, we will configure the web client .NET app to use Serilog for structured logging.

## Requirements
An entire EFK-stack eats pretty much resources. So to be safe, you need at least 
* Around 2-3ghz for the vCPU total (since you wanna run other stuff too!) depending on the 1 or 3-pod version.
* For RAM? Dunno but consider 1-2gigs for the 1-pod version.

## Choices
The docs state that we can either:
* Use a node-level logging agent that runs on every node. (prefet DaemonSet replica) this is most common.
    * We can use the cloud providers guide to do this: https://kubernetes.io/docs/tasks/debug-application-cluster/logging-elasticsearch-kibana/
    * Or we can do it ourselves.
* Include a dedicated sidecar container for logging in an application pod.
* Push logs directly to a backend from within an application

We shall focus on the node level logging agents. There are a couple of guides:
* https://www.digitalocean.com/community/tutorials/how-to-set-up-an-elasticsearch-fluentd-and-kibana-efk-logging-stack-on-kubernetes
* https://medium.com/@jbsazon/aggregated-kubernetes-container-logs-with-fluent-bit-elasticsearch-and-kibana-5a9708c5dd9a
* https://mherman.org/blog/logging-in-kubernetes-with-elasticsearch-Kibana-fluentd/
* https://prabhatsharma.in/blog/logging-in-kubernetes-using-elasticsearch-the-easy-way/

I elected to compose yamls following the topmost guide for Elastic and Kibana, but for FluentBit I use stuff from the Prabhat guide (Helm) with some modifications.

## EFK installation

We can use an a) image or a b) Helm installation or c) composing our own yamls. The a) is the cheaper variant for testing, not really for production, and b) requires a lot more resources, where c) we got more control. So we go with c).

### 1. Install ElasticSearch Cluster (E of EFK)

We're gonna use the 1-pod version, so it might not suit all production needs! Refer to the guide for the 3-pod version.
* Tune the __elasticsearch-*.yaml__'s to your liking
* Run `kubectl apply -f logging-namespace.yaml`
* Run `kubectl apply -f elasticsearch-service.yaml`
* Run `kubectl apply -f elasticsearch-statefulset.yaml`
* Run this command until you see a mount as **Bound** `kubectl get pvc -n logging`
* Run this command until you see everything and running `kubectl get all -n logging`
* Run `kubectl port-forward es-cluster-0 9200:9200 -n logging`
    * Go to `http://localhost:9200/_cluster/state?pretty` and verify

** Errors? **
* Fix the yamls
    * Perhaps the volume storageclass was wrong. Switch from standard to default, or something else
    * In the yaml CTRL-F for "Tune" and edit
* Delete and try again
    * `kubectl delete -f elasticsearch-service.yaml`
    * `kubectl delete -f elasticsearch-statefulset.yaml`
    * `kubectl delete pvc data-es-cluster-0 -n logging`

#### 2. Install Kibana (K of EFK)
* Tune the __kibana-service-and-deployment.yaml__ to your liking
* Run `kubectl apply -f .\kibana-service-and-deployment.yaml`
* Wait until all are running `kubectl get pods -n logging`
* Run `expose-kibana-pod.ps1` and browse to http://localhost:5601, make sure it works and then exit

#### 3. Install FluentBit (F of EFK)
* run `helm install stable/fluent-bit --name=fluent-bit --namespace=logging --set backend.type=es --set backend.es.host=elasticsearch`
* run `kubectl get pods -n logging` until you see X many running fluentbit pods where X is as many nodes as you got
* Run `expose-kibana-pod.ps1` again
* Browse to http://localhost:5601
    * Create an Index with: `kubernetes_*` and choose @timestamp
    * Wait, Click Discover, and see logs!

#### Notes
Using the 1-pod version it seems like CPU only went up from 7 to 11% across 3 nodes (Azure Standard DSv2), and memory from 17 to 20 %. That's a good deal!
Next time, I'll use the aforementioned guide: https://prabhatsharma.in/blog/logging-in-kubernetes-using-elasticsearch-the-easy-way/ and use Helm with parameters to specialize the installation.

## .NET AspnetCore installation
We're gonna use Serilog. We'll need these nuget packages (I use the preview packages!)
* Serilog.AspNetCore (For asp.net core)
* Serilog.Sinks.Console (Fluentbit catches the console output)
* Serilog.Formatting.Elasticsearch (So that elasticsearch can understand the output in a structured way)
* Serilog.Settings.Configuration (To read appsettings.json)

Pro tip: For non-AspNet .NET Core, but still .NET Core, use Serilog.Extensions.Logging.

# References & Links
* https://andrewlock.net/writing-logs-to-elasticsearch-with-fluentd-using-serilog-in-asp-net-core/
* https://github.com/serilog/serilog-sinks-elasticsearch
* https://github.com/serilog/serilog-sinks-console
* https://www.humankode.com/asp-net-core/logging-with-elasticsearch-kibana-asp-net-core-and-docker
* https://serilog.net/