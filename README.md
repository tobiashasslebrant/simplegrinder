
# Description
Measure webrequests in sync, async or parallel  

## Usage
```
dotnet run -p SimpleGrind.Runner/SimpleGrind.Runner.csproj method url [Parameters...]  
```

## Parameters:  
```
  method               Method for request. [get|post|put|delete]  
  url                  Url used for request  
  -h headers           Headers included in request.  
                         Format "header1=value1;header2=value2"  
  -c cookies           Cookies included in request.  
                         Format "cookie1=value1;cookie2=value2"  
  -j json              Json used by action put and post  
  -b behavior          Request behavior [sync|async|parallel]. Default is async  
  -w wait              Wait between requests in milliseconds. Default is 0s  
  -t timeout           Timeout for each request in seconds. Default is 30  
  -nr numberOfRuns     Number of runs before quitting. Default is 10  
  -nc numberOfCalls    Number of calls for first run. Default is 10  
  -ic increaseByCalls  Increase number of calls between runs. Default is 5  
  -cl connectionLimit  Connection limit. Default is 1000  
  -wu dateTime         Wait to start until datetime (yyyyMMdd hhmmss).  
  -ll loglevel         Loglevel can be FRIENDLY, VERBOSE, REPORT or SUMMARY. Default is Friendly  
                         FRIENDLY is reporting friendly messages with a result grid and summary  
                         VERBOSE is FRIENDLY but with detailed errors  
                         RESULT is only reporting the result grid. Useful when integrating with other tools  
                         SUMMARY is only reporting the summary. Useful when integrating with other tools  
  -li items            Number of log error items to show. Default is 3  
  -ec condition        Exit condition. Will exit 1 when fullfilled  
                         Syntax: [ok|failed|timedout|time|avg|totaltime|totalavg][%|#|=|<|>|!][value|percentage];[...]  
                           Fields ok, failed, timedout, time and avg will test against each run  
                           Fields totaltime and totalavg will compare the aggregated result for all runs  
                           A semicolon (;) will seperate multiple conditions. Each condition will be applies with OR  
                         When a % is used, comparing is doing by percentage  
                           Percentage must be greater than value. (# is the same as % but will compare with  
                           lower than value instead  
                         All time comparisions will be with milliseconds  
                         Example: failed%80 => any run, with 'failed' compared to number of calls > 80 percent  
                         Example: ok#80 => any run, with 'ok' compared to number of calls < 80 percent  
                         Example: failed!0 => any run, with 'failed' not equal 0  
                         Example: totaltime>1000 => total time larger than 1000ms  
  -?                   Show this help
  ```
  
  # Example output
  ```
 ====== Parameters ======
 Executing 10 runs against [GET]http://localhost:5000/healthcheck
 First run starts with 10 calls and increasing by 5 calls between each run
 Each call will have a timeout of 30s and will wait 0ms between each call

====== Result ======
Run             Calls           Ok              Failed          Timed Out       Total Time      Average Time    
1               10              10              0               0               69 ms           -               
2               15              15              0               0               17 ms           -               
3               20              20              0               0               21 ms           -               
4               25              25              0               0               14 ms           -               
5               30              30              0               0               10 ms           -               
6               35              35              0               0               15 ms           -               
7               40              40              0               0               11 ms           -               
8               45              45              0               0               12 ms           -               
9               50              50              0               0               17 ms           -               
10              55              55              0               0               14 ms           -               

====== Summary ======
 A total of 325 calls where made
 Total time is 0 seconds 207 milliseconds
 Average time per run is 20 milliseconds

  ```
