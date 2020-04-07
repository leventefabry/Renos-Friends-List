﻿using System;
using System.Collections.Generic;

namespace RenosFriendsList.API.Services.PropertyMapping
{
    public class PropertyMappingValue
    {
        public PropertyMappingValue(IEnumerable<string> destinationProperties, bool revert = false)
        {
            DestinationProperties = destinationProperties ?? throw new ArgumentNullException(nameof(destinationProperties));
            Revert = revert;
        }

        public IEnumerable<string> DestinationProperties { get; private set; }
        public bool Revert { get; private set; }
    }
}
