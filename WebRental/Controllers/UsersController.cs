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
    [RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
        private CarRentalEntities db = new CarRentalEntities();

        // GET: api/Users
        public IQueryable<spGetUsers_Result> GetUsers()
        {
            return db.spGetUsers().AsQueryable();
        }

        [Route("Auth")]
        [HttpPost]
        [ResponseType(typeof(User))]
        public IHttpActionResult Auth([FromBody]UserForAuth user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var output = new ObjectParameter("ResultID", typeof(int));
            db.spAuthUser(user.Username, user.Password, output);
            int? userID = output.Value as int?;
            if (userID != null)
            {
                if (db.Employees.Count(e => e.UserID == userID) > 0)
                {
                    return Ok(db.spGetEmployeeWithUserByID(userID));
                }
                else
                {
                    return Ok(db.spGetUserByID(userID));
                }
            }
            else
            {
                return NotFound();
            }

        }

        [Route("With-Employees/{id}")]
        [HttpGet]
        public IQueryable<spGetUsersAndEmloyeesForManager_Result> GetWithEmployeesByBranch([FromUri]int id)
        {
            return db.spGetUsersAndEmloyeesForManager(id).AsQueryable();
        }

        [Route("With-Employees/{id}/{b_id}")]
        [HttpGet]
        public IHttpActionResult GetWithEmployeesByBranchAndID(int id, int b_id)
        {
            var res = GetWithEmployeesByBranch(b_id).Where(u => u.ID == id).SingleOrDefault();
            if (res == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(res);
            }
        }


        // GET: api/Users/5
        [ResponseType(typeof(spGetUserByID_Result))]
        public IHttpActionResult GetUser(int id)
        {
            var user = db.spGetUserByID(id).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.ID)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Users.Add(user);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = user.ID }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.ID == id) > 0;
        }
    }
}