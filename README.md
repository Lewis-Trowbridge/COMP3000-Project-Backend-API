# COMP3000 Project - Backend API

## Navigation

```mermaid
%%{ init: { 'flowchart': { 'curve': 'stepAfter' } } }%%
graph
    aggregator[Aggregator API]
    frontend[Frontend application]
    shim[DEFRA Shim service]
    csvs[DEFRA CSVs]
    predictions[Prediction service]
    metadata[AURN station metadata]

    aggregator --- |Historical temperature data| shim
    aggregator --- |Historical PM2.5 data| csvs
    aggregator --- |Future predictions from trained model| predictions
    aggregator --- |Station metadata| metadata
    frontend --- aggregator
```

- [Frontend](https://github.com/Lewis-Trowbridge/COMP3000-Project-Frontend)
- Aggregator (you are here)
- [Predictions](https://github.com/Lewis-Trowbridge/COMP3000-Project-Machine-Learning)
- [Metadata](https://github.com/Lewis-Trowbridge/COMP3000-DEFRA-To-Mongo)
- [Shim](https://github.com/Lewis-Trowbridge/COMP3000-Project-DEFRA-Shim)
