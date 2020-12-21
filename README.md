# Voxel-PN

Voxel - Petri Nets

Voxel-PN seeks to use the Petri Net concept from the field of Discrete Event Systems to model the behavior of a digital 3D printing system system, and evaluate the reachability of various structures that could be printed.

This concept, which I hope to explore in more depth in later work, is derived from the research done by the "Creative Machines Lab" at Columbia University on "cellular machines". The digital 3D printing system consists of depositing layers of 3D "cells" that are bonded together to form a lattice structure, rather than continuously printing material as is done in traditional 3D printing. These layers can have gaps, which are left intentionally to influence properties of the resulting object, such as density, pliability, and permeability. The aim of my work in Voxel-PN is to design a system to evaluate which voxel lattice structures would be feasible based on a variable set of constraints. While I will not explore every type of constraint structure, I hope that this work will serve as an infrastructure for easily modeling future constraints.

This repository is still a work in progress. There are a few tasks left to complete before version 1.0 is finished:
* Trim Petri Net config file, removing unecessary fields
* Add command line arguments to pn_eval.py for input and output files
* Overlay semi-transparent previous layer deposited in the voxel builder GUI
* Add some quality of life updates to voxel builder, including: undo layer, unclick voxel button, improve orbit camera

## Dependencies
Python 3.9.0

Unity 2019.4.15f1

## Running
"python pn_eval.py" evaluates the patterned Petri Net specified by the filename passed into readPNConfig() inside pn_eval.py with the commands in the file "voxel_structure.csv". It outputs the resulting achievable commands to "voxel_structure_out.csv".

The Unity project (also called Voxel-PN) has only been run in editor mode so far. It exports a command sequence called "voxel_structure.csv" to and imports a command sequence "voxel_structure_out.csv" from it's root directory.
