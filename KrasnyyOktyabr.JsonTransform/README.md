# JsonTransform

## Instructions list

### Table of contents 

| Group                       | Instructions                                                                                     |
| :---                        | :---                                                                                             |
| [Context](#context)         | [$add](#add), [$mget](#mget), [$mset](#mset), [$select](#select)                                 |
| [Arithmetic](#arithmetic)   | [$sum](#sum), [$substract](#substract), [$mul](#mul), [$div](#div), [$round](#round)             |
| [Logical](#logical)         | [$and](#and), [$eq](#eq), [$neq](#neq), [$gt](#gt), [$gte](#gte), [$not](#not), [$or](#or)       |
| [Containers](#containers)   | [Array](#array), [ExpressionsBlock](#expressionsblock), [$map](#map)                             |
| [Conditional](#conditional) | [$if](#if)                                                                                       |
| [Loops](#loops)             | [$cur](#cur), [$curindex](#curindex), [$foreach](#foreach), [$while](#while)                     |
| [Other](#other)             | [$cast](#cast), [$resolve](#resolve), [$regexgetgroup](#regexgetgroup), [$strformat](#strformat) |



### Context

#### $add

##### Description

Adds a new property to output.

> Throws an error when trying to add a property with a key that already exists.

##### Parameters

| Name                 | Type          | Description                                      |
|:---                  |:---           |:---                                              |
| `key`                | `string` exp. | Name of the property.                            |
| `value`              | `any` exp.    | Value of the property.                           |
| (_optional_) `index` | `int` exp.    | Index of the output property (__0__ by default). |

##### Example

```json
{
  "$add": {
    "key": "newProperty",
    "value": "newPropertyValue",
    "index": 1
  }
}
```

Output:

```json
[
  {},
  {
    "newProperty": "newPropertyValue"
  }
]
```


#### $mget

##### Description

Gets value from _memory_ by name.

> Throws an error if the name is not present in _memory_.

##### Parameters

| Name    | Type          | 
|:---     |:---           |
| `name`  | `string` exp. |

##### Returns

`any`.

##### Example

```json
{
  "$mget": {
    "name": "variableName"
  }
}
```


#### $mset

##### Description

Saves value to _memory_ by name.

##### Parameters

| Name    | Type          | 
|:---     |:---           |
| `name`  | `string` exp. |
| `value` | `any` exp.    |

##### Example

```json
{
  "$mset": {
    "name": "three",
    "value": 3
  }
}
```


#### $select

##### Description

Returns result of JSONPath query.

> Throws an error if query returns nothing and `optional` is _false_.

##### Parameters

| Name                     | Type          | 
|:---                      |:---           |
| `path`                   | `string` exp. |
| (_optional_) `optional`  | `bool` exp.   |

##### Returns

`any`.

##### Example

If input is:

```json
{
  "key": {
    "nestedKey": "value"
  }
}
```

```json
{
  "$select": {
    "path": "key.nestedKey"
  }
}
```

Returns _"value"_.



### Arithmetic

#### $sum

##### Description

Sums operands.

##### Parameters

| Name    | Type          | 
|:---     |:---           |
| `left`  | `number` exp. |
| `right` | `number` exp. |

##### Returns

`number`.

##### Example

```json
{
  "$sum": {
    "left": 4,
    "right": 3
  }
}
```

Returns _7_.


#### $substract

##### Description

Substracts operands.

##### Parameters

| Name    | Type          | 
|:---     |:---           |
| `left`  | `number` exp. |
| `right` | `number` exp. |

##### Returns

`number`.

##### Example

```json
{
  "$substract": {
    "left": 4,
    "right": 3
  }
}
```

Returns _1_.


#### $mul

##### Description

Multiplies operands.

##### Parameters

| Name    | Type          | 
|:---     |:---           |
| `left`  | `number` exp. |
| `right` | `number` exp. |

##### Returns

`number`.

##### Example

```json
{
  "$mul": {
    "left": 4,
    "right": 3
  }
}
```

Returns _12_.


#### $div

##### Description

Divides operands.

##### Parameters

| Name    | Type          | 
|:---     |:---           |
| `left`  | `number` exp. |
| `right` | `number` exp. |

##### Returns

`number`.

##### Example

```json
{
  "$div": {
    "left": 4,
    "right": 2
  }
}
```

Returns _2_.


#### $round

##### Parameters

| Name                 | Type          | 
|:---                  |:---           |
| `value`              | `number` exp. |
| (_optional_) `digits` | `int` exp.    |

##### Returns

`number`.

##### Example

```json
{
  "$round": {
    "value": 4.6666,
    "digits": 2
  }
}
```

Returns _4.67_.



### Logical

#### $and

##### Parameters

| Name    | Type        | 
|:---     |:---         |
| `left`  | `bool` exp. |
| `right` | `bool` exp. |

##### Returns

`bool`.

##### Example

```json
{
  "$and": {
    "left": true,
    "right": true
  }
}
```

Returns _true_.


#### $eq

##### Description

Checks if operands are _equal_.

##### Parameters

| Name    | Type       | 
|:---     |:---        |
| `left`  | `any` exp. |
| `right` | `any` exp. |

##### Returns

`bool`.

##### Example

```json
{
  "$eq": {
    "left": 66,
    "right": 66
  }
}
```

Returns _true_.


#### $neq

##### Description

Checks if operands are _not equal_.

##### Parameters

| Name    | Type       | 
|:---     |:---        |
| `left`  | `any` exp. |
| `right` | `any` exp. |

##### Returns

`bool`.

##### Example

```json
{
  "$neq": {
    "left": 66,
    "right": 66
  }
}
```

Returns _false_.


#### $gt

##### Description

Checks if `left` operand is _greater_ than the `right` one.

##### Parameters

| Name    | Type          | 
|:---     |:---           |
| `left`  | `number` exp. |
| `right` | `number` exp. |

##### Returns

`bool`.

##### Example

```json
{
  "$gt": {
    "left": 99,
    "right": 66
  }
}
```

Returns _true_.


#### $gte

##### Description

Checks if `left` operand is _greater or equal_ than the `right` one.

##### Parameters

| Name    | Type          | 
|:---     |:---           |
| `left`  | `number` exp. |
| `right` | `number` exp. |

##### Returns

`bool`.

##### Example

```json
{
  "$gt": {
    "left": 65.9,
    "right": 66
  }
}
```

Returns _false_.


#### $not

##### Description

Inverts value of the nested `bool` expression.

##### Parameters

| Name    | Type        | 
|:---     |:---         |
| `value` | `bool` exp. |

##### Returns

`bool`.

##### Example

```json
{
  "$not": {
    "value": true
  }
}
```

Returns _false_.


#### $or

##### Parameters

| Name    | Type        | 
|:---     |:---         |
| `left`  | `bool` exp. |
| `right` | `bool` exp. |

##### Returns

`bool`.

##### Example

```json
{
  "$or": {
    "left": true,
    "right": false
  }
}
```

Returns _true_.



### Containers

#### Array

##### Description

Runs nested expressions and assembles results in an array.

##### Returns

`array`.

##### Example

```json
[
 1,
 "two",
 {
   "$sum": {
     "left": 2,
     "right": 1
   }
 }
]
```

Returns _[1, "two", 3]_.


#### ExpressionsBlock

##### Description

Runs nested expressions.

##### Example

```json
[
  {
    "$add": {
      "key": "property1",
      "value": "value1"
    }
  },
  {
    "$add": {
      "key": "property2",
      "value": "value2"
    }
  }
]
```

Output:

```json
[
  {
    "property1": "value1"
    "property2": "value2"
  }
]
```


#### $map

##### Description

Runs nested expressions and assembles them into an associative array.

##### Returns

`map<string, object?>`.

##### Example

```json
{
  "$map": {
    "result1": "value1",
    "result2": {
      "$sum": {
        "left": 3,
        "right": 2
      }
    }
  }
}
```

Returns _{"result1":"value1","result2":5}_.



### Conditional

#### $if

##### Parameters

| Name                | Type        | 
|:---                 |:---         |
| `condition`         | `bool` exp. |
| `then`              | exp.        |
| (_optional_) `else` | exp.        |

##### Example

```json
{
  "$if": {
    "condition": true,
    "then": [],
    "else": []
  }
}
```



### Loops

#### $cur

##### Description

Gets current cursor value or by name.

> Throws an error if cursor is not present.

##### Parameters

| Name                 | Type          | 
|:---                  |:---           |
| (_optional_) `name`  | `string` exp. |

##### Returns

`any`.

##### Example

```json
{
  "$cur": {}
}
```

or

```json
{
  "$cur": {
    "name": "CursorName"
  }
}
```



#### $curindex

##### Description

Gets current cursor index or by name.

> Throws an error if cursor is not present.

##### Parameters

| Name                 | Type          | 
|:---                  |:---           |
| (_optional_) `name`  | `string` exp. |

##### Returns

`int`.

##### Example

```json
{
  "$curindex": {}
}
```

or

```json
{
  "$curindex": {
    "name": "CursorName"
  }
}
```


#### $foreach

##### Parameters

| Name                | Type         | 
|:---                 |:---          |
| `items`             | `array` exp. |
| `instructions`      | exp.         |
| (_optional_) `name` | `string`     |

##### Example

```json
{
  "$foreach": {
    "name": "f",
    "items": [],
    "instructions": []
  }
}
```


#### $while

##### Parameters

| Name           | Type        | 
|:---            |:---         |
| `condition`    | `bool` exp. |
| `instructions` | exp.        |

##### Example

```json
{
  "$while": {
    "condition": false,
    "instructions": []
  }
}
```



### Other

#### $cast

##### Description

Runs nested expression and converts its result to the specified type.

##### Parameters

| Name    | Type                                                    | 
|:---     |:---                                                     |
| `value` | `any` exp.                                              |
| `type`  | _"int"_, _"float"_, _"bool"_, _"string"_ or _"array"_.  |

##### Returns

When `type` is:

* _"int"_ -> `int`
* _"float"_ -> `float`
* _"string"_ -> `string`
* _"bool"_ -> `bool`
* _"array"_ -> `array`

##### Example

```json
{
  "$cast": {
    "value": "99.9",
    "type": "float"
  }
}
```

Returns _99.9_.


#### $resolve

##### Parameters

| Name       | Type                        | 
|:---        |:---                         |
| `resolver` | `string` exp.               |
| `params`   | `map<string, object?>` exp. |

##### Notes

Available resolvers (i.e. `resolver` parameter values) with parameters for now:

* _"HttpDataResolver"_
  * `url` - `string`
  * `method` - `string` (_"GET"_, _"POST"_ etc.)
  * (_optional_) `contentType` - `string`
  * (_optional_) `username` - `string`
  * (_optional_) `password` - `string`
  * (_optional_) `body` - `string`

* _"MsSqlSingleValueDataResolver"_
  * `connectionString` - `string`
  * `connectionType` - `string` (_"OleDbConnection"_ or _"SqlConnection"_)
  * `query` - `string` (_SELECT_ SQL-query)

* _"ComV77ApplicationResolver"_
  * `infobasePath` - `string`
  * `username` - `string`
  * `password` - `string`
  * `ertName` - `string` (name of the file located in `infobasePath` + `/EXTFORMS/EDO/Test`)
  * (_optional_) `formParams` - `map<string, object?>`
  * (_optional_) `resultName` - `string`

> In _ComV77ApplicationResolver_ `formParams` values are converted to strings before passing to ERT.

##### Returns

`any`.

##### Example

```json
{
  "$resolve": {
    "resolver": "HttpDataResolver",
    "params": {
      "$map": {
        "url": "http://example.net/api",
        "method": "GET"
      }
    }
  }
}
```

```json
{
  "$resolve": {
    "resolver": "MsSqlSingleValueDataResolver",
    "params": {
      "$map": {
        "connectionString": "TestConnectionString",
        "query": "SELECT TOP 1 ColumnName FROM TestTable"
      }
    }
  }
}
```

```json
{
  "$resolve": {
    "resolver": "ComV77ApplicationResolver",
    "params": {
      "$map": {
        "infobasePath": "D:\\path\\to\\infobase",
        "username": "InfobaseUser",
        "password": "Password",
        "ertName": "ErtToCall.ert",
        "formParams": {
          "$const": {
            "param1": "param1Value",
            "param2": 66
          }
        },
        "resultName": "ErtResultName"
      }
    }
  }
}
```


#### $regexgetgroup

##### Parameters

| Name          | Type          | Description                                                      |
|:---           |:---           | :--                                                              |
| `regex`       | `string` exp. |                                                                  |
| `input`       | `string` exp. |                                                                  |
| `groupNumber` | `int` exp.    | When __0__ the whole match value is returned (__1__ by default). |

##### Returns

`string` (empty when regex doesn't match).

##### Example

```json
{
  "$regexgetgroup": {
    "regex": "Hello (.*)?",
    "input": "Hello World"
  }
}
```

Returns _"World"_.


#### $strformat

##### Parameters

| Name    | Type          |
|:---     |:---           |
| `value` | `string` exp. |
| `args`  | `array` exp.  |

##### Returns

`string`.

##### Example

```json
{
  "$strformat": {
    "value": "Hello {0}",
    "args": ["World"]
  }
}
```

Returns _"Hello World"_.
