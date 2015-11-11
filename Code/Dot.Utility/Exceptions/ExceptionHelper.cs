using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;

namespace Dot.Utility.Exceptions
{
    public class ExceptionHelper
    {
        public static string ToErrorMessage(ModelStateDictionary ModelState)
        {
            System.Text.StringBuilder errs = new System.Text.StringBuilder();
            foreach (var item in ModelState.Values)
            {
                foreach (var err in item.Errors)
                {
                    errs.Append(err.ErrorMessage);
                }
            }
            return errs.ToString(); 
        }

    }
}
