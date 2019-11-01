using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using System.Configuration;
using System.Web.UI;
using Mythril.Exceptions;

using AirData.Business;
using AirData.Business.Users;
using AirData.DataAccess;

using Firebase.Auth;


namespace TestMixArch.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        string Username { get; set; }
        string Password { get; set; }

        public string test()
        {
            return "Test";
        }

        public void OnPost()
        {
            Response.Redirect("/forgot-password.cshtml");
        }

        public string Test()
        {
            return "Tester";
        }

        public async Task Login(string username, string password)
        {
            UserEntity user = null;
            AirDataContext DbContext = new AirDataContext();
            try
            {
                //string email = Framework.FieldValidation.ValidateStringField(this.txtEmail, "Email Address", 200, true).ToLower();
                //string password = Framework.FieldValidation.ValidateStringField(this.txtPassword, "Password", int.MaxValue, 6, true);

                string firebaseApiKey = ConfigurationManager.AppSettings["Firebase.ApiKey"];
                FirebaseAuthProvider authProvider = new FirebaseAuthProvider(new FirebaseConfig(firebaseApiKey));
                FirebaseAuthLink auth = await authProvider.SignInWithEmailAndPasswordAsync(username, password);

                user = UserEntity.GetByFirebaseId(DbContext, auth.User.LocalId);

                if (user == null) // For some reason the fb auth id didn't get set. Try getting user by email
                {
                    user = UserEntity.Get(DbContext, username);
                }

                user.VerifyStatus();

                // create the cookie and record the event and other items
                Security.LoginUser(user, auth, true);

                Global.TrackEvent("Login_Success", null, user);

                // redirect the user to the root redirector page
                this.Response.Redirect("/", false);
            }
            catch (FirebaseAuthException ex)
            {
                //if (ex.Reason == AuthErrorReason.WrongPassword)
                //{
                //    int retryCount = int.Parse(this.hidPasswordFailCount.Value);
                //    if (retryCount > 1)
                //    {
                //        BootstrapUtils.ShowMessageAndRedirect(this, "Wrong Password", "It appears you have forgotten your password. We will now redirect you to the forgot password page", "forgot-password.aspx");
                //        return;
                //    }

                //    retryCount += 1;
                //    this.hidPasswordFailCount.Value = retryCount.ToString();
                //    BootstrapUtils.ShowMessage(this, "Wrong Password", BootstrapNotifyType.danger);
                //    return;
                //}
                //BootstrapUtils.ShowMessage(this, ex.Message, BootstrapNotifyType.danger);
            }
            catch (FieldValidationException ex)
            {
                if (user != null)
                {
                    Dictionary<string, string> info = new Dictionary<string, string>()
                    {
                        {"Error_Message", ex.Message}
                    };
                    Global.TrackEvent("Login_Failure", info, user);
                }
                //BootstrapUtils.ShowMessage(this, ex.Message, BootstrapNotifyType.danger);
                return;
            }
        }
    }
}