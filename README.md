# CrossArc
[![Build status](https://ci.appveyor.com/api/projects/status/drd2v75fe4mqm4po?svg=true)](https://ci.appveyor.com/project/Ploaj/arccross) 

A file extractor for Smash Ultimate's ARC file. 

**This project is no longer actively maintained and has been replaced by [ArcExplorer](https://github.com/ScanMountGoat/ArcExplorer), which has an updated interface, improved performance and stability, and support for Windows and Linux. It's recommended to only use CrossArc for data.arc files prior to game version 5.0 since these older versions of the ARC will not open in ArcExplorer.**  


### Opening an ARC
Click `File > Open ARC` and then select the appropriate `.arc` file. 

### Extracting files
Right click a file in the main window to extract an individual file. Right click a folder to extract all files in the given folder and all subfolders. Files will be extracted to the application directory, keeping the folder structure intact. 

### Updating file hashes
Click `Update Hashes` and then confirm the download. The latest hashes will be downloaded from the [github repo](https://github.com/ultimate-research/archive-hashes). Note that the current file size is around 4 MB.

# ArcCross
A library for parsing file information from Smash Ultimate's ARC file.
