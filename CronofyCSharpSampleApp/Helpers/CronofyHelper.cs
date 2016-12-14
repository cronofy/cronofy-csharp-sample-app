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
		public const string EnterpriseConnectCookieName = "EnterpriseConnectUID";

		private static string _cronofyUid;
		private static string _accessToken;
		private static string _refreshToken;

        private static string _enterpriseConnectCronofyUid;
        private static string _enterpriseConnectAccessToken;
        private static string _enterpriseConnectRefreshToken;

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

        private static CronofyEnterpriseConnectAccountClient _enterpriseConnectAccountClient;
        private static CronofyEnterpriseConnectAccountClient EnterpriseConnectAccountClient
        {
            get
            {
                if (_enterpriseConnectAccountClient == null)
                    _enterpriseConnectAccountClient = new CronofyEnterpriseConnectAccountClient(_enterpriseConnectAccessToken);
                return _enterpriseConnectAccountClient;
            }
        }

		private static string _oauthCallbackUrl = $"{ConfigurationManager.AppSettings["domain"]}/oauth";
		private static string _enterpriseConnectOAuthCallbackUrl = $"{ConfigurationManager.AppSettings["domain"]}/oauth/enterpriseconnect";
        private static string _enterpriseConnectUserAuthCallbackUrl = $"{ConfigurationManager.AppSettings["domain"]}/serviceaccountcallback/authorize/";

		public static bool LoadUser(string cronofyUid, bool serviceAccount)
		{
            var user = DatabaseHandler.Get<Persistence.Models.User>($"SELECT CronofyUid, AccessToken, RefreshToken FROM Users WHERE CronofyUID='{cronofyUid}' AND ServiceAccount='{(serviceAccount ? 1 : 0)}'");

			if (user == null)
				return false;


            if (serviceAccount)
            {
                _enterpriseConnectCronofyUid = cronofyUid;
                _enterpriseConnectAccessToken = user.AccessToken;
                _enterpriseConnectRefreshToken = user.RefreshToken;
            }
            else {
                _cronofyUid = cronofyUid;
                _accessToken = user.AccessToken;
                _refreshToken = user.RefreshToken;
            }

			return true;
		}

        public static void SetToken(OAuthToken token, bool serviceAccount)
        {
            SetToken(token.AccessToken, token.RefreshToken, serviceAccount);
        }

        public static void SetToken(string accessToken, string refreshToken, bool serviceAccount)
        {
            if (serviceAccount)
            {
                _enterpriseConnectAccessToken = accessToken;
                _enterpriseConnectRefreshToken = refreshToken;

                _enterpriseConnectAccountClient = new CronofyEnterpriseConnectAccountClient(_enterpriseConnectAccessToken);
            }
            else {
                _accessToken = accessToken;
                _refreshToken = refreshToken;

                _accountClient = new CronofyAccountClient(_accessToken);
            }
        }

		public static string GetAuthUrl()
		{
			return OAuthClient.GetAuthorizationUrlBuilder(_oauthCallbackUrl).ToString();
		}

		public static string GetEnterpriseConnectAuthUrl()
		{
			return OAuthClient.GetAuthorizationUrlBuilder(_enterpriseConnectOAuthCallbackUrl)
							  .EnterpriseConnect()
							  .ToString();
		}

		public static OAuthToken GetOAuthToken(string code)
		{
			return OAuthClient.GetTokenFromCode(code, _oauthCallbackUrl);
		}

        public static OAuthToken GetEnterpriseConnectUserOAuthToken(string enterpriseConnectId, string code)
        {
            return OAuthClient.GetTokenFromCode(code, _enterpriseConnectUserAuthCallbackUrl + enterpriseConnectId);
        }

		public static OAuthToken GetEnterpriseConnectOAuthToken(string code)
		{
			return OAuthClient.GetTokenFromCode(code, _enterpriseConnectOAuthCallbackUrl);
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

		public static IEnumerable<Cronofy.FreeBusy> GetFreeBusy()
		{
			return CronofyAccountRequest<IEnumerable<Cronofy.FreeBusy>>(() => { return AccountClient.GetFreeBusy(); });
		}

		public static IEnumerable<Cronofy.Event> ReadEventsForCalendar(string calendarId)
		{
			var readEvents = new GetEventsRequestBuilder()
				.IncludeManaged(true)
				.CalendarId(calendarId)
				.Build();

			return CronofyAccountRequest<IEnumerable<Cronofy.Event>>(() => { return AccountClient.GetEvents(readEvents); });
		}

		public static IEnumerable<Cronofy.Event> ReadEvents()
		{
			var readEvents = new GetEventsRequestBuilder()
				.IncludeManaged(true)
				.Build();

			return CronofyAccountRequest<IEnumerable<Cronofy.Event>>(() => { return AccountClient.GetEvents(readEvents); });
		}

		public static void UpsertEvent(string eventId, string calendarId, string summary, string description, DateTime start, DateTime end)
		{
			var builtEvent = new UpsertEventRequestBuilder()
				.EventId(eventId)
				.Summary(summary)
				.Description(description)
				.Start(start)
				.End(end)
				.Build();

			CronofyAccountRequest(() => { 
				AccountClient.UpsertEvent(calendarId, builtEvent); 
			});
		}

		public static void DeleteEvent(string calendarId, string eventId)
		{
			CronofyAccountRequest(() => { AccountClient.DeleteEvent(calendarId, eventId); });
		}

		public static IEnumerable<Cronofy.Channel> GetChannels()
		{
			return CronofyAccountRequest<IEnumerable<Cronofy.Channel>>(() => { return AccountClient.GetChannels(); });
		}

		public static Cronofy.Channel CreateChannel(string path, bool onlyManaged, IEnumerable<string> calendarIds)
		{
			var builtChannel = new CreateChannelBuilder()
				.CallbackUrl(path)
				.OnlyManaged(onlyManaged)
				.CalendarIds(calendarIds)
				.Build();

			return CronofyAccountRequest<Cronofy.Channel>(() => { return AccountClient.CreateChannel(builtChannel); });
		}

        public static Cronofy.UserInfo GetEnterpriseConnectUserInfo()
        {
            return CronofyAccountRequest<Cronofy.UserInfo>(() => { return EnterpriseConnectAccountClient.GetUserInfo(); });
        }

        public static IEnumerable<Cronofy.Resource> GetResources()
        {
            return CronofyEnterpriseConnectAccountRequest<IEnumerable<Cronofy.Resource>>(() => { return EnterpriseConnectAccountClient.GetResources(); });
        }

        public static void AuthorizeWithServiceAccount(string enterpriseConnectId, string email, string scopes)
        {
            CronofyEnterpriseConnectAccountRequest(() => { EnterpriseConnectAccountClient.AuthorizeUser(email, _enterpriseConnectUserAuthCallbackUrl + enterpriseConnectId, scopes); });
        }

		static void CronofyAccountRequest(Action request)
		{
			CronofyAccountRequest<bool>(() => { request(); return true; });
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

				DatabaseHandler.ExecuteNonQuery($"UPDATE Users SET AccessToken='{token.AccessToken}', RefreshToken='{token.RefreshToken}' WHERE CronofyUID='{_cronofyUid}' AND ServiceAccount=0");
				SetToken(token, false);

				try
				{
					response = request();
				}
				catch (CronofyException)
				{
					DatabaseHandler.ExecuteNonQuery($"UPDATE Users SET AccessToken='', RefreshToken='' WHERE CronofyUID='{_cronofyUid}' AND ServiceAccount=0");

					throw new CredentialsInvalidError();
				}
			}

			return response;
		}

        static void CronofyEnterpriseConnectAccountRequest(Action request)
        {
            CronofyEnterpriseConnectAccountRequest<bool>(() => { request(); return true; });
        }

        static T CronofyEnterpriseConnectAccountRequest<T>(Func<T> request)
        {
            T response = default(T);

            try
            {
                response = request();
            }
            catch (CronofyException)
            {
                var token = OAuthClient.GetTokenFromRefreshToken(_enterpriseConnectRefreshToken);

                DatabaseHandler.ExecuteNonQuery($"UPDATE Users SET AccessToken='{token.AccessToken}', RefreshToken='{token.RefreshToken}' WHERE CronofyUID='{_enterpriseConnectCronofyUid}' AND ServiceAccount=1");
                SetToken(token, true);

                try
                {
                    response = request();
                }
                catch (CronofyException)
                {
                    DatabaseHandler.ExecuteNonQuery($"UPDATE Users SET AccessToken='', RefreshToken='' WHERE CronofyUID='{_enterpriseConnectCronofyUid}' AND ServiceAccount=1");

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
