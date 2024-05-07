# cglibs-serialization
A .NET library for serializing to from XML and Json. Store configurations, settings etc. in file system as Xml or Json file.


## Usage

```csharp
using CGlibs.Serialization;

[Serializable]
public class MySettings
{
	public string Name { get; set; }
	public int Age { get; set; }
}


MySettings settings = new MySettings();
settings.Name = "John Doe";
settings.Age = 30;

// Save to file
Convertor.Translate.ToXmlFile(settings, "settings.xml");



// Load from file
MySettings loadedSettings = Convertor.Translate.FromXmlFile<MySettings>("settings.xml");

```