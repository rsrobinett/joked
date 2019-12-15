# Joked

This project has 2 modes the user can choose between
* 1. Display a random joke every 10 seconds.
* 2. Accept a search term and display the first 30 jokes containing that term, with the matching term emphasized in some way (upper, bold, color, etc.) and the matching jokes grouped by length: short (<10 words), medium (<20 words), long (>= 20 words)

## Getting Started

From the root run 
```
dotnet run --project .\src\Joked.csproj
```
to start the backend and 

```
npm start --prefix .\ui\
```
to start the frontend

checkout the webpage here 
```
http://localhost:4200/
```
and swager documentation for the backend can be found here 
```
http://localhost:5000
```
## Running the tests

To run the test from the root run
```
dotnet test
```
To run the tests for the fron end run
```
npm test --prefix .\ui\
```
## Other notes
* If you are unable to find the desired results on the angular page, please see the console for the output of option 1 in the reqirements and swagger for option 2 of the results.
* There is some ambiguity around the following:
** What is considered a word for counting?
** Should the search term be highlight if the term wasn't what resulted in the joke being included in the search results.  For example, searching for "he" would bring up a joke containing the full word "he", but not "the". Likely "he" and "the" would both be in the joke and should they be highlited as "*he*" and "t*he*", or should just "*he*" be highlighted?




## Dependencies
You will need to have the following at a minimum
* dotnet 3.0
* angular cli
* node
* npm 
