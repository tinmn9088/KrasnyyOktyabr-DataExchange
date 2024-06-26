﻿# Application

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
        "1C8": []
      }
    }
  }
}
```

Properties:

1. `Socket` - Kafka broker address.
1. (_optional_) `MessageMaxBytes` - the largest record batch size allowed by Kafka.
1. (_optional_) `MaxPollIntervalMs` - the maximum delay between poll requests.
1. (_optional_) `MessageTimeoutMs` - limits the time a produced message waits for successful delivery.



### Kafka Producers

#### 1C7

__Producer__ reads infobase log file (__10 000__ KB from the end (it is also the limit) when offset is not specified in _offsets.json_) looking for new transactions that match `ObjectFilters` and `TransactionTypeFilters`.

When transactions are found __producer__ retrieves corresponding __objects JSONs__ from infobase by calling ERT specified  in `ErtRelativePath`.

Then __producer__ adds corresponding transaction data to each of the retrieved __object JSONs__.

Then it sends these __object JSONs__ to Kafka topic which name is `<infobaseName>_<dataType>` where:

* `<infobaseName>` is directory name of the infobase
* `<dataType>` is the value of the property in each __object JSON__ which name is specified in `DataTypePropertyName`

Example:

```json
{
  "InfobasePath": "D:\\path\\to\\infobase",
  "Username": "InfobaseUser",
  "Password": "qwerty",
  "ErtRelativePath": "\\path\\to\\ert",
  "DataTypePropertyName": "T",
  "ObjectFilters": [
    "O/9999/:3"
  ],
  "TransactionTypeFilters": [
    "RefNew"
  ],
  "DocumentGuidsDatabaseConnectionString": "",
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
1. `ObjectFilters` - each entry is `<idPrefix>:<jsonMaxDepth>`.
1. `TransactionTypeFilters` - transaction types filters.
1. (_optional_) `ErtRelativePath` - path of the ERT to be called to retrieve __object JSONs__ relative to `InfobasePath` (be default _"ExtForms\EDO\Test\UN_JSON_Synch.ert"_).
1. (_optional_) `DocumentGuidsDatabaseConnectionString` - connection string to the database where global document IDs are stored.
1. (_optional_) `SuspendSchedule` - periods when producer have to suspend.


#### 1C8

__Producer__ requests infobase for new transactions that match `ObjectFilters` and `TransactionTypeFilters`, the repsonse also includes __object JSONs__.

Then it sends these __object JSONs__ to Kafka topic which name is `<infobaseName>_<dataType>` where:

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
    "O/9999/:3"
  ],
  "TransactionTypeFilters": [
    "RefNew"
  ],
  "DocumentGuidsDatabaseConnectionString": "",
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
1. `Username` - infobase user name.
1. `Password` - infobase user password.
1. `ObjectFilters` - each entry is `<idPrefix>:<jsonMaxDepth>`.
1. `TransactionTypeFilters` - transaction types filters.
1. (_optional_) `DocumentGuidsDatabaseConnectionString` - connection string to the database where global document IDs are stored.
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
  "ErtRelativePath": "\\path\\to\\ert",
  "ConsumerGroup": "<consumerGroup>",
  "Topics": [
    "Topic1"
  ],
  "Instructions": {
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
1. `Topics` - topic names.
1. `Instructions` - _topic names_ -> names of _instructions_ stored in `Properties/ConsumerInstructions` directory.
1. (_optional_) `ErtRelativePath` - path of the ERT to be called to save `JsonTransform` results to infobase relative to `InfobasePath`  (be default _"ExtForms\EDO\Test\SaveObject.ert"_).
1. (_optional_) `ConsumerGroup` - consumer group (__infobase directory name__ by default).
1. (_optional_) `SuspendSchedule` - periods when consumer have to suspend. 


#### 1C8

__Consumer__ retrieves message with __object JSON__.

Then it runs `JsonTransform` instruction corresponding to the topic.

Then __consumer__ saves results to infobase by sending them to specified `InfobaseUrl`.

Example:

```json
{
  "InfobaseUrl": "http://example.net/publicationName",
  "Username": "InfobaseUser",
  "Password": "qwerty",
  "ConsumerGroup": "<consumerGroup>",
  "Topics": [
    "Topic1"
  ],
  "Instructions": {
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

1. `InfobaseUrl` - 1C8 infobase HTTP-service endpoint for saving data.
1. `Username` - infobase user name.
1. `Password` - infobase user password.
1. `Topics` - topic names.
1. `Instructions` - _topic names_ -> names of _instructions_ stored in `Properties/ConsumerInstructions` directory.
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
  "Topics": [
    "Topic1"
  ],
  "ConsumerGroup": "<consumerGroup>",
  "Instructions": {
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
1. `Topics` - topic names.
1. `Instructions` - _topic names_ -> names of _instructions_ stored in `Properties/ConsumerInstructions` directory.
1. (_optional_) `ConnectionType` - _"OleDbConnection"_ or _"SqlConnection"_.
1. (_optional_) `ConsumerGroup` - consumer group (__database name__ by default).
1. (_optional_) `SuspendSchedule` - periods when consumer have to suspend. 
