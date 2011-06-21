#Atos Origin Kinect
This Kinect application framework and demo apps are made by [Marco Franssen][1] and [Jan Saris][3]. During competence development at their company Atos Origin they started on februari 2011 by creating some apps and a framework with Microsoft Kinect. They started with a implementation in c#/WPF using the PrimeSense drivers and OpenNI SDK.

#Contributors
* [Marco Franssen][1] - Founder
* [Jan Saris][3] - Founder
* [Rein Steens][4] - Implementer Pong
* [Erwin Wolff][5] - Created installer for PowerPoint plugin
* [Pieter-Joost van de Sande][2] - Gave some advice on the architecture

#Goal
Our goal is to create a framework for developers on top of Kinect. This Framework should support easy access to gestures etc. All the demo apps we created to test our framework are included. We would love it when you guys help to extend and improve the framework. Don't forget to add your own demo apps so you can show of all the cool stuff you made to the other developers participating.

#How to fork
1. Fork the `Kinect` repo<br />
Fork the Kinect repo by clicking the Fork button on [https://github.com/atosorigin/Kinect][7]<br />

1. Clone the `Kinect` project<br />
Run the following code:<br />
`$ git clone git@github.com:username/Kinect.git`<br />

1. Configure remotes<br />
When a repo is cloned, it has a default remote called origin that points to your fork on GitHub, not the original repo it was forked from. To keep track of the original repo, you need to add another remote named upstream:<br />
`$ cd Kinect` the active directory in the prompt to the newly cloned "Kinect" directory<br />
`$ git remote add upstream git@github.com:atosorigin/Kinect.git` Assigns the original repo to a remote called "upstream"<br />
`$ git fetch upstream`<br />

1. Pushing<br />
Pushing is always done to you fork<br />
`$ git push origin master`<br />

1. Fetch the upstream with your fork<br />
When you want to get the latest changes fetched from the upstream into your fork, you need to fetch the upstream.\n<br />
`$ git fetch upstream`<br />
`$ git merge upstream/master`<br />

1. Pull requests<br />
Don't forget to do a [pull request][6] when you have added value to the project.<br />

[1]: https://github.com/marcofranssen "Marco Franssen's Github profile"
[2]: https://github.com/pjvds "Pieter Joost van de Sande's Github profile"
[3]: https://github.com/jansaris "Jan Saris's Github profile"
[4]: https://github.com/ReinSteens "Rein Steens's Github profile"
[5]: https://github.com/erwinwolff "Erwin Wolff's Github profile"
[6]: http://github.com/guides/pull-requests "Pull request guide"
[7]: https://github.com/atosorigin/Kinect "Atos Origin Kinect Repository"

#Installation
Need to add an installation manual here
