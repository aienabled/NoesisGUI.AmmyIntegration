NoesisGUI Ammy Integration
=============
This library provides solution for integration [NoesisGUI v2.0+](http://noesisengine.com) with [Ammy](http://ammyui.com) library.
Currently it provides only building feature - it builds `.ammy` files to `XAML` files for loading them in NoesisGUI.
Building `XAML` files is implemented at `NoesisAmmyBackend` project.
A sample project based on SharpDX is included (Windows only, Direct3D 11, based on NoesisGUI sample). It demonstrates how to use `NoesisAmmyBackend`, it will automatically rebuild `XAML` files when any `.ammy` file is changed and reload NoesisGUI after that.

Prerequisites
-----
* [Visual Studio 2013/2015/2017](https://www.visualstudio.com/), any edition will be fine.

Installation
-----
1. Pull submodule `AmmyBackend` from https://github.com/AmmyUI/Backend
2. Download NoesisGUI v1.3 C# API Windows SDK from [NoesisGUI Forums](http://www.noesisengine.com/forums).
3. Extract it to the folder `\NoesisGUI-SDK\`. The resulting directory tree should look like this:
        
        NoesisGUI-SDK          
          |--Bin
          |--Doc
          |--Samples
          |--Src
        
4. Open `NoesisAmmyBackend.sln` with Visual Studio 2013/2015   
5. Open context menu on `NoesisAmmySampleD3D11` project and select `Set as StartUp Project`.
6. Press F5 to launch the sample project.
7. Please note that the sample project uses `.ammy` files from `SampleWPFAmmy/Data` folder. It builds them to XAML by using `NoesisAmmyBackend` project. It uses supporting C# classes for these `.ammy` files in the `NoesisAmmySampleD3D11/Data` folder. You could modify `.ammy` files to reload NoesisGUI with the new `XAML` data. But you cannot modify C# files while the project is running (so no dynamic recompilation of C# code).

Sample project notes
-----
It uses a very simple and unreliable mechanism of listening to the file system changes to reload NoesisGUI when any `.ammy` file is modified. It's simply a proof-of-concept and will crash often.

Dependencies
-----
Ammy depends on .NET Framework 4.5. Also it depends on [Nemerle](http://nemerle.com) and [Nitra](https://github.com/rsdn/nitra) (both are .NET 4.5 assemblies). It's expected to work properly on Windows, have not tested on other OSes yet but it might work fine under fresh version of Mono.

Contributing
-----
Pull requests are welcome.

License
-----
The code provided under MIT License. Please read [LICENSE.md](LICENSE.md) for details.
