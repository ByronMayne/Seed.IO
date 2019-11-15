# Seed.IO
Helper Library that contains an array of useful classes and helpers for working with IO in C#.

- [Seed.IO](#seedio)
- [File Paths](#file-paths)


# File Paths
Working with file paths is always a pain. There is a bunch of questions that come up every time. `AbsolutePath` and `RelativePath` are here to help save you time. 

``` csharp
public void BuildFaster()
{
    // Converts that path behind the scenes 
    AbsolutePath path = new AbsolutePath("C://docs"); 

    // Override the '/' for easy appending. 
    path /= "images";
    path /= "cats"; 

    path.ToString() // C://docs/images/cats 
}

// Feel free to mix forward and back slashes as they will be fixed.
public void MultiPlatform()
{
    // Enter a relative path with mixed separators
    RelativePath path = new RelativePath("./docs\dogs");
    // On Windows
    path.ToString(); // .\docs\dogs
    // Unix
    path.ToString(); // ./docs/dogs 

    // Note: AbsolutePaths are based off their root. 
}

// When paths are created they are normalized and all navigation is 
// resolved.
public void ResolveNavigation()
{
    RelativePath path =  new RelativePath("./docs/images/dogs/../cats"); 
    path.ToString(); // ./docs/images/cats 
}

// Now that paths are normalized it's easy to compair 
public void Compair()
{
    string pathOne = "./images/dogs/../cats";
    string pathTwo = ".\images\cats\"; 

    string.Equals(pathOne, pathTwo); // FALSE :( 
    new RelativePath(pathOne) == new RelativePath(pathTwo) // TRUE :) 
}
```

No more getting angry when it comes to paths. 

