# odh-imageresizer-core

little .Net Project to resize Images stored on a Amazon S3 Bucket
Using ImageSharp https://github.com/SixLabors/ImageSharp

## Project Goals/Requirements:

* .Net Core 5
* Docker

Test Imageresizer with
https://images.tourism.testingmachine.eu/api/Image/GetImage?imageurl=testbild.jpg&width=300

parameter
imageurl -> name of the image stored on the s3 bucket
width -> scale image to this width
height -> scale image to this height

## Getting started:

Clone the repository

### using Docker

'docker-compose build'
`docker-compose up` starts the appliaction on http://localhost:6005/

### using .Net Core CLI

Install .Net Core SDK 5\
go into \odh-imageresizer-core\ folder \
`dotnet run`
starts the application on 
https://localhost:6004/
http://localhost:6003/
