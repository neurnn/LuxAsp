using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuxAsp.Sessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LuxAsp.EFCore.TestSuite.Pages
{
    [LuxSessionFilter]
    public class IndexModel : PageModel
    {
        
        public void OnGet([FromServices] ILuxSession Session) 
        {
            Session.SetString("!!", "!!");
        }
    }
}
