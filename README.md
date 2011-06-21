#Atos Origin Kinect
This Kinect application framework and demo apps are made by Marco Franssen and Jan Saris. During competence development at their company Atos Origin they started on februari 2011 by creating some apps and a framework with Microsoft Kinect. They started with a implementation in c#/WPF using the PrimeSense drivers and OpenNI SDK.

#Contributors
* Marco Franssen - Founder
* Jan Saris - Founder
* Rein Steens - Implementer Pong
* Erwin Wolff - Created installer for PowerPoint plugin
* Pieter-Joost van de Sande - Gave some advice on the architecture

#Goal
Our goal is to create a framework for developers on top of Kinect. This Framework should support easy access to gestures etc. All the demo apps we created to test our framework are included. We would love it when you guys help to extend and improve the framework. Don't forget to add your own demo apps so you can show of all the cool stuff you made to the other developers participating.

#How to fork
1. Fork the `Kinect` repo
Fork the kinect repo by clicking the Fork button on https://github.com/atosorigin/Kinect

2. Clone the `Kinect` project
Run the following code:
$ git clone git@github.com:username/Kinect.git

3. Configure remotes
When a repo is cloned, it has a default remote called origin that points to your fork on GitHub, not the original repo it was forked from. To keep track of the original repo, you need to add another remote named upstream:\n
`$ cd Kinect` the active directory in the prompt to the newly cloned "Kinect" directory\n
`$ git remote add upstream git@github.com:atosorigin/Kinect.git` Assigns the original repo to a remote called "upstream"\n
`$ git fetch upstream`\n

4. Pushing
Pushing is always done to you fork\n
`$ git push origin master`\n

5. Fetch the upstream with your fork
When you want to get the latest changes fetched from the upstream into your fork, you need to fetch the upstream.\n
`$ git fetch upstream`\n
`$ git merge upstream/master`\n

#Installation
Need to add an installation manual here
