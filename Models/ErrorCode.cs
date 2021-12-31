using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiCore.Models
{
	public class ErrorCode : IErrorCode
	{
		public string Code { get; private set; }
		public string EnglishTranslation { get; private set; }
		public int? HTTPStatusCode { get; private set; }

		public ErrorCode(string name, string section, string englishTranslation, int? httpStatusCode)
		{
			Code = NameToCode(name, section);
			EnglishTranslation = englishTranslation;
			HTTPStatusCode = httpStatusCode;
		}

		private static string NameToCode(string name, string section)
		{
			string code = "err_";
			code += section.ToLower();
			code += "_";

			string[] nameSegments = name.Split('_');
			bool firstSegment = true;
			foreach (string segment in nameSegments)
			{
				if (firstSegment)
				{
					firstSegment = false;
					code += segment.ToLower();
				}
				else
				{
					code += string.Concat(segment[0].ToString().ToUpper(), segment.ToLower().AsSpan(1));
				}
			}

			return code;
		}

		public readonly static string SUPPORT = "Please contact the support team.";
	}
}
