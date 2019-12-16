# Joked

This project has 2 modes the user can choose between
* Requirement 1: (Random Joke) Display a random joke every 10 seconds.
* Requirement 2: (Curated Joke) Accept a search term and display the first 30 jokes containing that term, with the matching term emphasized in some way (upper, bold, color, etc.) and the matching jokes grouped by length: short (<10 words), medium (<20 words), long (>= 20 words).  

## Getting Started

From the root run 
```
dotnet run --project .\src\Joked.csproj
```
to start the backend and 

```
npm start --prefix .\ui\
```
to start the frontend.

You can view the webpage here 
```
http://localhost:4200/
```
and swager documentation for the backend can be found here 
```
https://localhost:5001/swagger
```
## Running the tests

To run backend tests from the root run
```
dotnet test
```

### Testing notes

* Tests for word count and emphasizing term are very robust.  
* Tests for Display timer and joke count are not functioning and are ingored due to dificulties with testing asyncronous methods.  I'd love feedback if you know how to acheive this.

## How to exercise the requirements

### Requirement 1: Random joke

* On the frontend navigate to http://localhost:4200/#/random and view the random joke. This is piped through signalR from a background service. 
* On the backend you can see this happening on the console from which you run dotnet run in the getting started steps. 

### Requirement 2: Curated Joke

* On the frontend navigate to http://localhost:4200/#/curated and enter a search term.  Click the tab for short, medium or long joke. Note that while the search term remains in the searchbox all occurences of the exact term are highlighted. 
* On the backend you can see the emphasized text in action by calling jokes endpoint with `curate` and `empasize` flags set to true.  In this case the emphasis begins with * and ends with &. 

## Other notes

* Ambiguities
  * What is considered a word for counting?
    * Are numbers considered words?  For example, "There are 7 days in week."  Is "7" considered a word?
    * Is a grawlix (!@#$%) considered a word?  For example, in the phrase "To #*!! with good intentions!"  Is " #*!!"  considered to be a word?
    * Are words with an emphasis dash (--)  in the middle considered 2 words? For example, "One thing's for sure--Degreed is a great place to work."  In this example, is "sure--Degreed" 2 words or 1?
    * Are groups of letters with punctuation in the middle other than an emphasis dash considered 1 word?  For example, is "www.degreed.com" 1 word; and in the phrase "I enjoy a good pick-me-up first thing in the morning" is the string "pick-me-up" one word?      * Are Groups of the same punctuation a word? For example,  !!! and ... .
  * How should I handle search terms with multiple words? 
    * The simple implementation is only replace the full string and this fulfills the requirement, but if seems that it is not helpful as searching for "ice cream" will return words with just cream and just ice.  None of these will be emphasized because it only empasizes exact strings. 
    * The ideal emphasis algorithm would emphasize all the words that resulted in the joke being included in the query. This algorithm is much more complicated and tricky to follow as it gets down to the char level. 
    * In this project I did the simple implementation to keep the project cleaner and easier to follow, plus I believe it fullfills the requirements.  The frontend also employs the simple solution.  If you are intereseted in the challenging algorithm, please check out this branch https://github.com/rsrobinett/joked/tree/OverThoughtIt. I have implemented the IEmphasize class to follow the search algorithm so that the terms resulting in the joke being included in the query are emphasized.  It was a fun and difficult challenge with an impressive set of test cases; I hope you get a chance to take a look at it.

## Dependencies

You will need to have the following at a minimum
* dotnet 3
* angular cli
* node
* npm 

## Afterward

Thanks for looking at my project.
