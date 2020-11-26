# DC-ILR-2021-ValidationService
This code will create the thrid part of the ILR pipeline. 
A file is validated for gross errors in File Validation Service and then passed to the reference data service where the ref data is created. The tight file and the ref data is then passed into the validation service that will validate the file according to the ILR specification.
The validation splits into 'for the whole file', 'for each learner','for records across learners'. Different approaches are used when it comes to spinning up remote cluster aware service fabric threads (actors) as a result. This repo will create a service fabric application or a nuget package for the desktop version.

