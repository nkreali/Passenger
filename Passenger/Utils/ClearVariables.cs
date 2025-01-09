﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passenger.Utils
{
    public class ClearVariables
    {
        public static void VariablesClear()
        {
            PassengerLib.Globals.serviceName = "";
            PassengerLib.Globals.accountName = "";
            PassengerLib.Globals.newAccountPassword = "";
            PassengerLib.Globals.closeAppConfirmation = false;
            PassengerLib.Globals.deleteConfirmation = false;
        }
    }
}
