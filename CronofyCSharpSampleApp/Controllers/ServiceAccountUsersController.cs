using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CronofyCSharpSampleApp.Models;
using CronofyCSharpSampleApp.Persistence.Models;
using Newtonsoft.Json;

namespace CronofyCSharpSampleApp.Controllers
{
    public class ServiceAccountUsersController : EnterpriseConnectControllerBase
    {
        public ActionResult New()
        {
            return View ("New", new EnterpriseConnectUser {
                Scopes = "read_account list_calendars read_events create_event delete_event read_free_busy"
            });
        }

        public ActionResult Create(EnterpriseConnectUser user)
        {
            if (ModelState.IsValid)
            {
                CronofyHelper.AuthorizeWithServiceAccount(uidCookie.Value, user.Email, user.Scopes);

                var recordCount = Convert.ToInt32(DatabaseHandler.Scalar($"SELECT * FROM EnterpriseConnectUserData WHERE Email='{user.Email}' AND OwnedBy='{uidCookie.Value}'"));

                if (recordCount == 0)
                {
                    DatabaseHandler.ExecuteNonQuery($"INSERT INTO EnterpriseConnectUserData(Email, OwnedBy, Status) VALUES('{user.Email}', '{uidCookie.Value}', '{(int)EnterpriseConnectUserData.ConnectedStatus.Pending}')");
                }
                else {
                    DatabaseHandler.ExecuteNonQuery($"UPDATE EnterpriseConnectUserData SET Status='{(int)EnterpriseConnectUserData.ConnectedStatus.Pending}' WHERE Email='{user.Email}' AND OwnedBy='{uidCookie.Value}'");
                }

                return new RedirectResult("/enterpriseconnect");
            }

            return View("New", user);
        }

        public ActionResult Show([Bind(Prefix = "id")] string userId)
        {
            var enterpriseConnectData = DatabaseHandler.Get<EnterpriseConnectUserData>($"SELECT CronofyUID, Email, Status FROM EnterpriseConnectUserData WHERE CronofyUID='{userId}' AND OwnedBy='{uidCookie.Value}'");

            ImpersonateUser(userId);

            var profiles = new Dictionary<Cronofy.Profile, Cronofy.Calendar[]>();
            var calendars = CronofyHelper.GetCalendars();

            foreach (var profile in CronofyHelper.GetProfiles())
            {
                profiles.Add(profile, calendars.Where(x => x.Profile.ProfileId == profile.Id).ToArray());
            }

            ViewData["profiles"] = profiles;

            return View("Show", enterpriseConnectData);
        }

        public ActionResult Calendar(string userId, string calendarId)
        {
            ImpersonateUser(userId);

            var calendar = CronofyHelper.GetCalendars().First(x => x.CalendarId == calendarId);

            ViewData["userId"] = userId;
            ViewData["events"] = CronofyHelper.ReadEventsForCalendar(calendarId);

            return View("Calendar", calendar);            
        }

        public ActionResult NewEvent(string userId, string calendarId)
        {
            ImpersonateUser(userId);

            ViewData["calendarName"] = CronofyHelper.GetCalendars().First(x => x.CalendarId == calendarId).Name;

            var newEvent = new Models.Event
            {
                UserId = userId,
                CalendarId = calendarId,
                EventId = "unique_event_id_" + (new Random().Next(0, 1000000).ToString("D6"))
            };

            return View("NewEvent", newEvent);
        }

        public ActionResult CreateEvent(Models.Event newEvent)
        {
            ImpersonateUser(newEvent.UserId);

            if (newEvent.Start > newEvent.End)
            {
                ModelState.AddModelError("End", "End time cannot be before start time");
            }

            if (ModelState.IsValid)
            {
                CronofyHelper.UpsertEvent(newEvent.EventId, newEvent.CalendarId, newEvent.Summary, newEvent.Description, newEvent.Start, newEvent.End);

                return new RedirectResult($"/serviceaccountusers/show/{newEvent.UserId}/calendar/{newEvent.CalendarId}");
            }

            ViewData["calendarName"] = CronofyHelper.GetCalendars().First(x => x.CalendarId == newEvent.CalendarId).Name;

            return View("NewEvent", newEvent);
        }

        private void ImpersonateUser(string userId)
        {
            var user = DatabaseHandler.Get<User>($"SELECT CronofyUID, AccessToken, RefreshToken from UserCredentials WHERE CronofyUID='{userId}' AND ServiceAccount=0");

            CronofyHelper.SetToken(user.AccessToken, user.RefreshToken, false);
        }
    }
}
