Pre requisite : The API makes use of Redis Cache and dockerfile for deployment and loadbalancing

================================================================================================

Installing Redis
You can download a copy of Redis Cache from GitHub.

While installing, you should check the option to add Redis to the PATH environmental variable. 
Once Redis Cache is installed in your system, you can type 
Run -> service.msc to see the Redis service running in your system.

API uses the ServiceStack C# Redis open source client. You can install ServiceStack.Redis via 
the NuGet Package Manager.

Downloading redis cache
1) use link - https://github.com/MicrosoftArchive/redis/releases
2) use msi in repository


Running Docker Services (Ensure docker for windows/other platform is installed)
Use the following commands to run mutliple istances for load balancing

docker build -t exchangerateapi-img -f exchangerateapi.dockerfile .

To run one instance
docker run --name exchangerateapi -d -p 5000:5000 -t exchangerateapi-img

To run multiple, use the scale number of your choice
docker swarm init

docker service create --publish 5000:5000 --name exchangerateapi exchangerateapi-img

docker service scale ExchangeRateApi=5

================================================================================================

Assumptions: Only the rates as of date is used and no historic data can be requested. 
And only the configured set of currencies will be handled

================================================================================================

Design: 
The API uses a FixerIoController which makes a call to the FixerApi class which is used to 
request Fixer.Io to get the rates and accordingly update the redis cache during initialization.
When a request is made in the format 
(http://localhost:<<PORT>>/api/fixerio/rates?baseccy=<<CCY1>>&targetccy=<<CCY2>>) 
the API first checks if the cache has the rates for the currency pair and will 
return it if it is available. 

If it is not then the API will query Fixer.Io and get the rates for all configured 
currencies against the base currency EUR which is in config as well. 
The data returned from Fixer.Io is parsed and stored into cache. The cache will contain 
basecurrency vs all currency and all currency vs base currency rates,
if a request for a curency pair that is not available in the cache, the API would use the 
currency rates against EUR for that pair and return the rates and adding to the cache as well.
The API has the ability to process the request synchronously and asynchronously

================================================================================================

Exceptions:  All exceptions will be returned as a JsonResult. This can be updated to include
http error status as well

================================================================================================

Tests: 
The solution includes a Unit test project which tests for invalid currency, valid 
currency and cartesian product of all currency combinations both synchronous and asynchronous

================================================================================================  

TODO:
Add comments in code and documentation for each method

================================================================================================  

