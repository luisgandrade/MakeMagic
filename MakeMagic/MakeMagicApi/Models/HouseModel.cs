﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakeMagic.MakeMagicApi.Models
{
    public class HouseModel
    {

        [JsonProperty("_id")]
        public string Id { get; set; }

        public string Name { get; set; }

    }
}
