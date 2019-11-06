using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using AirData.DataAccess;
using AirData.Business.Users;
using AirData.Business;
using Firebase.Auth;




namespace TestMixArch.Controllers
{
    public class LoginController : Controller
    {
        public async Task SubmitLogin(string email, string password)
        {
            AirDataContext DbContext = new AirDataContext();
            UserEntity user = null;
            try
            {
                if (!String.IsNullOrEmpty(email))
                {
                    if (!String.IsNullOrEmpty(password))
                    {
                        string firebaseApiKey = ConfigurationManager.AppSettings["Firebase.ApiKey"];
                        FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(firebaseApiKey));
                        FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.SignInWithEmailAndPasswordAsync(email, password);
                        
                        user = UserEntity.GetByFirebaseId(DbContext, firebaseAuthLink.User.LocalId);
                        if (user == null)
                        {
                            user = UserEntity.Get(DbContext, email);
                        }
                        user.VerifyStatus();

                        Security.LoginUser(user, firebaseAuthLink, true);
                        this.Response.Redirect("/");
                    }
                }

            }
            catch (FirebaseAuthException ex)
            {
                if (ex.Reason == AuthErrorReason.WrongPassword)
                {
                    //int retryCount = int.Parse(); Need to find a way to count retry attempts in razor.
                    //if (retryCount > 1)
                    //{
                    //}
                }
            }

        }
    }
}