# Application

## Settings

See [appsettings.json](appsettings.json).

```json
{
  "Kafka": {
    "Socket": "<socket>",
    "MessageMaxBytes": 1048588,
    "MaxPollIntervalMs": 300000,
    "MessageTimeoutMs": null,
    "Clients": {
      "Producers": {
        "1C7": [],
        "1C8": []
      },
      "Consumers": {
        "MsSql": [],
        "1C7": [],
        "Http": []
      }
    }
  }
}
```

Properties:

1. `Socket` - Kafka broker address.
1. (_optional_) `MessageMaxBytes` - the largest record batch size allowed by Kafka.
1. (_optional_) `MaxPollIntervalMs` - the maximum delay between poll requests.
1. (_optional_) `MessageTimeoutMs` - limits the time a produced message waits for successful delivery (use values __<5000__ to guarantee correct work with cache at `api/kafka/produce` endpoint).



### Kafka Producers

#### 1C7

__Producer__ reads infobase log file (__10 000__ KB from the end (it is also the limit) when offset is not specified in _offsets.json_) looking for new transactions that match `ObjectFilters` and `TransactionTypeFilters`.

When transactions are found __producer__ retrieves corresponding __objects JSONs__ from infobase by calling ERT specified  in `ErtRelativePath`.

Then __producer__ adds corresponding transaction data to each of the retrieved __object JSONs__.

Then it sends these __object JSONs__ to Kafka topic which name is specified at `ObjectFilters.Topic` or is generated like `<infobaseName>_<dataType>` where:

* `<infobaseName>` is directory name of the infobase
* `<dataType>` is the value of the property in each __object JSON__ which name is specified in `DataTypePropertyName`

Example:

```json
{
  "InfobasePath": "D:\\path\\to\\infobase",
  "Username": "InfobaseUser",
  "Password": "qwerty",
  "ErtRelativePath": "path\\to\\ert",
  "DataTypePropertyName": "T",
  "ObjectFilters": [
    {
      "IdPrefix": "O/9999/",
      "JsonDepth": 3,
      "Topic": "Topic1",
      "ReadLastOnly": false,
      "TransactionTypeFilters": [
        "RefNew"
      ]
    }
  ],
  "TransactionTypeFilters": [
    "RefNew"
  ],
  "SuspendSchedule": [
    {
      "Start": "23:00",
      "Duration": "6:00"
    }
  ]
}
```

Properties:

1. `InfobasePath` - absolute path to 1C7 infobase.
1. `DataTypePropertyName` - data type property name at JSONs extracted by `ErtRelativePath`.
1. `ObjectFilters` - object filters.
1. `TransactionTypeFilters` - transaction types filters.
1. (_optional_) `Username` - infobase user name.
1. (_optional_) `Password` - infobase user password.
1. (_optional_) `ObjectFilters.Topic` - topic name (by default is generated like `<infobaseName>_<dataType>`).
1. (_optional_) `ObjectFilters.ReadLastOnly` - produce only the last read transaction when multiple with equal object ids are found (by default `false`).
1. (_optional_) `ObjectFilters.TransactionTypeFilters` - transaction types filters (overwrites `TransactionTypeFilters` value).
1. (_optional_) `ErtRelativePath` - path of the ERT to be called to retrieve __object JSONs__ relative to `InfobasePath` (be default _"ExtForms\EDO\Test\GetObjectJson.ert"_).
1. (_optional_) `SuspendSchedule` - periods when producer have to suspend.


#### 1C8

__Producer__ requests infobase for new transactions that match `ObjectFilters` and `TransactionTypeFilters`, the repsonse also includes __object JSONs__.

Then it sends these __object JSONs__ to Kafka topic which name is specified at `ObjectFilters.Topic` or is generated like `<infobaseName>_<dataType>` where:

* `<infobaseName>` is directory name of the infobase
* `<dataType>` is the value of the property in each __object JSON__ which name is specified in `DataTypePropertyName`

Example:

```json
{
  "InfobaseUrl": "http://example.net/publicationName",
  "Username": "InfobaseUser",
  "Password": "qwerty",
  "DataTypePropertyName": "T",
  "ObjectFilters": [
    {
      "DataType": "Document.TestDocument",
      "JsonDepth": 3,
      "Topic": "Topic1"
    }
  ],
  "TransactionTypeFilters": [
    "RefNew"
  ],
  "SuspendSchedule": [
    {
      "Start": "23:00",
      "Duration": "6:00"
    }
  ]
}
```

Properties:

1. `InfobaseUrl` - 1C8 infobase HTTP-service endpoint for getting data.
1. `DataTypePropertyName` - data type property name at JSONs from `InfobaseUrl` response.
1. `ObjectFilters` - object filters.
1. `TransactionTypeFilters` - transaction types filters.
1. (_optional_) `Username` - infobase user name.
1. (_optional_) `Password` - infobase user password.
1. (_optional_) `ObjectFilters.Topic` - topic name (by default is generated like `<infobaseName>_<dataType>`).
1. (_optional_) `SuspendSchedule` - periods when producer have to suspend. 


### Kafka Consumers

#### 1C7

__Consumer__ retrieves message with __object JSON__.

Then it runs `JsonTransform` instruction corresponding to the topic.

Then __consumer__ saves results to infobase by calling ERT specified in `ErtRelativePath`.

Example:

```json
{
  "InfobasePath": "D:\\path\\to\\infobase",
  "Username": "InfobaseUser",
  "Password": "qwerty",
  "ErtRelativePath": "path\\to\\ert",
  "ConsumerGroup": "<consumerGroup>",
  "TopicsInstructions": {
    "Topic1": "Instruction1.json"
  },
  "SuspendSchedule": [
    {
      "Start": "23:00",
      "Duration": "6:00"
    }
  ]
}
```

Properties:

1. `InfobasePath` - absolute path to 1C7 infobase.
1. `Username` - infobase user name.
1. `Password` - infobase user password.
1. `TopicsInstructions` - _topic names_ -> names of _instructions_ stored in `Properties/ConsumerInstructions` directory.
1. (_optional_) `ErtRelativePath` - path of the ERT to be called to save `JsonTransform` results to infobase relative to `InfobasePath`  (be default _"ExtForms\EDO\Test\SaveObjectFromJson.ert"_).
1. (_optional_) `ConsumerGroup` - consumer group (__infobase directory name__ by default).
1. (_optional_) `SuspendSchedule` - periods when consumer have to suspend. 


#### Http

__Consumer__ retrieves message with __object JSON__.

Then it runs `JsonTransform` instruction corresponding to the topic.

Then __consumer__ sends results to the resource specified in `Url`.

Example:

```json
{
  "Url": "http://example.net/publicationName",
  "Username": "User",
  "Password": "qwerty",
  "ConsumerGroup": "<consumerGroup>",
  "TopicsInstructions": {
    "Topic1": "Instruction1.json"
  },
  "SuspendSchedule": [
    {
      "Start": "23:00",
      "Duration": "6:00"
    }
  ]
}
```

Properties:

1. `Url` - Endpoint for saving data.
1. `Username` - infobase user name.
1. `Password` - infobase user password.
1. `TopicsInstructions` - _topic names_ -> names of _instructions_ stored in `Properties/ConsumerInstructions` directory.
1. (_optional_) `ConsumerGroup` - consumer group (__infobase publication name__ by default).
1. (_optional_) `SuspendSchedule` - periods when consumer have to suspend. 


#### MS SQL

__Consumer__ retrieves message with __object JSON__.

Then it runs `JsonTransform` instruction corresponding to the topic.

Then __consumer__ inserts results to database table which are retrieved from each `JsonTransform` result __object JSON__ from the property with name specified in `TablePropertyName`.

Example:

```json
{
  "ConnectionString": "Provider=MSOLEDBSQL;Server=<serverAddress>;Database=<dbName>;UID=<username>;PWD=<password>;",
  "ConnectionType": "OleDbConnection",
  "TablePropertyName": "table",
  "ConsumerGroup": "<consumerGroup>",
  "TopicsInstructions": {
    "Topic1": "Instruction1.json"
  },
  "SuspendSchedule": [
    {
      "Start": "23:00",
      "Duration": "6:00"
    }
  ]
}
```

Properties:

1. `ConnectionString` - connection string.
1. `TablePropertyName` - name of the property which stores table name.
1. `TopicsInstructions` - _topic names_ -> names of _instructions_ stored in `Properties/ConsumerInstructions` directory.
1. (_optional_) `ConnectionType` - _"OleDbConnection"_ or _"SqlConnection"_.
1. (_optional_) `ConsumerGroup` - consumer group (__database name__ by default).
1. (_optional_) `SuspendSchedule` - periods when consumer have to suspend. 
