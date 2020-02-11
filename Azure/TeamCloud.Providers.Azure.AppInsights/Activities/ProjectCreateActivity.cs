﻿/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using TeamCloud.Azure.Deployment;
using TeamCloud.Model.Commands;
using TeamCloud.Providers.Azure.AppInsights.Templates;

namespace TeamCloud.Providers.Azure.AppInsights.Activities
{
    public class ProjectCreateActivity
    {
        private readonly IAzureDeploymentService azureDeploymentService;

        public ProjectCreateActivity(IAzureDeploymentService azureDeploymentService)
        {
            this.azureDeploymentService = azureDeploymentService ?? throw new ArgumentNullException(nameof(azureDeploymentService));
        }

        [FunctionName(nameof(ProjectCreateActivity))]
        public async Task<Dictionary<string, string>> RunActivity(
            [ActivityTrigger] ProviderProjectCreateCommand command,
            ILogger log)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            var template = new ProjectCreateTemplate();

            var deployment = await azureDeploymentService
                .DeployTemplateAsync(template, command.Payload.ResourceGroup.SubscriptionId, command.Payload.ResourceGroup.ResourceGroupName)
                .ConfigureAwait(false);

            var deploymentOutput = await deployment
                .WaitAndGetOutputAsync(throwOnError: true, cleanUp: true)
                .ConfigureAwait(false);

            return deploymentOutput
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString());
        }
    }
}