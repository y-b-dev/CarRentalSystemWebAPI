using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using WebRental.Models;

namespace CarRental.Controllers
{
    [RoutePrefix("api/RentDetails")]
    public class RentDetailsController : ApiController
    {
        private CarRentalEntities db = new CarRentalEntities();

        // GET: api/RentDetails
        public IQueryable<spGetRents_Result> GetRentDetails()
        {
            return db.spGetRents().AsQueryable();
        }

        [Route("By-UserID/{id}")]
        public IHttpActionResult GetRentsByUserID(int id)
        {
            var rents = db.spGetRentsByUserID(id).ToArray();
            if (rents.Count() == 0)
            {
                return NotFound();
            }
            return Ok(rents);
        }

        [Route("For-Manager")]
        public IQueryable GetRentsFullDetails()
        {
            db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
            return GetRentDetails().GroupJoin(db.spGetCars(), rent => rent.CarID, car => car.ID,
                (rent, car) => new { rent, car }).SelectMany(z => z.car.DefaultIfEmpty(),
                (r, c) => new 
                    {
                        r.rent.ID,
                        r.rent.StartDate,
                        r.rent.EndDate,
                        r.rent.ActualEndDate,
                        r.rent.UserID,
                        Image = c == null ? null : c.Image
                    })
            .GroupJoin(db.spGetUsers(), r => r.UserID, u => u.ID, (r, u) => new { r, u })
            .SelectMany(s => s.u.DefaultIfEmpty(), (r, u) =>
            new
            {
                r.r.ID,
                r.r.StartDate,
                r.r.EndDate,
                r.r.ActualEndDate,
                r.r.Image,
                UserName = u == null ? null : u.UserName,
                LastName = u == null ? null : u.LastName,
                FirstName = u == null ? null : u.FirstName
            });
        }

        // GET: api/RentDetails/5
        [ResponseType(typeof(RentDetail))]
        public IHttpActionResult GetRentDetail(int id)
        {
            var rentDetail = GetRentDetails().Where(r => r.ID == id).FirstOrDefault();
            if (rentDetail == null)
            {
                return NotFound();
            }

            return Ok(rentDetail);
        }

        // PUT: api/RentDetails/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRentDetail(int id, RentDetail rentDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rentDetail.ID)
            {
                return BadRequest();
            }

            db.Entry(rentDetail).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RentDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.Created);
        }

        // POST: api/RentDetails
        [HttpPost]
        [ResponseType(typeof(RentDetail))]
        public IHttpActionResult PostRentDetail(List<RentDetail> rents)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var rent in rents)
                    {
                        db.Cars.Find(rent.CarID).IsAvailable = false;
                        db.RentDetails.Add(rent);
                        db.SaveChanges();
                    }
                    trans.Commit();
                    return StatusCode(HttpStatusCode.NoContent);
                }
                catch(System.Exception e)
                {
                    trans.Rollback();
                    return InternalServerError(e);
                }
                
            }
        }

        // DELETE: api/RentDetails/5
        [ResponseType(typeof(RentDetail))]
        public IHttpActionResult DeleteRentDetail(int id)
        {
            RentDetail rentDetail = db.RentDetails.Find(id);
            if (rentDetail == null)
            {
                return NotFound();
            }

            db.RentDetails.Remove(rentDetail);
            db.SaveChanges();

            return Ok(rentDetail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RentDetailExists(int id)
        {
            return db.RentDetails.Count(e => e.ID == id) > 0;
        }
    }
}