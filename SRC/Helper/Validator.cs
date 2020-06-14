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
	
		public static bool IsNumber(this string str) {
			if (String.IsNullOrWhiteSpace(str))
				return false;
			return str.All(ch => ch >= '0' && ch <= '9');
		}

		public static string RemoveNonDigits(this string s) {
			if (s == null) {
				return null;
			} else {
				return Regex.Replace(s, @"\D", String.Empty);
			}
		}
        public static bool IsNullOrEmptyOrBlank(this string s)
        {
            if (s == null || s.Trim().Count() == 0)
            {
                return true;
            }
            return false;
        }

        public static string AccentRemove(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static void SendImportProgress(int count, int total, string msg, IHubContext<NotifyHub, ITypedHubClient> hubContext) {
            if (hubContext != null) {
                var percentage = (double)count / total * 100;
                if ((double)((int)percentage) == (double)percentage) {
                    hubContext.Clients.All.UpdatePercent($"{msg} {percentage}%");
                }
            }
        }
    }
}
