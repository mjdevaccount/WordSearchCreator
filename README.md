# Word Search Creator

A web application that generates word search puzzles. Built with ASP.NET MVC 5, Razor, Bootstrap, and jQuery.

## Setup

1. Clone the repository
2. Open `WordSearchCreator.sln` in Visual Studio 2022
3. Build the solution (F6)
4. Run the project (F5)

## Requirements

- Visual Studio 2022
- .NET Framework 4.8

## How to Use

1. Enter a grid size (e.g., 10 for a 10x10 puzzle)
2. Click "Create Grid"
3. Enter words one at a time and click "Add Word"
4. Click "Rebuild" to regenerate the puzzle with the same words in different positions

## Features

- Words can be placed horizontally or vertically
- Words can overlap if they share a common letter
- Validates words (too long, duplicates, invalid characters)
- AJAX-powered - no page refreshes
- Rebuild generates a new random layout with the same words

## Project Structure

- **WordSearch.Engine** - Core logic (grid generation, word placement)
- **WordSearch.Engine.Tests** - Unit tests (xUnit)
- **WordSearch.Web** - ASP.NET MVC 5 web interface

## Running Tests

Open Test Explorer in Visual Studio and run all tests, or use the command line:
```
vstest.console.exe .\WordSearch.Engine.Tests\bin\Release\WordSearch.Engine.Tests.dll
```