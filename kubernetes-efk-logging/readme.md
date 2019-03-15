# EFK

It's a stack with the components: ElasticSearch (Db), Fluent Bit or FluentD (log collectors), Kibana (Dashboard, graphs).
Useful for output logging for the components in our cluster, for example.

We're NOT gonna follow this guide: https://akomljen.com/get-kubernetes-logs-with-efk-stack-in-5-minutes/
But we're gonna read https://kubernetes.io/docs/concepts/cluster-administration/logging/ and decide what to use.
If we wanna do everything manually, we will do https://www.digitalocean.com/community/tutorials/how-to-set-up-an-elasticsearch-fluentd-and-kibana-efk-logging-stack-on-kubernetes

## Choices

The docs state that we can either:
* Use a node-level logging agent that runs on every node. (prefet DaemonSet replica) this is most common.
    * We can use the cloud providers guide to do this: https://kubernetes.io/docs/tasks/debug-application-cluster/logging-elasticsearch-kibana/
    * Or we can do it ourselves.
* Include a dedicated sidecar container for logging in an application pod.
* Push logs directly to a backend from within an application

We shall focus on the node level logging agents. Therefore we will try this guide: https://www.digitalocean.com/community/tutorials/how-to-set-up-an-elasticsearch-fluentd-and-kibana-efk-logging-stack-on-kubernetes

## Step by step commands

