using BinanceWatcherFunction.Interfaces;
using BinanceWatcherFunction.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceWatcherFunction.Services
{


    public class DataHandlerService : IDataHandlerService
    {
        private IMongoDbService _mongoDbService;
        private IQueueService _queueService;

        public DataHandlerService(IMongoDbService mongoDbServcie, IQueueService queueService)
        {
            _mongoDbService = mongoDbServcie;
            _queueService = queueService;
        }

        public async Task HandleExistingData(ProjectData incomingData, ProjectData existingData, FilterDefinition<ProjectData> filter, FilterDefinition<CryptoList> cryptoFilter)
        {

            var stakingCollection = _mongoDbService.GetStakingCollection();
            var cryptoCollection = _mongoDbService.GetCryptoCollection();
            var message = await BuildMessageForExistingData(cryptoCollection, incomingData, existingData, cryptoFilter);
            await _queueService.SendToQueue(message);

            var newProjects = incomingData.Projects.Where(ip => !existingData.Projects.Select(ep => ep.Id).Contains(ip.Id));
            await HandleNewProjects(incomingData, newProjects);

            await _mongoDbService.ReplaceDataAsync(filter, incomingData);
        }

        public async Task HandleNewData(ProjectData incomingData)
        {
            await _mongoDbService.InsertDataAsync(incomingData);
            await HandleNewProjects(incomingData, incomingData.Projects);
        }

        private async Task HandleNewProjects(ProjectData incomingData, IEnumerable<Project> newProjects)
        {
            foreach (var project in newProjects)
            {
                string message = BuildMessageForNewProject(incomingData, project);
                await _queueService.SendToQueue(message);
            }
        }

        private async Task<string> BuildMessageForExistingData(IMongoCollection<CryptoList> cryptoCollection, ProjectData incomingData, ProjectData existingData, FilterDefinition<CryptoList> cryptoFilter)
        {
            var message = "";
            foreach (var project in existingData.Projects)
            {
                var currentproject = incomingData.Projects.Find(x => x.Id == project.Id);
                if (currentproject != null && !currentproject.SellOut && project.SellOut && (await cryptoCollection.Find(cryptoFilter).AnyAsync()))
                {
                    message += currentproject.Duration + " Days at " + string.Format("{0:P2}.", currentproject.Config.AnnualInterestRate) + "\n";
                }
            }
            return message;
        }

        private static string BuildMessageForNewProject(ProjectData incomingData, Project project)
        {
            return incomingData.Asset + " NEW STAKING OPTION!!!!!:  " + project.Duration + " Days at " + String.Format("{0:P2}.", project.Config.AnnualInterestRate);
        }
    }


}
