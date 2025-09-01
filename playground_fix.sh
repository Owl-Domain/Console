#!/bin/sh

if ! which dotnet > /dev/null 2> /dev/null; then
   echo "The Dotnet tool is required for this script."
   exit 1
fi

if [ -e 'src/local/Playground/Playground.csproj' ]; then
   echo "Playground project already exists, no need to create it."
   exit
fi

dotnet new console -o src/local/Playground > /dev/null 2> /dev/null
echo "Playground project created, reload the solution if you already opened it."