# Cencon

### Description
Cencon is library to simplify to build a CenShare integration in applications written in .NET (both FW and Core).

### Features
* Modularity - all architecture is done using interfaces
* Functional extensibility - default implementation but any1 can do custom implementation – replacement really simple
* Library reusability on projects - the core is designed as generic source data reader, while concrete type materializers are project scoped
* Further custom integration extensibility using interception (data caching, logging, data validation, ...)
* Supports both sync and async approaches
* Custom modullable data querying/filtering - similar to .NET Linq Queryable (e.g. client.Query().AssetsAll().AssetType("product.").Products())
* Developed using .NET Standard - that allows to be used in both good old .NET FW and in new .NET Core

### Get started
See sample to get inspiration to build your own integration.

### For contributors
* Use tabs instead of spaces to indent code in editor.

### Licencing
The code is published using MIT licence which basically means you can read the source code freely to get understand its functionality and possibly for troubleshooting. However, if you use it in your product, you have to clearly state you uses this Software. Read LICENSE.txt file for further detail.