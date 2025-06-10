# FlexiMetrics

## How to run demo

### Prerequisites
Ensure you have the following prerequisites installed on your system:

```
dotnet
docker
```

### Clone the Repository

```
git clone https://github.com/Vetleskrett/FlexiMetrics
cd FlexiMetrics
```

### Checkout demo branch
The `demo` branch has all authentication/authorization logic removed for demonstration purposes.
```
git checkout demo
```

### Run
```
dotnet run --configuration Release --project Backend/AppHost/AppHost.csproj
```