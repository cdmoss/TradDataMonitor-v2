﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JSONSerializeDemo
{
    class Class1
	{
		string jsonTest = @"{
	""Hubs"": [{
			""SerialNumber"": ""559095"",
			""HubName"": ""Test Hub 1"",
			""Sensors"": [{
					""SensorType"": ""Humidity"",
					""Port"": ""0""
				},
				{
					""SensorType"": ""Moisture"",
					""Port"": ""5""
				}
			]
		},
		{
			""SerialNumber"": ""559023"",
			""HubName"": ""Test Hub 2"",
			""Sensors"": [{
					""SensorType"": ""Humidity"",
					""Port"": ""1""
				},
				{
					""SensorType"": ""Moisture"",
					""Port"": ""2""
				}
			]
		}
	]
}";
    }
}
