Sunfish allow plugin creation too add more functionality through service types.

For details see Sunfish Text Plugin solution.

# Create new plugin

Create a new C# library project, the assembly name should start with "sf-" and use Sunfish.exe as reference.
If the Sunfish reference is set as project, also copy the $sunfish directory

If the resulting assemlby is not a .dll and not starts with sf- Sunfish will not load it.

Also is hight recomendable to set the startup program on debug as the Sunfish.exe inside bin/debug folder.

Create a class that inherits SunfishService and implement Process method.

The call object contains methods for accessing request and response.

# Use the plugin

Place the plugin on the same path ans sunfish.exe

# Notes

Newtonsoft's Json.NET is available for plugins.