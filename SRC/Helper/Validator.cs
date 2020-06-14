using Microsoft.AspNetCore.SignalR;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Nibo.Services;
using System.Text.RegularExpressions;
using Nibo.Services.Interfaces;

namespace Nibo.Helper {
	public static class Validator {

        /// <summary>
        /// Verify string is null, empty or blank
        /// </summary>
        /// <param name="s"></param>
        /// <returns>bool</returns>
        public static bool IsNullOrEmptyOrBlank(this string s)
        {
            if (s == null || s.Trim().Count() == 0)
            {
                return true;
            }
            return false;
        }
    }
}
