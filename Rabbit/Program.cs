using Rabbit.Managers;
using Rabbit.Settings;
using Rabbit.Models;
using Rabbit.Models.Collections;
using Microsoft.Extensions.Configuration;

// 1. appsettings.json


var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

IConfiguration config = builder.Build();

var mongoDbSettings = config.GetSection("MongoDBSettings").Get<MongoDBSettings>();
var rabbitMqSettings = config.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();

// 2. MongoDbManager

var mongoDbManager = new MongoDBManager(mongoDbSettings);

// 3. RabbitMqManager

RabbitMQManager rabbitMqManager = new RabbitMQManager(rabbitMqSettings, mongoDbManager);