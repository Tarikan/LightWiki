using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace LightWiki.Aws.Cognito.Triggers.PostConfirmation;

public class Function
{
    public async Task<JsonElement> FunctionHandler(JsonElement input, ILambdaContext context)
    {
        if (input.GetProperty("triggerSource").GetString() == "PostConfirmation_ConfirmForgotPassword")
        {
            LambdaLogger.Log("User forgot password confirmation, return");
            return input;
        }

        var configuration = new ConfigurationBuilder()
            .AddSettingsSources()
            .Build();

        var connectionString = configuration.GetSection("ConnectionStrings")["DbConnectionString"];
        var userPoolId = configuration.GetSection("Aws")["UserPoolId"];

        var request = input.GetProperty("request");
        var userAttributes = request.GetProperty("userAttributes");
        var email = userAttributes.GetProperty("email").GetString() ?? throw new ArgumentException("email not provided");
        var username = input.GetProperty("userName").GetString() ?? throw new ArgumentException("username not provided");
        LambdaLogger.Log(email);
        
        var optionsBuilder = new DbContextOptionsBuilder<WikiContext>();
        optionsBuilder.UseNpgsql(connectionString);

        var dbContext = new WikiContext(optionsBuilder.Options);

        var userExists = await dbContext.Users.AnyAsync(u => u.Email == email);

        if (userExists)
        {
            LambdaLogger.Log("User already exists");
            throw new UserWithSuchEmailAlreadyExists();
        }

        var party = new Party
        {
            PartyType = PartyType.User,
        };

        var user = new User
        {
            Email = email,
            Name = username,
            Party = party,
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        await dbContext.DisposeAsync();
        
        using var cognitoClient = new AmazonCognitoIdentityProviderClient();

        var userAttrList = new List<AttributeType>
        {
            new AttributeType
            {
                Name = "custom:public_id",
                Value = user.Id.ToString(),
            },
        };
        
        var updateAttrRequest = new AdminUpdateUserAttributesRequest
        {
            UserAttributes = userAttrList,
            Username = username,
            UserPoolId = userPoolId,
        };

        await cognitoClient.AdminUpdateUserAttributesAsync(updateAttrRequest);
        
        return input;
    }
}