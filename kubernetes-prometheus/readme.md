# Install Prometheus

It's a monitoring framework for all kubernetes components.
The project and install guide is here:


## Quick install (some fixes from the guide included)

* Choose a temporary folder for where to store the cloned git repo
* `git clone https://github.com/coreos/prometheus-operator.git`
* `cd prometheus-operator`
* `cd contrib`
* `cd kube-prometheus`
* `kubectl create -f manifests/`
* `kubectl apply -f manifests/` to be sure
* `kubectl --namespace monitoring port-forward service/grafana 3000` (changed from svc/ to service/) the guide was wrong
* browse to `http://localhost:3000`  login: admin password: admin
* checkout the dashboards
* the guide provides more steps

## Make accessible from outside

There is the NodePort that can be used somehow.
But to expose it with a real url and make it accessible via a route via the ingress, follow this guide:
https://github.com/coreos/prometheus-operator/blob/master/contrib/kube-prometheus/docs/exposing-prometheus-alertmanager-grafana-ingress.md

* For the __htpasswd__ part, use http://www.htaccesstools.com/htpasswd-generator-windows/ to generate a file.
* Use that file and set a secret in the `monitoring` namespace with this file.

Not done this yet, but will add steps here later.