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
        private static string _oauthAccountIdCallbackUrl = $"{ConfigurationManager.AppSettings["domain"]}/availability/AccountId";

		private static string _enterpriseConnectOAuthCallbackUrl = $"{ConfigurationManager.AppSettings["domain"]}/oauth/enterpriseconnect";
        private static string _enterpriseConnectUserAuthCallbackUrl = $"{ConfigurationManager.AppSettings["domain"]}/serviceaccountcallback/authorize/";

		public static bool LoadUser(string cronofyUid, bool serviceAccount)
		{
            var user = DatabaseHandler.Get<Persistence.Models.User>($"SELECT CronofyUid, AccessToken, RefreshToken FROM UserCredentials WHERE CronofyUID='{cronofyUid}' AND ServiceAccount='{(serviceAccount ? 1 : 0)}'");

            if (user == null)
            {
                LogHelper.Log($"LoadUser failed - Unable to find user - CronofyUID=`{cronofyUid}` - ServiceAccount=`{serviceAccount}`");

                return false;
            }

            LogHelper.Log($"LoadUser success - CronofyUID=`{cronofyUid}` - ServiceAccount=`{serviceAccount}`");

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

        public static string GetAccountIdAuthUrl()
        {
            return OAuthClient.GetAuthorizationUrlBuilder(_oauthAccountIdCallbackUrl).ToString();
        }

		public static string GetEnterpriseConnectAuthUrl()
		{
			return OAuthClient.GetAuthorizationUrlBuilder(_enterpriseConnectOAuthCallbackUrl)
							  .EnterpriseConnect()
							  .ToString();
		}

		public static OAuthToken GetOAuthToken(string code)
		{
            OAuthToken token = null;

            try
            {
                token = OAuthClient.GetTokenFromCode(code, _oauthCallbackUrl);
                LogHelper.Log($"GetOAuthToken success - code=`{code}`");
            }
            catch (CronofyException)
            {
                LogHelper.Log($"GetOAuthToken failure - code=`{code}`");
            }

            return token;
		}

        public static OAuthToken GetAccountIdOAuthToken(string code)
        {
            OAuthToken token = null;

            try
            {
                token = OAuthClient.GetTokenFromCode(code, _oauthAccountIdCallbackUrl);
                LogHelper.Log($"GetAccountIdOAuthToken success - code=`{code}`");
            }
            catch (CronofyException)
            {
                LogHelper.Log($"GetAccountIdOAuthToken failure - code=`{code}`");
            }

            return token;
        }

        public static OAuthToken GetEnterpriseConnectUserOAuthToken(string enterpriseConnectId, string email, string code)
        {
            OAuthToken token = null;

            try
            {
                token = OAuthClient.GetTokenFromCode(code, _enterpriseConnectUserAuthCallbackUrl + enterpriseConnectId + "?email=" + email);
                LogHelper.Log($"GetEnterpriseConnectUserOAuthToken success - enterpriseConnectId=`{enterpriseConnectId}` - code=`{code}`");
            }
            catch (CronofyException)
            {
                LogHelper.Log($"GetEnterpriseConnectUserOAuthToken failure - enterpriseConnectId=`{enterpriseConnectId}` - code=`{code}`");
            }

            return token;
        }

        public static OAuthToken GetEnterpriseConnectOAuthToken(string code)
        {
            OAuthToken token = null;

            try
            {
                token = OAuthClient.GetTokenFromCode(code, _enterpriseConnectOAuthCallbackUrl);
                LogHelper.Log($"GetEnterpriseConnectOAuthToken success - code=`{code}`");
            }
            catch (CronofyException)
            {
                LogHelper.Log($"GetEnterpriseConnectOAuthToken failure - code=`{code}`");
            }

            return token;
        }

		public static Account GetAccount()
		{
            Account account = null;

            try
            {
                account = CronofyAccountRequest<Account>(() => { return AccountClient.GetAccount(); });
                LogHelper.Log($"GetAccount success");
            }
            catch (CronofyException)
            {
                LogHelper.Log($"GetAccount failure");
            }

            return account;
		}

		public static IEnumerable<Cronofy.Profile> GetProfiles()
		{
            IEnumerable<Cronofy.Profile> profiles = new Cronofy.Profile[0];

            try
            {
                profiles = CronofyAccountRequest<IEnumerable<Cronofy.Profile>>(() => { return AccountClient.GetProfiles(); });
                LogHelper.Log($"GetProfiles success");
            }
            catch (CronofyException)
            {
                LogHelper.Log($"GetProfiles failure");
            }

            return profiles;
		}

		public static IEnumerable<Cronofy.Calendar> GetCalendars()
		{
            IEnumerable<Cronofy.Calendar> calendars = new Cronofy.Calendar[0];

            try
            {
                calendars = CronofyAccountRequest<IEnumerable<Cronofy.Calendar>>(() => { return AccountClient.GetCalendars(); });
                LogHelper.Log($"GetCalendars success");
            }
            catch (CronofyException)
            {
                LogHelper.Log($"GetCalendars failure");
            }

            return calendars;
		}

		public static Cronofy.Calendar CreateCalendar(string profileId, string name)
		{
            Cronofy.Calendar calendar = null;

            try
            {
                calendar = CronofyAccountRequest<Cronofy.Calendar>(() => { return AccountClient.CreateCalendar(profileId, name); });
                LogHelper.Log($"CreateCalendar success - profileId=`{profileId}` - name=`{name}`");
            }
            catch (CronofyException)
            {
                LogHelper.Log($"CreateCalendar failure - profileId=`{profileId}` - name=`{name}`");
            }

            return calendar;
		}

		public static IEnumerable<Cronofy.FreeBusy> GetFreeBusy()
		{
            IEnumerable<Cronofy.FreeBusy> freeBusy = new Cronofy.FreeBusy[0];

            try
            {
                freeBusy = CronofyAccountRequest<IEnumerable<Cronofy.FreeBusy>>(() => { return AccountClient.GetFreeBusy(); });
                LogHelper.Log($"GetFreeBusy success");
            }
            catch (CronofyException)
            {
                LogHelper.Log($"GetFreeBusy failure");
            }

            return freeBusy;
		}

		public static IEnumerable<Cronofy.Event> ReadEventsForCalendar(string calendarId)
		{
            IEnumerable<Cronofy.Event> events = new Cronofy.Event[0];

			var readEvents = new GetEventsRequestBuilder()
				.IncludeManaged(true)
				.CalendarId(calendarId)
				.Build();

            try
            {
                events = CronofyAccountRequest<IEnumerable<Cronofy.Event>>(() => { return AccountClient.GetEvents(readEvents); });
                LogHelper.Log($"ReadEventsForCalendar success - calendarId=`{calendarId}`");
            }
            catch (CronofyException)
            {
                LogHelper.Log($"ReadEventsForCalendar failure - calendarId=`{calendarId}`");
            }

            return events;
		}

		public static IEnumerable<Cronofy.Event> ReadEvents()
		{
            IEnumerable<Cronofy.Event> events = new Cronofy.Event[0];

			var readEvents = new GetEventsRequestBuilder()
				.IncludeManaged(true)
				.Build();

            try
            {
                events = CronofyAccountRequest<IEnumerable<Cronofy.Event>>(() => { return AccountClient.GetEvents(readEvents); });
                LogHelper.Log($"ReadEvents success");
            }
            catch (CronofyException)
            {
                LogHelper.Log($"ReadEvents failure");
            }

            return events;
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

            try
            {
                CronofyAccountRequest(() => { AccountClient.UpsertEvent(calendarId, builtEvent); });
                LogHelper.Log($"UpsertEvent success - eventId=`{eventId}` - calendarId=`{calendarId}` - summary=`{summary}` - description=`{description}` - start=`{start}` - end=`{end}`");
            }
            catch (CronofyException)
            {
                LogHelper.Log($"UpsertEvent failure - eventId=`{eventId}` - calendarId=`{calendarId}` - summary=`{summary}` - description=`{description}` - start=`{start}` - end=`{end}`");
            }
		}

		public static void DeleteEvent(string calendarId, string eventId)
		{
            try
            {
                CronofyAccountRequest(() => { AccountClient.DeleteEvent(calendarId, eventId); });
                LogHelper.Log($"DeleteEvent success - calendarId=`{calendarId}` - eventId=`{eventId}`");
            }
            catch (CronofyException)
            {
                LogHelper.Log($"DeleteEvent failure - calendarId=`{calendarId}` - eventId=`{eventId}`");
            }
		}

		public static IEnumerable<Cronofy.Channel> GetChannels()
		{
            IEnumerable<Cronofy.Channel> channels = new Cronofy.Channel[0];

            try
            {
                channels = CronofyAccountRequest<IEnumerable<Cronofy.Channel>>(() => { return AccountClient.GetChannels(); });
                LogHelper.Log("GetChannels success");
            }
            catch (CronofyException)
            {
                LogHelper.Log("GetChannels failure");
            }

            return channels;
		}

		public static Cronofy.Channel CreateChannel(string path, bool onlyManaged, IEnumerable<string> calendarIds)
		{
            Cronofy.Channel channel = null;

			var builtChannel = new CreateChannelBuilder()
				.CallbackUrl(path)
				.OnlyManaged(onlyManaged)
				.CalendarIds(calendarIds)
				.Build();

            try
            {
                channel = CronofyAccountRequest<Cronofy.Channel>(() => { return AccountClient.CreateChannel(builtChannel); });
                LogHelper.Log($"CreateChannel success - path=`{path}` - onlyManaged=`{onlyManaged}` - calendarIds=`{String.Join(",", calendarIds)}`");
            }
            catch (CronofyException)
            {
                LogHelper.Log($"CreateChannel failure - path=`{path}` - onlyManaged=`{onlyManaged}` - calendarIds=`{String.Join(",", calendarIds)}`");
            }

            return channel;
		}

        public static Cronofy.UserInfo GetEnterpriseConnectUserInfo()
        {
            Cronofy.UserInfo userInfo = null;

            try
            {
                userInfo = CronofyAccountRequest<Cronofy.UserInfo>(() => { return EnterpriseConnectAccountClient.GetUserInfo(); });
                LogHelper.Log("GetEnterpriseConnectUserInfo success");
            }
            catch (CronofyException)
            {
                LogHelper.Log("GetEnterpriseConnectUserInfo failure");
            }

            return userInfo;
        }

        public static IEnumerable<Cronofy.Resource> GetResources()
        {
            IEnumerable<Cronofy.Resource> resources = new Cronofy.Resource[0];

            try
            {
                resources = CronofyEnterpriseConnectAccountRequest<IEnumerable<Cronofy.Resource>>(() => { return EnterpriseConnectAccountClient.GetResources(); });
                LogHelper.Log("GetResources success");
            }
            catch (CronofyException)
            {
                LogHelper.Log("GetResources failure");
            }

            return resources;
        }

        public static void AuthorizeWithServiceAccount(string enterpriseConnectId, string email, string scopes)
        {
            try
            {
                CronofyEnterpriseConnectAccountRequest(() => { EnterpriseConnectAccountClient.AuthorizeUser(email, _enterpriseConnectUserAuthCallbackUrl + enterpriseConnectId + "?email=" + email, scopes); });
                LogHelper.Log($"AuthorizeWithServiceAccount success - enterpriseConnectId=`{enterpriseConnectId}` - email=`{email}` - scopes=`{scopes}`");
            }
            catch (CronofyException)
            {
                LogHelper.Log($"AuthorizeWithServiceAccount failure - enterpriseConnectId=`{enterpriseConnectId}` - email=`{email}` - scopes=`{scopes}`");
            }
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
                // This may fail due to an out of day access token
				response = request();
			}
			catch (CronofyException)
			{
                try
                {
                    // First time this fails, attempt to get a new access token and store it for the user
                    var token = OAuthClient.GetTokenFromRefreshToken(_refreshToken);

                    DatabaseHandler.ExecuteNonQuery($"UPDATE UserCredentials SET AccessToken='{token.AccessToken}', RefreshToken='{token.RefreshToken}' WHERE CronofyUID='{_cronofyUid}' AND ServiceAccount=0");
                    SetToken(token, false);

                    LogHelper.Log($"Access Token out of date, tokens have been refreshed - _cronofyUid=`{_cronofyUid}` - serviceAccount=0");
                }
                catch (CronofyException)
                {
                    // If getting a new oauth token fails then delete the user's credentials and throw an exception
                    DatabaseHandler.ExecuteNonQuery($"DELETE FROM UserCredentials WHERE CronofyUID='{_cronofyUid}' AND ServiceAccount=0");
                    LogHelper.Log($"Credentials invalid, deleting account - _cronofyUid=`{_cronofyUid}` - serviceAccount=1");

                    throw new CredentialsInvalidError();
                }

                // Second time we perform the request, if it fails we want it to propagate up to the method that
                // called CronofyAccountRequest to log the appropriate information
				response = request();
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
                // This may fail due to an out of day access token
                response = request();
            }
            catch (CronofyException)
            {
                try
                {
                    // First time this fails, attempt to get a new access token and store it for the user
                    var token = OAuthClient.GetTokenFromRefreshToken(_enterpriseConnectRefreshToken);

                    DatabaseHandler.ExecuteNonQuery($"UPDATE UserCredentials SET AccessToken='{token.AccessToken}', RefreshToken='{token.RefreshToken}' WHERE CronofyUID='{_enterpriseConnectCronofyUid}' AND ServiceAccount=1");
                    SetToken(token, true);

                    LogHelper.Log($"Access Token out of date, tokens have been refreshed - _enterpriseConnectCronofyUid=`{_enterpriseConnectCronofyUid}` - serviceAccount=1");
                }
                catch (CronofyException)
                {
                    // If getting a new oauth token fails then delete the user's credentials and throw an exception
                    DatabaseHandler.ExecuteNonQuery($"DELETE FROM UserCredentials WHERE CronofyUID='{_enterpriseConnectCronofyUid}' AND ServiceAccount=1");
                    LogHelper.Log($"Credentials invalid, deleting account - _enterpriseConnectCronofyUid=`{_enterpriseConnectCronofyUid}` - serviceAccount=1");

                    throw new CredentialsInvalidError();
                }

                // Second time we perform the request, if it fails we want it to propagate up to the method that
                // called CronofyEnterpriseConnectAccountRequest to log the appropriate information
                response = request();
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
