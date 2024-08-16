﻿global using static EventPAM.BuildingBlocks.EventPAMBase;
global using MediatR;
global using Duende.IdentityServer.EntityFramework.Entities;
global using EventPAM.BuildingBlocks.Core.CQRS;
global using EventPAM.BuildingBlocks.Web;
global using EventPAM.Event.Data;
global using EventPAM.Event.Events.Dtos;
global using EventPAM.Event.Events.Exceptions;
global using FluentValidation;
global using Mapster;
global using MapsterMapper;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Routing;
global using Ardalis.GuardClauses;