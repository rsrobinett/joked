# Joked

This project has 2 modes the user can choose between
* 1. (Random Joke) Display a random joke every 10 seconds.
* 2. (Curated Joke) Accept a search term and display the first 30 jokes containing that term, with the matching term emphasized in some way (upper, bold, color, etc.) and the matching jokes grouped by length: short (<10 words), medium (<20 words), long (>= 20 words).  I referto these as curated jokes.  

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
https://localhost:5001/swagger
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

##How to exercise the requirements

### Requirement 1: Random joke
* On the frontend navigate to http://localhost:4200/#/random and view the random joke. This is piped through signalR from a background service. 
* On the backend you can see this happening on the console from which you run dotnet run in the getting started steps. 

### Requirement 2: Curated Joke
* On the frontend navigate to http://localhost:4200/#/curated and enter a search term.  Click the tab for short, medium or long joke. Note that while the search term remains in the searchbox all occurences of the exact term are highlighted. 
* On the backend you can see the emphasized text in action by calling jokes endpoint with `curate` and `empasize` flags set to true.  In this case the emphasis begin is * and end is &.  This endpoint emphasizes all terms individiually, it only emphasizes terms in the results that were responsible for the joke being in the results.  For example, if "he" is entered it will highlight the whole word "he", but not the "he" in the word "the".  I recomend hitting ```https://localhost:5001/api/Jokes?term=a%20pass%20sword&curate=true&limit=30&emphasize=true``` to see this in action.

## Other notes
* There is some ambiguity around the following:
** What is considered a word for counting?
** Should the search term be highlight if the term wasn't what resulted in the joke being included in the search results.  For example, searching for "he" would bring up a joke containing the full word "he", but not "the". Likely "he" and "the" would both be in the joke and should they be highlited as "*he*" and "t*he*", or should just "*he*" be highlighted?
* In the case of ambiguities I picked a solution that seemed reasonable, but also challengeing since it is a code *challenge*.  For the case of highlighting a term, I implemented it 2 ways to make sure that I was covering the requirments as stated. The frontend is as stated, the backend is the challenging part.    

* I used dotnet core 3.1 (just released last week). There may be somethings that don't look familiar.

## Dependencies
You will need to have the following at a minimum
* dotnet 3.0
* angular cli
* node
* npm 
