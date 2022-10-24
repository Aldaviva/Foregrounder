Foregrounder
===

[![Nuget](https://img.shields.io/nuget/v/Foregrounder?logo=nuget)](https://www.nuget.org/packages/Foregrounder/)

*Bring a window in your process to the foreground, even if a different process's window is currently in the foreground.*

## Requirements
- Windows
- A .NET desktop runtime, such as
	- .NET 5 or later
	- .NET Core 3.1 or later
	- .NET Framework 4.5.2 or later

## Installation
```ps1
dotnet add package Foregrounder
```

## Usage
```cs
Foregrounder.Foregrounder.BringToForeground(myWindow);
```

`myWindow` can be

- a `Window` from WPF
- a `Form` from Windows Forms
- an `AutomationElement` from [UI Automation](https://learn.microsoft.com/en-us/windows/win32/winauto/entry-uiauto-win32)
- a raw `HWND` window handle pointer

## Acknowledgements
- [Joseph Cooney](https://jcooney.net) for the [original article and C# implementation](https://learnwpf.com/post/2011/08/03/Adding-a-system-wide-keyboard-hook-to-your-WPF-Application.aspx)
- [Carl "Zodman" Scarlett](https://www.redperegrine.net) for the [StackOverflow answer](https://stackoverflow.com/a/11552906/979493) that this package is based upon