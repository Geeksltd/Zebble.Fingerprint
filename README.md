[logo]: https://raw.githubusercontent.com/Geeksltd/Zebble.Fingerprint/master/icon.png "Zebble.Fingerprint"


## Zebble.Fingerprint

![logo]

Fingerprint is an authentication plugin using device Fingerprint.


[![NuGet](https://img.shields.io/nuget/v/Zebble.Fingerprint.svg?label=NuGet)](https://www.nuget.org/packages/Zebble.Fingerprint/)

> This plugin makes you able to authenticate users and access to device Fingerprint in Android, iOS, and UWP platforms.

<br>


### Setup
* Available on NuGet: [https://www.nuget.org/packages/Zebble.Fingerprint/](https://www.nuget.org/packages/Zebble.Fingerprint/)
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