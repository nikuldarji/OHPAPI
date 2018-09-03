using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OHPAPI.DBClass;
using OHPAPI.Models;

namespace OHPAPI.Controllers
{
    public class LoginController : ApiController
    {

        #region Declaration

        #region Entity Objects

            User user = new User();

        #endregion

        #region Dal Objects

            UserDAL userDal = new UserDAL();

        #endregion

        #endregion

        #region API

        [Route("api/Login/Login")]
        [HttpGet]
        [Authorize]
        public User Login(string username, string password)
        {
            var user = userDal.Login(username, password);
            return user;
        }

        [Route("api/Login/GetUserDetail")]
        [HttpGet]
        [Authorize]
        public User GetUserDetail()
        {
            var user = userDal.GetLoggedInUser();
            return user;
        }

        [Route("api/Login/Register")]
        [HttpPost]
        [AllowAnonymous]
        public long Register(User user)
        {
            return userDal.Register(user);
        }

        [HttpPost]
        [Authorize]
        public long Update(User user)
        {
            return userDal.UpdateUser(user);
        }

        #endregion

    }
}
