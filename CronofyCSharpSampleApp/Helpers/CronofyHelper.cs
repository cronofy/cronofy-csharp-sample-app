using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Cronofy;

namespace CronofyCSharpSampleApp
{
	public class CronofyHelper
	{
		public const string CookieName = "CronofyUID";

		private static string _cronofyUid;
		private static string _accessToken;
		private static string _refreshToken;

		private static CronofyOAuthClient _oauthClient;

		private static CronofyOAuthClient OAuthClient
		{
			get
			{
				if (_oauthClient == null)
					_oauthClient = new CronofyOAuthClient(ConfigurationManager.AppSettings["cronofy_client_id"], ConfigurationManager.AppSettings["cronofy_client_secret"]);
				return _oauthClient;
			}
		}

		private static CronofyAccountClient _accountClient;
		private static CronofyAccountClient AccountClient
		{
			get
			{
				if (_accountClient == null)
					_accountClient = new CronofyAccountClient(_accessToken);
				return _accountClient;
			}
		}

		private static string _oauthCallbackUrl = $"{ConfigurationManager.AppSettings["domain"]}/oauth";

		public static bool LoadUser(string cronofyUid)
		{
			var user = DatabaseHandler.Get<User>("SELECT * FROM Users WHERE CronofyUID='" + cronofyUid + "'");

			if (user == null)
				return false;

			_cronofyUid = cronofyUid;
			_accessToken = user.AccessToken;
			_refreshToken = user.RefreshToken;

			return true;
		}

		public static void SetToken(OAuthToken token)
		{
			_accessToken = token.AccessToken;
			_refreshToken = token.RefreshToken;

			_accountClient = new CronofyAccountClient(_accessToken);
		}

		public static string GetAuthUrl()
		{
			return OAuthClient.GetAuthorizationUrlBuilder(_oauthCallbackUrl).ToString();
		}

		public static OAuthToken GetOAuthToken(string code)
		{
			return OAuthClient.GetTokenFromCode(code, _oauthCallbackUrl);
		}

		public static Account GetAccount()
		{
			return CronofyAccountRequest<Account>(() => { return AccountClient.GetAccount(); });
		}

		public static IEnumerable<Cronofy.Profile> GetProfiles()
		{
			return CronofyAccountRequest<IEnumerable<Cronofy.Profile>>(() => { return AccountClient.GetProfiles(); });
		}

		public static IEnumerable<Cronofy.Calendar> GetCalendars()
		{
			return CronofyAccountRequest<IEnumerable<Cronofy.Calendar>>(() => { return AccountClient.GetCalendars(); });
		}

		public static Cronofy.Calendar CreateCalendar(string profileId, string name)
		{
			return CronofyAccountRequest<Cronofy.Calendar>(() => { return AccountClient.CreateCalendar(profileId, name); });
		}

		static T CronofyAccountRequest<T>(Func<T> request)
		{
			T response = default(T);

			try
			{
				response = request();
			}
			catch (CronofyException)
			{
				var token = OAuthClient.GetTokenFromRefreshToken(_refreshToken);

				DatabaseHandler.ExecuteNonQuery($"UPDATE Users SET AccessToken='{token.AccessToken}', RefreshToken='{token.RefreshToken}' WHERE CronofyUID='{_cronofyUid}'");
				SetToken(token);

				try
				{
					response = request();
				}
				catch (CronofyException)
				{
					DatabaseHandler.ExecuteNonQuery($"UPDATE Users SET AccessToken='', RefreshToken='' WHERE CronofyUID={_cronofyUid}");

					throw new CredentialsInvalidError();
				}
			}

			return response;
		}
	}

	public class CredentialsInvalidError : Exception
	{
		public CredentialsInvalidError()
		{
		}
	}
}
