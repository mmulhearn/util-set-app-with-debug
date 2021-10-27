# Introduction
This .NET forms application was created in an attempt to automate debug DLL referencing for your own nuget packages.

It is assumed you have a solution that holds shareable code that is ultimately nugeted and installed into other projects (without using a monorepo).  In this setup, you are sharing code, but you're referencing Release DLLs, which can make debugging an issue painful if the bug might be coming from the nugeted code.

This appliation will allow you to attempt to copy Debug DLLs from your shared code solution into the consuming project so you can easily step into and debug all your code.  It also has the capability to easily and quicky revert back to the original DLLs that were in the packages.

# Use
This application is free to use with no restrictions.  The author does not make an guarantees and does not assume any responsibility.  Use at your own risk.

# Known Issues
This project is not perfect and surely has issues and bugs.  Though there are none to call out currently, you cannot assume this is a perfect application.

# Contributing
If you would like to contribute, you can clone the code and submit a PR to fix an issue.  In this case, please create an issue for reference in your PR description.