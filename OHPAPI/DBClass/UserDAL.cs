using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OHPAPI.Models;

namespace OHPAPI.DBClass
{
    public class UserDAL
    {
        OHPDBEntities dbcontext = new OHPDBEntities();
        
        /// <summary>
        /// Get admin user details for token generation
        /// </summary>
        /// <param name="username">email Id</param>
        /// <param name="password">password</param>
        /// <returns>return full user details</returns>
        public User GetAdminUser(string username, string password)
        {
            string decryptedPassword = CryptoEngine.Encrypt(password, "sblw-3hn8-sqoy19");
            var user = dbcontext.Users.FirstOrDefault(a => a.Email.ToLower() == username.ToLower()
                                                            && a.Password == decryptedPassword
                                                            && a.IsAdmin == true);
            return user;
        }

        /// <summary>
        /// Get authorized non-admin user details for login
        /// </summary>
        /// <param name="username">email Id</param>
        /// <param name="password">password</param>
        /// <returns>return full user details</returns>
        public User Login(string username, string password)
        {
            string decryptedPassword = CryptoEngine.Encrypt(password, "sblw-3hn8-sqoy19");
            var user = dbcontext.Users.FirstOrDefault(a => a.Email.ToLower() == username.ToLower() 
                                                            && a.Password == decryptedPassword
                                                            && a.IsAdmin == true);
            return user;
        }

        /// <summary>
        /// Get user detail using bearer access token value
        /// </summary>
        /// <returns></returns>
        public User GetLoggedInUser()
        {
            var user = dbcontext.Users.FirstOrDefault(a => a.ID == 1);
            return user;
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="user">all the data of new user</param>
        /// <returns>newly registered user's Id</returns>
        public long Register(User user)
        {
            string encryptedPassword = CryptoEngine.Encrypt(user.Password, "sblw-3hn8-sqoy19");
            user.Password = encryptedPassword;
            dbcontext.Users.Add(user);
            dbcontext.SaveChanges();

            return user.ID;
        }

        /// <summary>
        /// Update user details
        /// </summary>
        /// <param name="user">updated user object from frontend</param>
        /// <returns>updated user Id</returns>
        public long UpdateUser(User user)
        {   
            var userdata = dbcontext.Users.FirstOrDefault(a => a.ID == user.ID);

            userdata.FirstName = user.FirstName;
            userdata.LastName = user.LastName;
            userdata.Email = user.Email;
            userdata.Mobile = user.Mobile;
            userdata.Password = CryptoEngine.Encrypt(user.Password, "sblw-3hn8-sqoy19");
            userdata.IsActive = user.IsActive;
            userdata.IsAdmin = user.IsAdmin;

            dbcontext.SaveChanges();

            return userdata.ID;
        }

    }
}