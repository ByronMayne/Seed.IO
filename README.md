# Seed.IO
Helper Library that contains an array of useful classes and helpers for working with IO in C#.

- [Seed.IO](#seedio)
- [File Paths](#file-paths)


# File Paths
Strings are used as the go to type when working with file paths in code however they have a bunch of issues. 
* Which direction to the braces go?
* Does this path end with a slash or not?
* Is this argument support a relative path or does it requires absolute?
* How do I get the parent directory.

`AbsolutePath` and `RelativePath` try to fix this issues for you. Below is some of the things they can do to help you make less mistakes when it comes to IO.

## Concatenation
Combining paths is one of the best features, you can never get the slash direction working
``` csharp
AbsolutePath images = AbsolutePath.Parse("C:\\");
images \= "Images";
images \= "Cats" \ "On Holidays";
WriteLine(images);

// output: C:\\Images\Cats\On Holidays
```
Both the path objects have overload the division operator to make concatenation brain dead simple. You can get the brace is the wrong direction if you never type them out. 
## Path Comparison
All paths are normalized behind the scenes making comparison consistent. Below we have two paths with diffent casing and different seperators. 

```csharp
AbsolutePath leftString = new AbsolutePath("C:\\data");
AbsolutePath rightString = new AbsolutePath("c://DATA");

WriteLine($"The two paths are equal {leftString == rightString}");

// output: true
```
Paths are also resolved so any '..' are removed.

```csharp
AbsolutePath leftString = new AbsolutePath("C:\\data");
AbsolutePath rightString = new AbsolutePath("C:\\data\\images\\...");

WriteLine($"The two paths are equal {leftString == rightString}");

// output: true
```

## Multi Platform 
The direction of the seperators changes depending on your current platform 

``` csharp
// Enter a relative path with mixed separators
RelativePath path = new RelativePath("./docs\dogs");
// On Windows
path.ToString(); // .\docs\dogs
// Unix
path.ToString(); // ./docs/dogs 

// Note: AbsolutePaths are based off their root. 
```
> On Windows all paths are case insensitive but on linux systems they are case sensitive.


## Getting Relative Paths
You can get relatives path simply with one function call. 
```csharp
AbsolutePath left = "C:\\data\\graphs\\graph.png";
AbsolutePath right = "C:\\data\\images\\";
RelativePath relative = left.GetRelative(right);
WriteLine(relative);

// output: ".\..\images"
```
> If the paths don't share a common root and exception will be thrown. If you are not sure you can always use `TryGetRelative` instead. 