using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using WebRental.Models;

namespace CarRental.Controllers
{
    [RoutePrefix("api/Cars")]
    public class CarsController : ApiController
    {
        private CarRentalEntities db = new CarRentalEntities();

        // GET: api/Cars
        public IQueryable<spGetCars_Result> GetCars()
        {
            return db.spGetCars().AsQueryable();
        }

        // GET: api/Cars/5
        [ResponseType(typeof(spGetCarByID_Result))]
        public IHttpActionResult GetCar([FromUri]int id)
        {
            var car = db.spGetCarByID(id).FirstOrDefault();
            if (car == null)
            {
                return NotFound();
            }

            return Ok(car);
        }

        [Route("With-Type/{id}")]
        [ResponseType(typeof(spGetCarWithTypeByID_Result))]
        public IHttpActionResult GetCarWithType([FromUri]int id)
        {
            var car = db.spGetCarWithTypeByID(id).FirstOrDefault();
            if (car == null)
            {
                return NotFound();
            }

            return Ok(car);
        }

        [Route("By-BranchID/{id}")]
        [ResponseType(typeof(spGetCarWithTypeByID_Result))]
        public IHttpActionResult GetCarsByBranch([FromUri]int id)
        {
            var cars = GetCars().Where(c => c.BranchID == id).ToArray();
            if (cars.Count() == 0)
            {
                return NotFound();
            }

            return Ok(cars);
        }


        [Route("FullDetails")]
        public IQueryable<FullCarDetails> GetFullDetails()
        {
            db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

            return db.Cars.Join(db.CarTypes, car => car.CarTypeID, ct => ct.ID,
                (car, ct) => new { car, ct })
           .Where(c => c.car.IsDriveable && c.car.IsAvailable).Select(s => new FullCarDetails {
               ID = s.car.ID,
               Year = s.ct.Year,
               Image = s.car.Image,
               KM = s.car.KM,
               BranchID = s.car.BranchID,
               LatePrice = s.ct.LatePrice,
               DailyPrice = s.ct.DailyPrice,
               Gear = s.ct.Gear,
               Manufactor = s.ct.Manufactor,
               Model = s.ct.Model,
               Dates = db.RentDetails
               .Where(r => r.CarID == s.car.ID && r.EndDate > DbFunctions.TruncateTime(DateTime.Now))
               .Select(n => new { n.StartDate, n.EndDate })
           }).AsQueryable();
        }


    [Route("FullDetails/{id}")]
        [ResponseType(typeof(FullCarDetails))]
        public IHttpActionResult GetFullDetailsByID(int id)
        {
            var res = db.Cars.Join(db.CarTypes, car => car.CarTypeID, ct => ct.ID,
                (car, ct) => new { car, ct })
           .Where(c => c.car.IsDriveable && c.car.IsAvailable && c.car.ID == id).Select(s => new FullCarDetails
           {
               ID = s.car.ID,
               Year = s.ct.Year,
               Image = s.car.Image,
               KM = s.car.KM,
               BranchID = s.car.BranchID,
               LatePrice = s.ct.LatePrice,
               DailyPrice = s.ct.DailyPrice,
               Gear = s.ct.Gear,
               Manufactor = s.ct.Manufactor,
               Model = s.ct.Model,
               Dates = db.RentDetails
               .Where(r => r.CarID == s.car.ID && r.EndDate > DbFunctions.TruncateTime(DateTime.Now))
               .Select(n => new { n.StartDate, n.EndDate })
           }).FirstOrDefault();
            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        [Route("Return-Car/{carID}/{branchID}")]
        [HttpGet]
        [ResponseType(typeof(spReturnCar_Result))]
        public IHttpActionResult ReturnCar(int carID, int branchID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var out1 = new ObjectParameter("CarTableID", typeof(int));
            var out2 = new ObjectParameter("RentID", typeof(int));
            var res = db.spReturnCar(carID, out1, out2).ToList();
            int? carTableID = out1.Value as int?;
            int? rentID = out2.Value as int?;
            if (rentID == null)
            {
                return BadRequest();
            }

            Car car = db.Cars.Find((int)carTableID);
            car.IsAvailable = true;
            car.BranchID = branchID;
            db.RentDetails.Find(rentID).ActualEndDate = DateTime.Now.Date;
            db.SaveChanges();

            return Ok(res);
        }

        // PUT: api/Cars/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCar(int id, Car car)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != car.ID)
            {
                return BadRequest();
            }

            db.Entry(car).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(id))
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

        // POST: api/Cars
        [ResponseType(typeof(Car))]
        public IHttpActionResult PostCar(Car car)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Cars.Add(car);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = car.ID }, car);
        }

        // DELETE: api/Cars/5
        [ResponseType(typeof(Car))]
        public IHttpActionResult DeleteCar(int id)
        {
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return NotFound();
            }

            db.Cars.Remove(car);
            db.SaveChanges();

            return Ok(car);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CarExists(int id)
        {
            return db.Cars.Count(e => e.ID == id) > 0;
        }
    }
}