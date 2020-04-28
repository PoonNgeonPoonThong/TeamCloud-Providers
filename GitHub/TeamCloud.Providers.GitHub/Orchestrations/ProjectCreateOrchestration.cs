﻿// /**
//  *  Copyright (c) Microsoft Corporation.
//  *  Licensed under the MIT License.
//  */

// using System;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.Azure.WebJobs;
// using Microsoft.Azure.WebJobs.Extensions.DurableTask;
// using Microsoft.Extensions.Logging;
// using TeamCloud.Model;
// using TeamCloud.Model.Commands;
// using TeamCloud.Model.Data;
// using TeamCloud.Orchestration;
// using TeamCloud.Providers.GitHub.Activities;
// using TeamCloud.Providers.Core;
// using TeamCloud.Serialization;

// namespace TeamCloud.Providers.GitHub.Orchestrations
// {
//     public static class ProjectCreateOrchestration
//     {
//         [FunctionName(nameof(ProjectCreateOrchestration))]
//         public static async Task RunOrchestration(
//             [OrchestrationTrigger] IDurableOrchestrationContext functionContext,
//             ILogger log)
//         {
//             if (functionContext is null)
//                 throw new ArgumentNullException(nameof(functionContext));

//             if (log is null)
//                 throw new ArgumentNullException(nameof(log));

//             var command = functionContext.GetInput<ProviderProjectCreateCommand>();
//             var commandResult = command.CreateResult();

//             using (log.BeginCommandScope(command))
//             {
//                 try
//                 {
//                     var deploymentOutput = await functionContext
//                         .GetDeploymentOutputAsync(nameof(ProjectCreateActivity), command.Payload)
//                         .ConfigureAwait(true);

//                     commandResult.Result = new ProviderOutput
//                     {
//                         Properties = deploymentOutput.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString())
//                     };
//                 }
//                 catch (Exception exc)
//                 {
//                     commandResult ??= command.CreateResult();
//                     commandResult.Errors.Add(exc);

//                     throw exc.AsSerializable();
//                 }
//                 finally
//                 {
//                     var commandException = commandResult.GetException();

//                     if (commandException is null)
//                         functionContext.SetCustomStatus($"Command succeeded", log);
//                     else
//                         functionContext.SetCustomStatus($"Command failed: {commandException.Message}", log, commandException);

//                     functionContext.SetOutput(commandResult);
//                 }
//             }
//         }
//     }
// }