using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuxAsp.Authorizations;
using LuxAsp.Sessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LuxAsp.EFCore.TestSuite.Pages
{
    [LuxSessionFilter]
    public class IndexModel : PageModel
    {
        
        public void OnGet(
            [FromServices] ILuxAuthentication Session,
            [FromServices] LuxUserRepository User) 
        {
            var Se = Session.GetUserModel();

            var U = User.Load(X => X.Where(Y => Y.LoginName == "admin")).FirstOrDefault();
            Session.SetUserModel(U);
        }
    }
}
