# replace-in-file

A .NET Global tool for replacing placeholders in files.

## How to install?

To install it, run the following command in CMD:

```
dotnet tool install -g replace-in-file
```

## How to use?

In command line, you can invoke this tool on any file using the following syntax:

```
replace-in-file "file-path.txt" | "placeholder1" "value1" | "placeholder2" "value2" | ...
```

It will simply load the file as text into memory, replace each of the specified placeholders with the provided value, and overwrite the file.
