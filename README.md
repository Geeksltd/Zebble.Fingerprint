[logo]: https://raw.githubusercontent.com/Geeksltd/Zebble.FingerPrint/master/Shared/NuGet/Icon.png "Zebble.FingerPrint"


## Zebble.FingerPrint

![logo]

FingerPrint is an authentication plugin using device FingerPrint.


[![NuGet](https://img.shields.io/nuget/v/Zebble.FingerPrint.svg?label=NuGet)](https://www.nuget.org/packages/Zebble.FingerPrint/)

> This plugin makes you able to authenticate users and access to device FingerPrint in Android, iOS, and UWP platforms.

<br>


### Setup
* Available on NuGet: [https://www.nuget.org/packages/Zebble.FingerPrint/](https://www.nuget.org/packages/Zebble.FingerPrint/)
* Install in your platform client projects.
* Available for iOS, Android and UWP.
<br>


### Api Usage

Call `Zebble.Device.Fingerprint` from any project to gain access to APIs.

##### Authenticate:
```csharp
var result = await Zebble.Device.Fingerprint.Authenticate("some reason");
```
##### Check availability:
```csharp
var result = await Zebble.Device.Fingerprint.IsAvailable();
```

<br>

### Methods
| Method       | Return Type  | Parameters                          | Android | iOS | Windows |
| :----------- | :----------- | :-----------                        | :------ | :-- | :------ |
| Authenticate         | Task<FingerprintResult&gt;| reason -> string<br> requestConfig -> FingerprintRequestConfig<br> errorAction -> OnError| x       | x   | x       |
| IsAvailable   | Task<bool&gt; | allowAlternativeAuthentication -> bool | x | x | x |