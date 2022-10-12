﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Services.WeatherService;

public interface IWeatherService
{
    public Task<WeatherResponse> GetWeatherAsync();
}