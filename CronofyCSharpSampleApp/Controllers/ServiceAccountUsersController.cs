using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cronofy;
using CronofyCSharpSampleApp.Models;
using CronofyCSharpSampleApp.Persistence.Models;

namespace CronofyCSharpSampleApp.Controllers
{
    public class ServiceAccountUsersController : EnterpriseConnectControllerBase
    {
        public ActionResult New()
        {
            return View("New", new EnterpriseConnectUser
            {
                Scopes = "read_account list_calendars read_events create_event delete_event read_free_busy"
            });
        }

        public ActionResult Create(EnterpriseConnectUser user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CronofyHelper.AuthorizeWithServiceAccount(uidCookie.Value, user.Email, user.Scopes);
                }
                catch (CronofyResponseException ex)
                {
                    user.SetError(ex);
                }

                if (user.NoErrors())
                {
                    var recordCount = Convert.ToInt32(DatabaseHandler.Scalar("SELECT COUNT(1) FROM EnterpriseConnectUserData WHERE Email=@email AND OwnedBy=@ownedBy",
                                                                             new Dictionary<string, object> { { "email", user.Email }, { "ownedBy", uidCookie.Value } }));

                    if (recordCount == 0)
                    {
                        DatabaseHandler.ExecuteNonQuery("INSERT INTO EnterpriseConnectUserData(Email, OwnedBy, Status) VALUES(@email, @ownedBy, @status)",
                                                        new Dictionary<string, object> { { "email", user.Email }, { "ownedBy", uidCookie.Value }, { "status", (int)EnterpriseConnectUserData.ConnectedStatus.Pending } });
                    }
                    else {
                        DatabaseHandler.ExecuteNonQuery("UPDATE EnterpriseConnectUserData SET Status=@status WHERE Email=@email AND OwnedBy=@ownedBy",
                                                        new Dictionary<string, object> { { "status", (int)EnterpriseConnectUserData.ConnectedStatus.Pending }, { "email", user.Email }, { "ownedBy", uidCookie.Value } });
                    }

                    return new RedirectResult("/enterpriseconnect");
                }
            }

            return View("New", user);
        }

        public ActionResult Show([Bind(Prefix = "id")] string userId)
        {
            var enterpriseConnectData = DatabaseHandler.Get<EnterpriseConnectUserData>("SELECT CronofyUID, Email, Status FROM EnterpriseConnectUserData WHERE CronofyUID=@userId AND OwnedBy=@uidCookie",
                                                                                       new Dictionary<string, object> { { "userId", userId }, { "uidCookie", uidCookie.Value } });

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
                try
                {
                    CronofyHelper.UpsertEvent(newEvent.EventId, newEvent.CalendarId, newEvent.Summary, newEvent.Description, newEvent.Start, newEvent.End);
                }
                catch (CronofyResponseException ex)
                {
                    newEvent.SetError(ex);
                }

                if (newEvent.NoErrors())
                {
                    return new RedirectResult(String.Format("/serviceaccountusers/show/{0}/calendar/{1}", newEvent.UserId, newEvent.CalendarId));
                }
            }

            ViewData["calendarName"] = CronofyHelper.GetCalendars().First(x => x.CalendarId == newEvent.CalendarId).Name;

            return View("NewEvent", newEvent);
        }

        private void ImpersonateUser(string userId)
        {
            var user = DatabaseHandler.Get<User>("SELECT CronofyUID, AccessToken, RefreshToken from UserCredentials WHERE CronofyUID=@userId AND ServiceAccount=0",
                                                 new Dictionary<string, object> { { "userId", userId } });

            CronofyHelper.SetToken(user.AccessToken, user.RefreshToken, false);
        }
    }
}
