﻿using OnlineShopApp.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopApp.Data.Entities
{
    public class SettingEntity : BaseEntity
    {
        public bool MaintenenceMode { get; set; } 
    }
}
