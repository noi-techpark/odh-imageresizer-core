<!--
SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>

SPDX-License-Identifier: CC0-1.0
-->

# odh-imageresizer-core

.Net Project to resize Images stored on a Amazon S3 Bucket
Using ImageSharp https://github.com/SixLabors/ImageSharp

![REUSE Compliance](https://github.com/noi-techpark/odh-imageresizer-core/actions/workflows/reuse.yml/badge.svg)
[![CI/CD](https://github.com/noi-techpark/odh-imageresizer-core/actions/workflows/main.yml/badge.svg)](https://github.com/noi-techpark/odh-imageresizer-core/actions/workflows/main.yml)

## Project Goals/Requirements:

* .Net Core 5
* Docker

### ImageResizer Functionality

Test Imageresizer with  
https://images.tourism.testingmachine.eu/api/Image/GetImage?imageurl=testbild.jpg&width=300

parameter
imageurl -> name of the image stored on the s3 bucket
width -> scale image to this width
height -> scale image to this height

### Image Proxy Functionality

Test Image Proxy Functionality by passing imageurl (useful for CORS issues)  
https://images.tourism.testingmachine.eu/api/Image/GetImageByUrl?imageurl=https://noi.bz.it/themes/custom/noi_techpark/logo.svg


## Getting started:

Clone the repository

### using Docker

`docker-compose build` build image  
`docker-compose up` starts the appliaction on  
http://localhost:6005/

### using .Net Core CLI

Install .Net Core SDK 5\
go into \odh-imageresizer-core\ folder \
`dotnet run`
starts the application on   
https://localhost:6004/  
http://localhost:6003/

## REUSE

This project is [REUSE](https://reuse.software) compliant, more information about the usage of REUSE in NOI Techpark repositories can be found [here](https://github.com/noi-techpark/odh-docs/wiki/Guidelines-for-developers-and-licenses#guidelines-for-contributors-and-new-developers).

Since the CI for this project checks for REUSE compliance you might find it useful to use a pre-commit hook checking for REUSE compliance locally. The [pre-commit-config](.pre-commit-config.yaml) file in the repository root is already configured to check for REUSE compliance with help of the [pre-commit](https://pre-commit.com) tool.

Install the tool by running:
```bash
pip install pre-commit
```
Then install the pre-commit hook via the config file by running:
```bash
pre-commit install
```
