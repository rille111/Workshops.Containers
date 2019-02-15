echo off
REM Need to use the Hyper-V switch since we're on windows.
REM Also need to put ourselves in C: (same folder as minikube is installed) because it cant find the iso of minikube otherwise
REM Other commands:

REM 
REM 
REM 
REM 

c:
cd\
minikube start --vm-driver hyperv --hyperv-virtual-switch "minikube_network_switch"