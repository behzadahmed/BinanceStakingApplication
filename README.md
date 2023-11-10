#Binance Staking Watcher

#Overview
The Binance Staking Watcher is a microservices application that monitors cryptocurrency staking options on binance and sends a message to a queue, which then forwards a message to a chat Service. 
The purpose is to know when a staking option is availble as soon as possible. The application consists of several services:

## BinanceWatcherFunction
An Azure Function that fetches data from a binance API with a timed trigger, processes it, updates a MongoDB database accordingly and adds a message to a queue. It includes several components:

- **DataHandlerService**: Handles the processing of incoming data and sends messages to an Azure Service Bus queue.
- **ServiceBusQueueService**: Sends messages to an Azure Service Bus queue.
- **MongoDbService**: Interacts with a MongoDB database to fetch and update data.

## TelegramBotFunction
An Azure Function that is triggered whenever a message is added to the Service Bus queue. It sends the message via a Telegram bot to a Telegram group.

## StakingConfig.Api
A separate service that provides an API for adding and removing items from the crypto list. 

Need to add authorisation and login for the Web and api
## React Web Frontend
A React application that allows users to view the crypto list and add or remove cryptos via the StakingConfig.Api.

# Getting Started

## Prerequisites
- .NET 6.0 or later
- Node.js and npm
- An Azure account
- A MongoDB database
- A Telegram bot

