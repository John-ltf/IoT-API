# IoT-api

This repo is the back-end server of the IoT Home On Cloud project. It is consisted of a C# application that implements the below APIs:
## APIs
| HTTP method | endpoint                                | Description                                                   |
| ------      | -----                                   | ----------                                                    |
| GET         | `/devices/list/`                        | Retrieve the list (only names) of the user's devices          |
| GET         | `/devices/`                             | Retrieve the user's devices including any related information |
| GET         | `/device/{deviceId}`                    | Retrieve a device's information                               |
| GET         | `/device/{deviceId}`                    | Retrieve a device's information                               |
| POST        | `/device/{deviceId}`                    | Register a new device's                                       |
| PUT         | `/device/{deviceId}`                    | Update a device's information                                 |
| DELETE      | `/device/{deviceId}`                    | Delete a device                                               |
| PUT         | `/device/{deviceId}/status/{statusVal}` | Enable/disable a device                                       |
| POST        | `/direct-method/{deviceId}/`            | Send message to device throught Azure IoT hub                 |

All APIs interact with the Azure IoT hub either to retrieve devices or manage them
The user must have been logged in via the front-end to use the APIs.
