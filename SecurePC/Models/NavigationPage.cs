﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurePC.Models
{
    public class NavigationPage
    {
        public string Title
        {
            set;
            get;
        }

        public Type PageClass
        {
            get;
            set;
        }
    }
}
