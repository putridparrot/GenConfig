dotnet pack -c Release
dotnet tool install --global --add-source ./nupkg genconfig

genconfig init
genconfig dev
genconfig test
genconfig prod
genconfig --template api --output appsettings.json dev
genconfig --template ui --output src/assets/config.json dev



genconfig dev
generates
appsettings.json

genconfig --template config.template.json --output src/assets/config.json dev
genconfig --template importer.template.json --output importer.json test

genconfig prod

genconfig init
Scaffolds templates + .env.* files.

genconfig <env>
Generates config using template + env + secrets.

genconfig --template <file> --output <file> <env>
Multi-template support.

genconfig list
Lists:
  available .env.* files
  available templates
  available Key Vaults (from .env.*)
  available Azure DevOps variable groups (if configured)

  genconfig doctor