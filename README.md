# Batch Execute

[![Build status](https://ci.appveyor.com/api/projects/status/d1eqkbkgom7hd9xu?svg=true)](https://ci.appveyor.com/project/danielscherzer/batchexecute)

---------------------------------------

This is a Microsoft Windows tool to run shell executable files in batch mode. Often I want to execute a list of CPU or HDD heavy files, like a parity check. If I would start them all at once, CPU and other resources, like HDD access would degrade performance. So I wrote this tool to start them automatically after each other. This tool either waits for one executable file to finish or if enabled it checks for CPU consumption to drop to zero to close it and start the next executable file in the list.

See the [change log](CHANGELOG.md) for changes and road map.

## Features
- Copy/paste file lists
- Option to detect idle applications (no CPU ticks) and close them

## Errors and questions
Please us the GitHub [Issue function](https://github.com/danielscherzer/AutoUpdateViaGitHubRelease/issues/new) to report errors or ask questions.

## Contribute
Check out the [contribution guidelines](CONTRIBUTING.md)
if you want to contribute to this project.


## License
[Apache 2.0](http://www.apache.org/licenses/LICENSE-2.0)