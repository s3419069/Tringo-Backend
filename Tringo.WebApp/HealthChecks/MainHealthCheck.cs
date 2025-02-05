﻿using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tringo.FlightsService;

namespace Tringo.WebApp.HealthChecks
{
    public class MainHealthCheck : IHealthCheck
    {
        private readonly IFlightsService _flightsService;
        private readonly IAirportsService _airportsService;

        public MainHealthCheck(IFlightsService flightsService,
            IAirportsService airportsService)
        {
            _flightsService = flightsService;
            _airportsService = airportsService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var healthCheckResultHealthy =
                _airportsService.GetPriceGuaranteeAirports().Any()
                && (await _flightsService.GetFlights(
                    new FlightsService.DTO.WJFlightsRequest
                    {
                        DepartureAirportCode = "MEL",
                        DestinationAirportCodes = new List<string> { "SYD" },
                        TravelClass = "Economy"
                    })).Any();

            if (healthCheckResultHealthy)
            {
                return await Task.FromResult(
                    HealthCheckResult.Healthy("The check indicates a healthy result."));
            }

            return await Task.FromResult(
                HealthCheckResult.Unhealthy("The check indicates an unhealthy result."));
        }
    }
}
