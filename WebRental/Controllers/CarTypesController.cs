using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using WebRental.Models;

namespace CarRental.Controllers
{
    [RoutePrefix("api/CarTypes")]
    public class CarTypesController : ApiController
    {
        private CarRentalEntities db = new CarRentalEntities();

        // GET: api/CarTypes
        public IQueryable<spGetCarTypes_Result> GetCarTypes()
        {
            return db.spGetCarTypes().AsQueryable();
        }

        // GET: api/CarTypes/5
        [ResponseType(typeof(CarType))]
        public IHttpActionResult GetCarType(int id)
        {
            var carType = GetCarTypes().Where(c => c.ID == id).FirstOrDefault();
            if (carType == null)
            {
                return NotFound();
            }

            return Ok(carType);
        }


        // PUT: api/CarTypes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCarType(int id, CarType carType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != carType.ID)
            {
                return BadRequest();
            }

            db.Entry(carType).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/CarTypes
        [ResponseType(typeof(CarType))]
        public IHttpActionResult PostCarType(CarType carType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CarTypes.Add(carType);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = carType.ID }, carType);
        }

        // DELETE: api/CarTypes/5
        [ResponseType(typeof(CarType))]
        public IHttpActionResult DeleteCarType(int id)
        {
            CarType carType = db.CarTypes.Find(id);
            if (carType == null)
            {
                return NotFound();
            }

            db.CarTypes.Remove(carType);
            db.SaveChanges();

            return Ok(carType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CarTypeExists(int id)
        {
            return db.CarTypes.Count(e => e.ID == id) > 0;
        }
    }
}