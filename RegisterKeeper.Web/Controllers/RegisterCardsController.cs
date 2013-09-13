﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using RegisterKeeper.Web.Models;

namespace RegisterKeeper.Web.Controllers
{
	public class RegisterCardsController : Controller
	{
		private readonly RegisterKeeperDb _db = new RegisterKeeperDb();

		//
		// GET: /RegisterCards/

		public ActionResult Index()
		{
			return View(_db.RegisterCards.ToList());
		}

		//
		// GET: /RegisterCards/Details/5

		public ActionResult Details(int id = 0)
		{
			RegisterCard registerCard = _db.RegisterCards.Find(id);
			if (registerCard == null)
			{
				return HttpNotFound();
			}
			return View(registerCard);
		}

		//
		// GET: /RegisterCards/Create

		public ActionResult Create(int individualCompetitionId)
		{
			var competition = _db.Competitions.Find(individualCompetitionId);
			var registerCard = new RegisterCard
				{
					IndividualCompetitionId = individualCompetitionId,
					Shoots = new List<Shoot>()
				};

			foreach (var shoot in competition.Distances.ToList().Select(distance => new Shoot { Distance = distance }))
			{
				foreach (var sightingShotNumber in Enumerable.Range(1, competition.NumberOfSightingShots))
				{
					shoot.Shots.Add(new Shot { ShotNumber = sightingShotNumber });
				}
				foreach (var scoringShotNumber in Enumerable.Range(1, competition.NumberOfScoringShots))
				{
					shoot.Shots.Add(new Shot { ShotNumber = scoringShotNumber });
				}
				registerCard.Shoots.Add(shoot);
			}
			
			AddCompetitionDetailsToViewBag(competition, ViewBag);
			return View();
		}

		//
		// POST: /RegisterCards/Create

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(RegisterCard registerCard)
		{
			if (ModelState.IsValid)
			{
				_db.RegisterCards.Add(registerCard);
				_db.SaveChanges();
				return RedirectToAction("Details", "IndividualCompetitions", new { id = registerCard.IndividualCompetitionId });
			}

			var competition = _db.Competitions.Find(registerCard.IndividualCompetitionId);
			AddCompetitionDetailsToViewBag(competition, ViewBag);

			return View();
		}

		//
		// GET: /RegisterCards/Edit/5

		public ActionResult Edit(int id = 0)
		{
			RegisterCard registerCard = _db.RegisterCards.Find(id);
			if (registerCard == null)
			{
				return HttpNotFound();
			}

			var competition = _db.Competitions.Find(registerCard.IndividualCompetitionId);
			AddCompetitionDetailsToViewBag(competition, ViewBag);

			return View(registerCard);
		}

		//
		// POST: /RegisterCards/Edit/5

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(RegisterCard registerCard)
		{
			if (ModelState.IsValid)
			{
				// Add any new child entities (it seems adding more than one new child object at a time causes a duplicate key exception..)
				foreach (var shoot in registerCard.Shoots)
				{
					foreach (var sightingShot in shoot.Sighters)
					{
						_db.Entry(sightingShot).State = sightingShot.Id == default(int) ? EntityState.Added : EntityState.Modified;
						_db.SaveChanges();
					}
					foreach (var scoringShot in shoot.ScoringShots)
					{
						_db.Entry(scoringShot).State = scoringShot.Id == default(int) ? EntityState.Added : EntityState.Modified;
						_db.SaveChanges();
					}
				}

				// Update entity
				_db.Entry(registerCard).State = EntityState.Modified;

				foreach (var shoot in registerCard.Shoots)
				{
					_db.Entry(shoot).State = shoot.Id == default(int) ? EntityState.Added : EntityState.Modified;
				}

				_db.SaveChanges();
				return RedirectToAction("Details", "IndividualCompetitions", new { id = registerCard.IndividualCompetitionId });
			}
			return View(registerCard);
		}

		//
		// GET: /RegisterCards/Delete/5

		public ActionResult Delete(int id = 0)
		{
			RegisterCard registerCard = _db.RegisterCards.Find(id);
			if (registerCard == null)
			{
				return HttpNotFound();
			}
			return View(registerCard);
		}

		//
		// POST: /RegisterCards/Delete/5

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			var registerCard = _db.RegisterCards.Find(id);
			_db.RegisterCards.Remove(registerCard);
			_db.SaveChanges();
			return RedirectToAction("Index");
		}

		internal static void AddCompetitionDetailsToViewBag(Competition competition, dynamic viewBag)
		{
			viewBag.CompetitionId = competition.Id;
			viewBag.CompetitionName = competition.Name;
			viewBag.Distances = competition.Distances.ToList();
			viewBag.NumberOfSightingShots = competition.NumberOfSightingShots;
			viewBag.NumberOfScoringShots = competition.NumberOfScoringShots;
		}

		protected override void Dispose(bool disposing)
		{
			_db.Dispose();
			base.Dispose(disposing);
		}
	}
}