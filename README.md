# How to use OSCADSharp

First off, if you don't have OpenSCAD, it's a prerequisite for using OSCADSharp.  Head on over to their downloads page and install it on your computer!

http://www.openscad.org/downloads.html



If you're not familiar with OpenSCAD you might also want to head on over to the OpenSCAD Documentation base to get an understanding of what OSCADSharp is meant to leverage.

http://www.openscad.org/documentation.html


In Visual Studio, you can add OSCADSharp to any project with the following NuGet command:

Install-Package OSCADSharp_0.6

## Basic Objects

OSCADSharp supports a subset of of OpenSCAD's capabilities, including the three basic solids (Cube, Cylinder, Sphere) and Text.

You can create these solids by instantiating them and passing attributes into the constructors in much the same way you would call an OpenSCAD function.

```CSharp    
Cube cube = new Cube(10 /*length*/, 7 /*width*/, 9 /*height*/);
```

```CSharp
Cylinder cylinder = new Cylinder(20 /*diameter*/, 10 /*height*/, true /* center=true*/);
```

```CSharp
Sphere sphere = new Sphere(15 /*radius*/) { Resolution = 30 };
```

```CSharp
Text3D text = new Text3D("Are we Human, or are we Dancers?", 12 /* font size */);
```

In OSCADSharp all objects that represent nodes in OpenSCAD inherit from OSCADObject.  The result of .ToString() on these objects produces the OpenSCAD script generated by the object.  In this way calling .ToString on the above objects will yield their direct OpenSCAD counterpart, as demonstrated below.

cube.ToString() =

```CSharp
cube(size = [10, 7, 9], center = false); 
```


cylinder.ToString() = 

```CSharp
cylinder(center = true, r1 = 10, r2 = 10, h = 10);
```


sphere.ToString() = 
```CSharp
sphere(r = 7.5, $fn = 30);
```


text.ToString() =
```CSharp  
linear_extrude(height = 1) 
{  
    text("Are we Human, or are we Dancers?", size = 12, halign = "center", valign = "center");  
}  
```

With the exception of Text3D (which is two operations), most objects and methods in OSCADSharp are directly analogous to an OpenSCAD method.

## Applying Transforms
Creating objects is pretty neat, but you'll need to do a lot more than that to create functional 3D models.  Luckily OSCADSharp supports almost all of the transforms in OpenSCAD.

Supported transforms (as of 3/6/2016) are:
* Translate
* Rotate
* Scale
* Color
* Resize
* Mirror
* Hull
* Minkowski

Transforms are used by calling methods on OSCADObjects.  Every transform method returns and OSCADObject that is the result of the transform.  

In this way you could chain transforms in a big stack of method calls, like so:

```CSharp
//A cylinder that's translated, rotated, then scaled (in that order).
OSCADObject cylinder = new Cylinder(20, 10, true)
                .Translate(0, 0, 10)
                .Rotate(90, 0, 0)
                .Scale(1, 1, 2);
```

Likewise you can use transforms as single statements, but you need to assign the result to a variable in order to use it.

```CSharp
OSCADObject cube = new Cube(10, 7, 9);
//This statement does nothing
cube.Resize(5, 5, 5);
//This statement assigns a resized cube to a variable
OSCADObject resizedCube = cube.Resize(5, 5, 5);
```

The Color transform only supports OpenSCAD's string-based colors.  RBG values aren't supported out of sheer laziness.  You can view the guide in the OpenSCAD documentatino for a full list of the options:
https://en.wikibooks.org/wiki/OpenSCAD_User_Manual/The_OpenSCAD_Language#color

There are many options, and the usage is what you might expect:

```CSharp
var mySeagreenName = new Text3D("Mike").Color("Seagreen");
```

The Hull and Minkowski transforms are actually block statements that require more than one OSCADObject to perform, so the only difference here is that you'll need to pass in another object for them to work.

```CSharp
var myHull = new Cube().Hull(new Sphere().Translate(0, 5, 5));
var myMinkowski = new Text3D("Hi!").Minkowski(new Sphere());
```


## Boolean Operations
As with boolean operations in OpenSCAD, boolean operations in OSCADSharp require an initial OSCADObject, and at least one additional OSCADObject to perform the operation on.

All boolean operations are supported in OSCADSharp, these are:
* Union
* Difference
* Intersection

The Union operator merges two objects together.  Which is actually a lot more useful in OSCADSharp because it is the only way to combine multiple OSCADObjects into a block.  Because this operation is so fundamental, the + operator on OSCADObjects is overloaded to perform a Union on them.

As such, these two statements are equivalent.

```CSharp
OSCADObject myUnion1 = new Cube(10, 7, 9).Union(new Sphere(15));
OSCADObject myUnion2 = new Cube(10, 7, 9) + new Sphere(15);
```

Union and all other boolean operations support passing multiple parameters.

```CSharp
OSCADObject myUnion3 = new Cube(10, 7, 9).Union(new Sphere(15), new Text3D("Word"), new Cylinder());
//The + operator is also smart enough to combine chained operations into a single Union
OSCADObject myUnion4 = new Cube(10, 7, 9) + new Sphere(15) + new Text3D("Word") + new Cylinder();
```

As you might expect, Difference subtracts two objects from each other. The same syntax as Union is supported, and much like Union, the - operator is overloaded to perform Difference operations when used on two OSCADObjects.

```CSharp
var difference = new Cube() - new Sphere();
var difference2 = new Cube().Difference(new Sphere());
```

Intersection is the same, but there is no operator shorthand.
```CSharp
var intersect = new Cube().Intersection(new Sphere());
```

## Using OSCADSharp Variables
The Variable class in OSCADSharp is analogous to variables in OpenSCAD.  Variables have no functional purpose outside of making your generated script easier to manipulate, or to potentially support Thingiverse Customizer functionality somewhere down the road.

The main things you need to know about Variables are as follows:
* After being instantiated, variables must be bound to properties on OSCADObjects
* In order to appear in script output, variables must be added to Variables.Global, which appear at the top of your generated files
* You can perform dynamic operations on variables, as long as it's valid for the data type

There are multiple ways to use Variables, but the simplest form is as follows:
```CSharp
var diameter = new Variable("diam", Inches.Half, true /*Automatically adds to globals*/);
var sphere = new Sphere(diameter);
```

In a slightly longer version of the same code, I could skip the true flag which automatically adds my variable to globals, and bind the sphere's diameter property manually.

```CSharp
var diameter = new Variable("diam", Inches.Half);
Variables.Global.Add(diameter);

var sphere = new Sphere();
sphere.Bind("Diameter", diameter);
```

The .Bind method is important because it's the only other way to bind Solids outside of the constructor that supports Variables, which has optional parameters for every possible variable binding.

Luckily variable bindings for Transforms like Translate, Rotate, Scale (etc) are a little bit more fluent, because every possible permutation of their usage is supported.  Take this cube for example, which uses literals for its constructor, a variable for a translation, then two literals and one variable for a rotation.

```CSharp
var zRot = new Variable("zRot", 90, true);
var translation = new Variable("upMovement", new Vector3(0, 0, 15), true);
var cube = new Cube(15, 15, 10)
    .Translate(translation)
    .Rotate(0, 0, zRot);
```

You can use variables inline with multiple operators to produce relativistic values.  Use variables to define the values that will be crucial to the overall structure of your 3d objects, then leverage mathematical operators on your variables to produce relative values for everything else.

```CSharp
//Up at the top of your code file...
private static Variable wheelThickness = new Variable("wheelThickness", Inches.Eigth + Inches.Sixteenth, true);
private static Variable wheelDiameter = new Variable("wheelDiameter", Inches.ToMillimeters(1.5), true);

//Much later on, after lots of code...
//Note that in your output script, the length of your cube will literally read "wheelDiameter * 2 - 4",
//BUT the actual Cube object in OSCADSharp will know the numerical value as well.
OSCADObject top = new Cube(wheelDiameter * 2 - 4, wheelDiameter + 4, wheelThickness)
{
    Center = true
};
```

## Writing to File
Writing to file is the simplest step by far.  Combine all objects together, then simply call .ToFile("filenName.scad") to write to disk. 

The previous example for variables can be saved like so:
```CSharp
var zRot = new Variable("zRot", 90, true);
var translation = new Variable("upMovement", new Vector3(0, 0, 15), true);
var cube = new Cube(15, 15, 10)
    .Translate(translation)
    .Rotate(0, 0, zRot);
cube.ToFile("myCube.scad");
```

Which yields a script file named myCube.scad in your output directory with this content:

```CSharp
/*Code Generated using OSCADSharp on 3/7/2016 12:13:39 AM. 

For more information, please visit https://github.com/Exolun/OSCADSharp */

upMovement = [0, 0, 15];
zRot = 90;

rotate([0, 0, zRot])
{
    translate(v = upMovement)
    {
        cube(size = [15, 15, 10], center = false); 
    }
}
```

Neat!

Try the following methods as well on the object returned from .ToFile as well to automatically open or create your .STL files after writing to .scad file.

```CSharp
//.ToFile returns a FileInvoker
var file = cube.ToFile("myCube.scad");

//Calling .Open() launches OpenSCAD
file.Open();

// Calling .CreateModel uses the OpenSCAD command line to render and generate a .stl
// NOTE: This operation may take a significant amount of time.
file.CreateModel("myCube.stl");
```

## Special Features
This section coming soon.  Featured methods are:
* OSCADObject.Position()
* OSCADObject.Bounds()
* OSCADObject.Clone()
* OSCADObject.Children()
* OSCADObject.Name
* OSCADObject.Id